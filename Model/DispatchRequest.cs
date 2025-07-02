using Microsoft.AspNetCore.Http;

namespace eCommerceLite.Model
{
    public class DispatchRequest
{
    public string TrackingUrl { get; set; }
    public IFormFile InvoiceFile { get; set; }
}
}
