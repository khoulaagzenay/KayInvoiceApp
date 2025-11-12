using System.ComponentModel.DataAnnotations;

namespace InvoiceApp.Models
{
    public class Client
    {
        [Key]
        public int? Id { get; set; } 

        [Required(ErrorMessage = "The customer name is required.")]
        [StringLength(100)]
        public string Name { get; set; }
        [Required(ErrorMessage = "The SIREN number is required.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The SIREN number must contain exactly 9 digits.")]
        public string Siren { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    }
}



