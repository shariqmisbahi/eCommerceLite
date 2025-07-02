using System.ComponentModel.DataAnnotations;

namespace eCommerceLite.Model
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string flavor { get; set; }        
        public bool IsDispatched { get; set; }

        public string GenerateInvoiceLink => $"/api/orders/{Id}/generate-invoice";
        public string DispatchLink => $"/api/orders/{Id}/dispatch";
    }
}


