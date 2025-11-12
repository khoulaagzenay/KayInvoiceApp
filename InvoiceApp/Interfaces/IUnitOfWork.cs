using InvoiceApp.Models;

namespace InvoiceApp.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Client> clients { get; }
        IGenericRepository<Invoice> invoices { get; }
        IGenericRepository<InvoiceLine> invoiceLine { get; }
        Task <int> CompleteAsync();
    }
}
