using InvoiceApp.Interfaces;
using InvoiceApp.Models;
using InvoiceApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork myUnit;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork myUnit)
        {
            _logger = logger;
            this.myUnit = myUnit;
        }

        public async Task<IActionResult> Index()
        {
            var invoices = (await myUnit.invoices.GetAllAsync("InvoiceLines,Client")).Cast<Invoice>().ToList();

            var clients = (await myUnit.clients.GetAllAsync()).Cast<Client>().ToList();

            int totalInvoices = invoices.Count;
            int totalClients = clients.Count;

            decimal outstanding = invoices.Sum(i => i.TotalAmount);

            var recentInvoices = invoices
                .OrderByDescending(i => i.InvoiceDate)
                .Take(5)
                .ToList();

            var recentClients = clients
                .OrderByDescending(c => c.Id)
                .Take(5)
                .ToList();

            var vm = new DashboardViewModel
            {
                TotalInvoices = totalInvoices,
                TotalClients = totalClients,
                OutstandingBalance = outstanding,
                RecentInvoices = recentInvoices,
                RecentClients = recentClients
            };

            return View(vm);
        }
    }
}
