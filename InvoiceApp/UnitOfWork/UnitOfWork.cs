using InvoiceApp.Data;
using InvoiceApp.Interfaces;
using InvoiceApp.Models;
using InvoiceApp.Repositories;

namespace InvoiceApp.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            clients = new GenericRepository<Client>(_context);
            invoices = new GenericRepository<Invoice>(_context);
            invoiceLine = new GenericRepository<InvoiceLine>(_context);
        }
        public IGenericRepository<Client> clients { get; private set; }

        public IGenericRepository<Invoice> invoices { get; private set; }

        public IGenericRepository<InvoiceLine> invoiceLine { get; private set; }

        public async Task<int> CompleteAsync()
        {
           return await _context.SaveChangesAsync();  
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
