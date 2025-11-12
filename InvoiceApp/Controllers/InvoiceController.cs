using InvoiceApp.Interfaces;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InvoiceApp.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IUnitOfWork myUnit;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoiceController(IUnitOfWork _myUnit, UserManager<ApplicationUser> userManager)
        {
            this.myUnit = _myUnit;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Invoice> listinvoices = await myUnit.invoices.GetAllAsync(includeProperties: "Client");
            return View(listinvoices);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var clients = await myUnit.clients.GetAllAsync();
            ViewBag.Clients = clients;
            return View(new InvoiceCreateViewModel
            {
                InvoiceLines = new List<InvoiceLineViewModel>{new InvoiceLineViewModel() }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InvoiceCreateViewModel model)
        {
            var existingClient = (await myUnit.invoices.GetAllAsync())
       .FirstOrDefault(i => i.InvoiceNumber.ToLower() == model.InvoiceNumber.ToLower());

            if (existingClient != null)
            {
                ModelState.AddModelError("Name", "This invoice number already exists.");
            }
            ViewBag.Clients = await myUnit.clients.GetAllAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            decimal subTotal = 0;
            decimal totalTax = 0;

            var invoice = new Invoice
            {
                InvoiceDate = model.InvoiceDate,
                ClientId = model.ClientId,
                InvoiceNumber = model.InvoiceNumber,
                InvoiceLines = new List<InvoiceLine>()
            };

            foreach (var line in model.InvoiceLines)
            {
                var lineHT = line.Quantity * line.UnitPrice;
                var lineTVA = lineHT * (line.TaxRate / 100);
                subTotal += lineHT;
                totalTax += lineTVA;

                invoice.InvoiceLines.Add(new InvoiceLine
                {
                    Description = line.Description,
                    Quantity = line.Quantity,
                    UnitPrice = line.UnitPrice,
                    TaxRate = line.TaxRate,
                    AmountBeforeTax = lineHT,
                    TaxAmount = lineTVA,
                    AmountWithTax = lineHT + lineTVA,
                    Invoice = invoice
                });
            }

            invoice.SubTotal = subTotal;
            invoice.TotalTax = totalTax;
            invoice.TotalAmount = subTotal + totalTax;

            await myUnit.invoices.AddAsync(invoice);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await myUnit.invoices.GetByIdWithIncludesAsync(id, i => ((Invoice)i).InvoiceLines);
            if (invoice == null)
            {
                return NotFound();
            }

            var clients = await myUnit.clients.GetAllAsync();
            ViewBag.Clients = clients;

            var model = new InvoiceCreateViewModel
            {
                InvoiceDate = invoice.InvoiceDate,
                ClientId = invoice.ClientId.GetValueOrDefault(),
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceLines = invoice.InvoiceLines.Select(l => new InvoiceLineViewModel
                {
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    TaxRate = l.TaxRate
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InvoiceCreateViewModel model)
        {
            ViewBag.Clients = await myUnit.clients.GetAllAsync();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var invoice = await myUnit.invoices.GetByIdWithIncludesAsync(id, i => ((Invoice)i).InvoiceLines);

            if (invoice == null)
            {
                return NotFound();
            }

            // Recalcul des totaux
            decimal subTotal = 0;
            decimal totalTax = 0;
            foreach (var line in model.InvoiceLines)
            {
                var lineTotalHT = line.Quantity * line.UnitPrice;
                var lineTVA = lineTotalHT * (line.TaxRate / 100);
                subTotal += lineTotalHT;
                totalTax += lineTVA;
            }
            decimal totalTTC = subTotal + totalTax;

            // Mise à jour de l'objet Invoice
            invoice.InvoiceDate = model.InvoiceDate;
            invoice.ClientId = model.ClientId;
            invoice.InvoiceNumber = model.InvoiceNumber;
            invoice.TotalAmount = totalTTC;
            invoice.InvoiceLines = model.InvoiceLines.Select(l => new InvoiceLine
            {
                Description = l.Description,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice,
                TaxRate = l.TaxRate
            }).ToList();

            await myUnit.invoices.UpdateAsync(invoice);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await myUnit.invoices.GetByIdWithIncludesAsync(id, i => ((Invoice)i).InvoiceLines);
            if (invoice == null)
            {
                return NotFound();
            }
            var model = new InvoiceCreateViewModel
            {
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceDate = invoice.InvoiceDate,
                ClientId = invoice.ClientId.GetValueOrDefault(),
                InvoiceLines = invoice.InvoiceLines.Select(l => new InvoiceLineViewModel
                {
                    Description = l.Description,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    TaxRate = l.TaxRate
                }).ToList()
            };
            ViewBag.Clients = await myUnit.clients.GetAllAsync();
            ViewBag.InvoiceId = invoice.Id;
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await myUnit.invoices.GetByIdWithIncludesAsync(id, i => ((Invoice)i).InvoiceLines);
            if (invoice == null)
            {
                return NotFound();
            }
            await myUnit.invoices.DeleteAsync(id);

            return RedirectToAction("Index");
        }
        private async Task<ApplicationUser> GetCurrentUserAsync() => await _userManager.GetUserAsync(User);
        public async Task<IActionResult> Download(int id)
        {
            var invoice = myUnit.invoices.GetAllAsync(includeProperties: "Client,InvoiceLines")
                .Result.FirstOrDefault(
                i => i.Id == id
            );

            if (invoice == null)
                return NotFound();
            var user = await GetCurrentUserAsync();

            var pdfBytes = InvoicePdfGenerator.Generate(invoice, user);

            return File(pdfBytes, "application/pdf", $"Facture_{invoice.InvoiceNumber}.pdf");
        }
    }

}

