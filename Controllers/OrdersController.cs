using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using eCommerceLite.Data;
using eCommerceLite.Model;
using eCommerceLite.OrderService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.RegularExpressions;

namespace eCommerceLite.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderContext _context;
        private readonly IOrderService _orderService; 

        public OrdersController(OrderContext context, IOrderService orderService) 
        {
            _context = context;
            _orderService = orderService; 
        }

        [HttpPost("receive")]
        public async Task<IActionResult> ReceiveOrder([FromBody] OrderRequest order)
        {
            Console.Beep();
            if (order == null)
            {
                return BadRequest("Invalid order data.");
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Order saved successfully!" });
        }
    

     [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (orders, totalCount) = await _orderService.GetOrdersAsync(page, pageSize);
            return Ok(new
            {
                Data = orders,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [HttpPost("{id}/generate-invoice")]
        public async Task<IActionResult> GenerateInvoice(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            // 1. Generate PDF and save it in Templates
            //string invoiceFileName = $"Invoice_{id:0000}.pdf";
            string invoiceFileName = $"Invoice.pdf";
            string invoicePath = Path.Combine("Templates", invoiceFileName);

            // 2. Save the file if needed — this is a placeholder
            if (!System.IO.File.Exists(invoicePath))
            {
                System.IO.File.WriteAllText(invoicePath, $"Invoice content for Order #{id}");
            }

            // 3. Construct public URL to return
            var fileUrl = $"{Request.Scheme}://{Request.Host}/templates/{invoiceFileName}";
            order.InvoiceGenerated = true;
            await _context.SaveChangesAsync();
            // 4. Return as JSON
            return Ok(new
            {
                orderId = id,
                invoiceUrl = fileUrl
            });
        }



        [HttpPost("{id}/generate-invoice-new")]
        public async Task<IActionResult> GenerateInvoiceNew(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            // Fetch price from ProductEmbeddings using the flavor
            var product = await _context.ProductEmbeddings
                .FirstOrDefaultAsync(p => p.productName == order.flavor);

            if (product == null)
                return BadRequest($"Product '{order.flavor}' not found in ProductEmbeddings.");

            // Parse weight from cakeSize (e.g. "1.5kg" → 1.5)
            double weight = 1.0; // default
            var sizeText = order.cakeSize?.ToLower().Replace("kg", "").Trim();
            if (!double.TryParse(sizeText, out weight))
                weight = 1.0; // fallback

            var price = product.productPrice;
            var amount = price * (decimal)weight;

            // Generate invoice file
            string templatePath = Path.Combine("Templates", "InvoiceTemplate.docx");
            string invoiceFileName = $"Invoice_{id:0000}.docx";
            string invoicePath = Path.Combine("Templates", invoiceFileName);

            System.IO.File.Copy(templatePath, invoicePath, true);

            using (WordprocessingDocument doc = WordprocessingDocument.Open(invoicePath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var text = body.Descendants<Text>().ToList();

                foreach (var t in text)
                {
                    if (t.Text.Contains("fullName"))
                        t.Text = t.Text.Replace("fullName", order.fullName ?? "");

                    if (t.Text.Contains("fullDeliveryAddress"))
                        t.Text = t.Text.Replace("fullDeliveryAddress", order.fullDeliveryAddress ?? "");

                    if (t.Text.Contains("phoneNumber"))
                        t.Text = t.Text.Replace("phoneNumber", order.phoneNumber ?? "");

                    if (t.Text.Contains("flavor"))
                        t.Text = t.Text.Replace("flavor", order.flavor ?? "");

                    if (t.Text.Contains("cakeSize"))
                        t.Text = t.Text.Replace("cakeSize", order.cakeSize ?? "");

                    if (t.Text.Contains("price"))
                        t.Text = t.Text.Replace("price", price.ToString("0.00"));

                    if (t.Text.Contains("amount"))
                        t.Text = t.Text.Replace("amount", amount.ToString("0.00"));
                }

                doc.MainDocumentPart.Document.Save();
            }

            var fileUrl = $"{Request.Scheme}://{Request.Host}/templates/{invoiceFileName}";
            order.InvoiceGenerated = true;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                orderId = id,
                invoiceUrl = fileUrl,
                pricePerKg = price,
                quantity = weight,
                finalAmount = amount
            });
        }












        [HttpPost("{id}/dispatch")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> DispatchOrder(int id, [FromForm] DispatchRequest request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.IsDispatched = true;
            await _context.SaveChangesAsync();

            // Save invoice file
            var uploadsPath = Path.Combine("wwwroot", "invoices");
            Directory.CreateDirectory(uploadsPath);
            var filePath = Path.Combine(uploadsPath, request.InvoiceFile.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.InvoiceFile.CopyToAsync(stream);
            }

            // Send email
            _orderService.SendEmailWithInvoice(order.email, request.TrackingUrl, filePath);

            return Ok(new { message = "Order dispatched and email sent." });
        }

        //[HttpPost("{id}/generate-invoice")]
        //public async Task<IActionResult> GenerateInvoice(int id)
        //{
        //    var order = await _context.Orders.FindAsync(id);
        //    if (order == null) return NotFound();

        //    // Fake invoice URL
        //    //order.InvoiceUrl = $"https://yourdomain.com/invoices/{id}.pdf";
        //    // string InvoiceUrl = "https://yourdomain.com/invoices/" + id + ".pdf";
        //    string invoiceFileName = "Invoice_0806.pdf";
        //    string invoicePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", invoiceFileName);
        //    string InvoiceUrl = invoicePath;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Invoice generated", InvoiceUrl = InvoiceUrl });
        //}

        //[HttpPost("{id}/dispatch")]
        //public async Task<IActionResult> DispatchOrder(int id)
        //{
        //    var order = await _context.Orders.FindAsync(id);
        //    if (order == null) return NotFound();

        //    order.IsDispatched = true;
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Order dispatched" });
        //}



    }


}



//[ApiController]
//[Route("api/[controller]")]
//public class OrdersController : ControllerBase
//{
//    [HttpPost("receive")]
//    public IActionResult ReceiveOrder([FromBody] OrderRequest order)
//    {
//        if (order == null)
//        {
//            return BadRequest("Invalid order data.");
//        }

//        // Example: log or process the order
//        Console.WriteLine($"🎂 Order received from {order.fullName}");
//        Console.WriteLine($"📞 Phone: {order.phoneNumber}");
//        Console.WriteLine($"📦 Cake Size: {order.cakeSize}");
//        Console.WriteLine($"📅 Delivery Date: {order.deliveryDate}");
//        Console.WriteLine($"📍 Address: {order.fullDeliveryAddress}");

//        // You can save to a database or file here...

//        return Ok(new { message = "Order received successfully!" });
//    }
//}
