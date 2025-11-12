using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvoiceApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? CompanyName { get; set; }
        public string? Address { get; set; }
        public string? SIREN { get; set; }

        [NotMapped]
        public IFormFile LogoFile { get; set; }
        public byte[]? dbLogo { get; set; }
        [NotMapped]
        public string? LogoImagesrc
        {
            get
            {
                if (dbLogo != null)
                {
                    string base64String = Convert.ToBase64String(dbLogo, 0, dbLogo.Length);
                    return "data:image/png;base64," + base64String;
                }
                else
                {
                    return string.Empty;
                }
            }

        }
    }
}
