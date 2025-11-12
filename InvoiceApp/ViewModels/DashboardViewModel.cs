using InvoiceApp.Models;

namespace InvoiceApp.ViewModels
{
    public class DashboardViewModel
    {
            public int TotalInvoices { get; set; }
            public int TotalClients { get; set; }
            public decimal OutstandingBalance { get; set; }

            public List<Invoice> RecentInvoices { get; set; } = new();
            public List<Client> RecentClients { get; set; } = new();

        public List<string> MonthLabels { get; set; } = new();  
        public List<decimal> MonthlyRevenues { get; set; } = new(); 
    }
}
