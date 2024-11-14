using Binary.Utilities;
using BinaryDecimalStore.Models.View_Models;
using iText.IO.Font.Constants;
using iText.IO.Image;

namespace BinaryDecimalStore;
using iText.Kernel.Font;

using System;
using System.IO;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

public class OrderConfirmationPdfGenerator
{
    public  byte[] GeneratePdfAsync(orderManagVM orderVM)
    {
        // Create a new PDF document
        using (var ms = new MemoryStream())
        {
            PdfWriter writer = new PdfWriter(ms);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
            
            // Add header with logo and title
            Image logo = new Image(ImageDataFactory.Create("wwwroot/Gemini_Generated_Image_kqhu1ckqhu1ckqhu.jpeg"))
                .SetWidth(UnitValue.CreatePointValue(100))
                .SetHeight(UnitValue.CreatePointValue(100))
                .SetMarginBottom(10);
            Paragraph header = new Paragraph("Order Confirmation")
                .SetFont(font)
                .SetFontSize(18)
                .SetTextAlignment(TextAlignment.CENTER);
            document.Add(logo);
            document.Add(header);

            // Add order details
            Paragraph customerName = new Paragraph($"Customer: {orderVM.orderHeader.Name}")
                .SetMarginTop(20);
            Paragraph customerPhone = new Paragraph($"Phone: {orderVM.orderHeader.PhoneNumber}");
            Paragraph customerEmail = new Paragraph($"City: {orderVM.orderHeader.City}");
            Paragraph shippingAddress = new Paragraph($"Shipping Address: {orderVM.orderHeader.Address}");
            Paragraph paymentStatus = new Paragraph($"Payment Status: {orderVM.orderHeader.PaymentStatus}");
            document.Add(customerName);
            document.Add(customerPhone);
            document.Add(customerEmail);
            document.Add(shippingAddress);
            document.Add(paymentStatus);

            // Add order items table
            Table itemsTable = new Table(UnitValue.CreatePercentArray(new float[] { 2, 1, 1, 1 }))
                .SetWidth(UnitValue.CreatePercentValue(100));
            itemsTable.AddHeaderCell("Item");
            itemsTable.AddHeaderCell("Qty");
            itemsTable.AddHeaderCell("Price");
            itemsTable.AddHeaderCell("Total");
            
            double finalPrice, price;
            string currencyCode = orderVM.orderHeader.OrderCurrencyCode;
            string currencyName = StaticData.getCurrencyName(currencyCode);
            double totalBill = 0;
            foreach (var item in orderVM.orderDetails)
            {
                price = item.Product.price;
                finalPrice =price - price * item.Product.discountRatio / 100;
                 
                itemsTable.AddCell(item.Product.Name);
                itemsTable.AddCell(item.count.ToString());
                string pFormat = StaticData.getPrice(finalPrice, currencyName).ToString();
                itemsTable.AddCell(pFormat);
                string finalFormat = StaticData.getPrice(finalPrice * item.count , currencyName).ToString();
                itemsTable.AddCell(finalFormat);

                totalBill += finalPrice * item.count;
            }
            document.Add(itemsTable);

            // Add total price
            Paragraph totalPrice = new Paragraph($"Total: {StaticData.getPrice(totalBill, currencyName)}")
                .SetMarginTop(20)
                .SetTextAlignment(TextAlignment.RIGHT);
            document.Add(totalPrice);
            
            // Add expected delivery date and contact info
            Paragraph expectedDelivery = new Paragraph($"Expected delivery: {orderVM.orderHeader.OrderDate.AddDays(7).ToShortDateString()}")
                .SetMarginTop(20);
            Paragraph contactInfo = new Paragraph("Feel free to contact us at Ayman.yassien.fci@gmail.com if you have any questions.")
                .SetMarginTop(10);
            document.Add(expectedDelivery);
            document.Add(contactInfo);

            // Add closing message
            Paragraph closingMessage = new Paragraph("Thank you for your order!")
                .SetMarginTop(20)
                .SetTextAlignment(TextAlignment.CENTER);
            document.Add(closingMessage);

            // Close the document
            document.Close();

            // Return the PDF as a byte array
            return ms.ToArray();
        }
    }
}