using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp.Models
{
    public class InvoiceLine
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;
        public decimal TaxRate { get; set; } 
        public decimal AmountBeforeTax { get; set; } 
        public decimal TaxAmount { get; set; }       
        public decimal AmountWithTax { get; set; }   

        [ForeignKey(nameof(Invoice))]
        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; }
    }
}
