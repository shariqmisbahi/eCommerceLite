using eCommerceLite.Data;
using eCommerceLite.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;
using System.Net.Mail;

namespace eCommerceLite.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly OrderContext _context;

        public OrderService(OrderContext context)
        {
            _context = context;
        }

        public async Task<(List<OrderRequest>, int)> GetOrdersAsync(int page, int pageSize)
        {
            var totalOrders = await _context.Orders.CountAsync();
            var orders = await _context.Orders
                .OrderBy(o => o.updateDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderRequest
                {
                    Id = o.Id,
                    fullName = o.fullName,
                    deliveryDate = o.deliveryDate,
                    IsDispatched = o.IsDispatched,
                    updateDate = o.updateDate
                }).ToListAsync();

            return (orders, totalOrders);
        }

        public void SendEmailWithInvoice(string toEmail, string trackingUrl, string invoicePath)
        {
            var fromEmail = new MailAddress("shariqmisbahi@gmail.com");
            var appPassword = "kjbq rmam jaxt afsg"; // Google App password

            var message = new MailMessage(fromEmail, new MailAddress(toEmail))
            {
                Subject = "Your Cake Order Has Been Dispatched!",
                Body = $"Hi, your order has been dispatched.\n\nYou can track your order here:\n{trackingUrl}\n\nPlease find the invoice attached.",
                IsBodyHtml = false
            };

            if (System.IO.File.Exists(invoicePath))
            {
                message.Attachments.Add(new Attachment(invoicePath)); // Optional
            }

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail.Address, appPassword),
                EnableSsl = true
            };

            smtp.Send(message);
        }
    }
}
