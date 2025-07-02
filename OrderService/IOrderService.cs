using eCommerceLite.Model;

namespace eCommerceLite.OrderService
{
    public interface IOrderService
    {
        Task<(List<OrderRequest>, int)> GetOrdersAsync(int page, int pageSize);

        void SendEmailWithInvoice(string toEmail, string trackingUrl, string invoicePath);
    }
}
