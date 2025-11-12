using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.ViewModels
{
    public class InvoiceLineViewModel
    {
        [Required]
        public string? Description { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity invalide")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price invalide")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Display(Name = "Tax Rate")]
        public decimal TaxRate { get; set; } 
    }
}

