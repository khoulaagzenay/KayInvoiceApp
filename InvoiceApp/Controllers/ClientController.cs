using InvoiceApp.Interfaces;
using InvoiceApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        private readonly IUnitOfWork myUnit;

        public ClientController(IUnitOfWork myUnit)
        {
            this.myUnit = myUnit;
        }
        public async Task<IActionResult> Index()
        {
            return View(await myUnit.clients.GetAllAsync());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            var existingClient = (await myUnit.clients.GetAllAsync())
        .FirstOrDefault(c => c.Name.ToLower() == client.Name.ToLower());

            if (existingClient != null)
            {
                ModelState.AddModelError("Name", "This customer name already exists.");
            }

            if (ModelState.IsValid)
            {
                return View(client);
            }

            await myUnit.clients.AddAsync(client);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var client = await myUnit.clients.GetByIdAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Client client)
        {
            var existingClient = (await myUnit.clients.GetAllAsync()).FirstOrDefault
                (c => c.Name.ToLower() == client.Name.ToLower() && c.Id != client.Id);

            if (existingClient != null)
            {
                ModelState.AddModelError("Name", "This customer name already exists.");
            }
            if (!ModelState.IsValid)
                return View(client);

            var clientInDb = await myUnit.clients.GetByIdAsync(client.Id.Value);
            if (clientInDb == null)
                return NotFound();

            clientInDb.Name = client.Name;
            clientInDb.Siren = client.Siren;
            clientInDb.Email = client.Email;
            clientInDb.PhoneNumber = client.PhoneNumber;
            clientInDb.Address = client.Address;

            await myUnit.clients.UpdateAsync(clientInDb);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null && id == 0)
            {
                return NotFound();
            }
            var client = await myUnit.clients.GetByIdAsync(id.Value);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            await myUnit.clients.DeleteAsync(id.Value);
            return RedirectToAction("Index");
        }
    }
}
