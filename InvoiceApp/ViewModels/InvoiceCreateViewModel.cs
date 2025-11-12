using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.ViewModels
{
    public class InvoiceCreateViewModel
    {
        [Required]
        public string? InvoiceNumber { get; set; }

        [Required]
        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Client")]
        public int ClientId { get; set; }
        public List<InvoiceLineViewModel> InvoiceLines { get; set; } = new List<InvoiceLineViewModel>();
    }
}
