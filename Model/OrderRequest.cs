using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore.Design;

namespace eCommerceLite.Model
{
    public class OrderRequest
    {
        [Key] // ✅ This marks Id as the primary key
        public int Id { get; set; }
        
        public string email { get; set; }
        [Required]
        public string fullName { get; set; }
        [Required]
        public string phoneNumber { get; set; }
        [Required]
        public string whatAppNumber { get; set; }
        [Required]
        public string cakeSize { get; set; }
        [Required]
        public string flavor { get; set; }
        public string customFlavor { get; set; }
        public string colourTheme { get; set; }
        public string referenceDesign { get; set; }
        public string messageOnCake { get; set; }
        [Required]
        public string fullDeliveryAddress { get; set; }
        [Required]
        public DateTime deliveryDate { get; set; }
        [Required]
        public string deliveryTime { get; set; }
        public string specialInstructions { get; set; }

        public bool InvoiceGenerated { get; set; } = false;
        public bool IsDispatched { get; set; } = false;

        [Required]
        public DateTime updateDate { get; set; }
    }
}


