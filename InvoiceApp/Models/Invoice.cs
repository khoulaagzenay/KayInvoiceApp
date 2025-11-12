using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp.Models
{
    public class Invoice
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string InvoiceNumber { get; set; }  = string.Empty;
        [Required]
        public DateTime InvoiceDate { get; set; }
        public decimal SubTotal { get; set; } 
        public decimal TotalTax { get; set; }  
        public decimal TotalAmount { get; set; }

        // Foreign key to Client
        [ForeignKey(nameof(Client))]
        public int? ClientId { get; set; }
        public Client? Client { get; set; }
        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    }
}
