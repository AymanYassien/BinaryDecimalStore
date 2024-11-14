using System.Linq.Expressions;
using System.Security.Claims;
using Binary.Utilities;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
using BinaryDecimalStore.Models.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace BinaryDecimalStore.Controllers;
[Area("customer")]
[Authorize]
public class CartController : Controller
{
    private readonly IOptions<stripeSettings> _stripeSettings;
    private readonly ICustomEmailSender _emailSender;

    readonly OrderConfirmationPdfGenerator _pdfGenerator;
    readonly OrderConfirmationService _pdfConfirmationService;
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty] 
    public shoppingCartVM ShoppingCartVm { get; set; }

    public CartController(IOptions<stripeSettings> stripeSettings, IUnitOfWork unitOfWork,
        ICustomEmailSender emailSender)
    {
        _unitOfWork = unitOfWork;
        _stripeSettings = stripeSettings;
        StripeConfiguration.ApiKey = _stripeSettings.Value.SecretKey;
        _emailSender = emailSender;
    }
    
    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = 
            claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVm = new shoppingCartVM
        {
            ShoppingCartList = _unitOfWork.shoppingCart.getAll(new Expression<Func<ShoppingCart, object>>[]
            {
                p => p.Product, p => p.Product.productImages 

            }).Where(s => s.AppUserId == userId).ToList()
        };
      
            
        
        return View(ShoppingCartVm);
        
    }

    public IActionResult plus(int cartId)
    {
        var cartFromDb = _unitOfWork.shoppingCart.get(s => s.ShoppingCartId == cartId);
        cartFromDb.Quantity += 1;
        
        _unitOfWork.shoppingCart.Update(cartFromDb);
        _unitOfWork.save();
        
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = 
            claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        HttpContext.Session.SetInt32(StaticData.SessionCART, getShoppingCartNumber(userId));


        
        return RedirectToAction(nameof(Index));
    }              

    public IActionResult minus(int cartId)
    {
        var cartFromDb = _unitOfWork.shoppingCart.get(s => s.ShoppingCartId == cartId);
        
        if (cartFromDb.Quantity > 1)
        {
            cartFromDb.Quantity -= 1;
            _unitOfWork.shoppingCart.Update(cartFromDb);
        }
        else
            _unitOfWork.shoppingCart.remove(cartFromDb);
        
        _unitOfWork.save();
        
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = 
            claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        HttpContext.Session.SetInt32(StaticData.SessionCART, getShoppingCartNumber(userId));

        
        return RedirectToAction(nameof(Index));
    }

    public IActionResult remove(int cartId)
    {
        var cartFromDb = _unitOfWork.shoppingCart.get(s => s.ShoppingCartId == cartId);
        
        _unitOfWork.shoppingCart.remove(cartFromDb);
        _unitOfWork.save();
        
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = 
            claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
        HttpContext.Session.SetInt32(StaticData.SessionCART, getShoppingCartNumber(userId));

        
        return RedirectToAction(nameof(Index));
    }

    public IActionResult summary()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId    = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVm = new shoppingCartVM
        {
            ShoppingCartList = _unitOfWork.shoppingCart.getAll(new Expression<Func<ShoppingCart, object>>[]
            {
                p => p.Product 

            }).Where(s => s.AppUserId == userId).ToList(),
            orderHeader = new ()
        };

        ShoppingCartVm.orderHeader.ExtendIdentity = _unitOfWork.AppIdentity.get(u => u.Id == userId);
        ShoppingCartVm.orderHeader.Name = ShoppingCartVm.orderHeader.ExtendIdentity.name; 
        ShoppingCartVm.orderHeader.Code = ShoppingCartVm.orderHeader.ExtendIdentity.Code; 
        ShoppingCartVm.orderHeader.Address = ShoppingCartVm.orderHeader.ExtendIdentity.address; 
        ShoppingCartVm.orderHeader.City = ShoppingCartVm.orderHeader.ExtendIdentity.city; 
        ShoppingCartVm.orderHeader.State = ShoppingCartVm.orderHeader.ExtendIdentity.state; 
        ShoppingCartVm.orderHeader.PhoneNumber = ShoppingCartVm.orderHeader.ExtendIdentity.PhoneNumber; 
        ShoppingCartVm.orderHeader.OrderCurrencyCode = StaticData.getCurrencyCode(); 
        
        
        
        return View(ShoppingCartVm);
    }
    
    [HttpPost]
    [ActionName("sum-mary")]
    public IActionResult summary_Post()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId    = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVm.ShoppingCartList =
            _unitOfWork.shoppingCart.getAll(new Expression<Func<ShoppingCart, object>>[]
            {
                p => p.Product
            }).Where(u => u.AppUserId == userId);
        
        ShoppingCartVm.orderHeader.OrderDate = DateTime.Now;
        ShoppingCartVm.orderHeader.ExtendUserId = userId;
        // ShoppingCartVm.orderHeader.ExtendIdentity = _unitOfWork.AppUser.get(u => u.Id == userId);
        Models.ExtendIdentity extendIdentity = _unitOfWork.AppIdentity.get(u => u.Id == userId);

        if (extendIdentity.CompanyID.GetValueOrDefault() == 0)
        {
            ShoppingCartVm.orderHeader.OrderStatus = StatusesStaticData.OrderStatusPending;
            ShoppingCartVm.orderHeader.PaymentStatus = StatusesStaticData.PaymentStatusPending;
        }
        else
        {
            ShoppingCartVm.orderHeader.OrderStatus = StatusesStaticData.OrderStatusApproved;
            ShoppingCartVm.orderHeader.PaymentStatus = StatusesStaticData.PaymentStatusApprovedForDelayPayment;
        }
        //ShoppingCartVm.orderHeader.PaymentIntentId = "Temp"; // Temp
        _unitOfWork.OrderHeader.add(ShoppingCartVm.orderHeader);
        _unitOfWork.save();

        foreach (var product in ShoppingCartVm.ShoppingCartList)
        {
            OrderDetail orderDetail = new()
            {
                orderHeaderId = ShoppingCartVm.orderHeader.Id,
                ProductId = product.ProductId,
                priceWhenOrder = product.Product.price,
                count = product.Quantity

            };
            
            _unitOfWork.OrderDetail.add(orderDetail);
            _unitOfWork.save();

        }
        
        if (extendIdentity.CompanyID.GetValueOrDefault() == 0)
        {
            // return stripePaymentProcess();
            var domain = "https://localhost:7179/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"customer/Cart/orderConfirmation?id={ShoppingCartVm.orderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment"
            };
            foreach (var item in ShoppingCartVm.ShoppingCartList)
            {
                double price = item.Product.price;
                double discount = item.Product.discountRatio;
                double Final = price - price * discount / 100;
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(Final * 100), //  =>20.50 2050 
                        Currency = StaticData.getCurrencyCode().ToLower(),
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = item.Product.Name
                        }

                    },
                    Quantity = item.Quantity
                };
                options.LineItems.Add(sessionLineItem);
            }

            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.UpdateStripePayment(ShoppingCartVm.orderHeader.Id,
                session.Id, session.PaymentIntentId);
            // session id will create even T or F, but Payment only true
            _unitOfWork.save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }
        
        
        return RedirectToAction(nameof(OrderConfirmation), new {id = ShoppingCartVm.orderHeader.Id });
        
    }
    
    [HttpPost]
    [ActionName("summary")]
    public IActionResult summaryPost() 
    {
		var claimsIdentity = (ClaimsIdentity)User.Identity;
		var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        ShoppingCartVm.ShoppingCartList =
            _unitOfWork.shoppingCart.getAll(new Expression<Func<ShoppingCart, object>>[]
            {
                p => p.Product
            }).Where(u => u.AppUserId == userId);


        ShoppingCartVm.orderHeader.OrderDate = DateTime.Now;
        ShoppingCartVm.orderHeader.ExtendUserId = userId;

        Models.ExtendIdentity extendIdentity = _unitOfWork.AppIdentity.get(u => u.Id == userId);


		
           if (extendIdentity.CompanyID.GetValueOrDefault() == 0) 
           {
			//it is a regular customer 
            ShoppingCartVm.orderHeader.OrderStatus = StatusesStaticData.OrderStatusPending;
            ShoppingCartVm.orderHeader.PaymentStatus = StatusesStaticData.PaymentStatusPending;
           }
           else 
           {
			//it is a company user
            ShoppingCartVm.orderHeader.OrderStatus = StatusesStaticData.OrderStatusApproved;
            ShoppingCartVm.orderHeader.PaymentStatus = StatusesStaticData.PaymentStatusApprovedForDelayPayment;
           }
           _unitOfWork.OrderHeader.add(ShoppingCartVm.orderHeader);
           _unitOfWork.save();
                
           foreach (var product in ShoppingCartVm.ShoppingCartList)
           {
               OrderDetail orderDetail = new()
               {
                   orderHeaderId = ShoppingCartVm.orderHeader.Id,
                   ProductId = product.ProductId,
                   priceWhenOrder = product.Product.price,
                   count = product.Quantity

               };
            
               _unitOfWork.OrderDetail.add(orderDetail);
               _unitOfWork.save();

           }
           
           if (extendIdentity.CompanyID.GetValueOrDefault() == 0)
           {
               //it is a regular customer account and we need to capture payment
               //stripe logic
                var domain = Request.Scheme+ "://"+ Request.Host.Value +"/";

                var options = new SessionCreateOptions
                {
				  SuccessUrl = domain+ $"customer/cart/OrderConfirmation?id={ShoppingCartVm.orderHeader.Id}",
                  CancelUrl = domain+"customer/cart/index",
				  LineItems = new List<SessionLineItemOptions>(),
				  Mode = "payment",
			   };
                
                foreach (var item in ShoppingCartVm.ShoppingCartList)
                {
                    double price = item.Product.price;
                    double discount = item.Product.discountRatio;
                    double Final = StaticData.getPrice((price - price * discount / 100), StaticData.CurrentCurrency) ;
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(Final * 100), //  =>20.50 2050 
                            Currency = StaticData.getCurrencyCode().ToLower(),
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Name
                            }

                        },
                        Quantity = item.Quantity
                    };
                    options.LineItems.Add(sessionLineItem);
                }

			    var service = new SessionService();
			    Session session = service.Create(options);
               _unitOfWork.OrderHeader.UpdateStripePayment(ShoppingCartVm.orderHeader.Id, session.Id, session.PaymentIntentId);
               _unitOfWork.save();
               Response.Headers.Add("Location", session.Url);
               return new StatusCodeResult(303);
               
               
               // session id will create even T or F, but Payment only true
              
              

		}

			// return RedirectToAction(nameof(OrderConfirmation),new { id=ShoppingCartVM.OrderHeader.Id });
            return RedirectToAction(nameof(OrderConfirmation), new {id = ShoppingCartVm.orderHeader.Id });
		}
    

    public async Task<IActionResult> OrderConfirmation(int id)
    {
        orderHeader order = _unitOfWork.OrderHeader.get(
        
            O => O.Id == id,
        
            new Expression<Func<orderHeader, object>>[]
            {
                o => o.ExtendIdentity   
            });

        if (order.PaymentStatus != StatusesStaticData.PaymentStatusApprovedForDelayPayment)
        {
            var service = new SessionService();
            Session session = service.Get(order.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(id,
                    StatusesStaticData.OrderStatusApproved, StatusesStaticData.PaymentStatusApproved );
                _unitOfWork.OrderHeader.UpdateStripePayment(id, session.Id, session.PaymentIntentId);
                _unitOfWork.save();
            }
            HttpContext.Session.Clear();

        }

        List<ShoppingCart> shoppingCarts =
            _unitOfWork.shoppingCart.getAll()
                .Where(u => u.AppUserId == order.ExtendUserId).ToList();
        _unitOfWork.shoppingCart.removeRange(shoppingCarts);
        _unitOfWork.save();

        orderManagVM orderVm = new orderManagVM
        {
            orderHeader = order,
            orderDetails = _unitOfWork.OrderDetail.getAll(new Expression<Func<OrderDetail, object>>[]{o => o.Product})
                .Where(o => o.orderHeaderId == order.Id).ToList()
        };
        

        sendEmail(order.ExtendIdentity.Email, order.Id,  StaticData.getPriceWithCurrency(order.OrderTotal, 
            StaticData.getCurrencyName(order.OrderCurrencyCode)), orderVm);
        
        
        return View(id);
    }
    

    private async Task sendEmail(string userMail, int order_id, string totalPrice, orderManagVM orderVm = null)
    {
        byte[] pdfBytes =  GeneratePdfAsync(orderVm);

        string html = $@"<h2>Thank you for your order!</h2>
                   <p>Order Number: {order_id}</p>
                   <p>Total Amount: {totalPrice}</p>
                   <p>We'll notify you when your order ships.</p><div>Your Invoice in pdf: </p>";
        //_pdfConfirmationService.SendOrderConfirmationAsync(userMail, "Order Confirmation", html, pdfBytes);
        
         _emailSender.SendEmailAsync(userMail, "Order Confirmation", html, attachments:  new  [] 
        { 
            new EmailAttachment(
                fileName: "order_confirmation.pdf",
                contentType: "application/pdf",
                data: pdfBytes
            )
        });
        
    }
    
    private int getShoppingCartNumber(string userid)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = 
            claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        int shoppingCounter = 0;

        IEnumerable<ShoppingCart> shoppingCart = _unitOfWork.shoppingCart.
            getAll().Where(u => u.AppUserId == userId ).ToList();
        foreach (var cart in shoppingCart)
            shoppingCounter += cart.Quantity;

        return shoppingCounter;
    }
    
    
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
            
            double finalPrice_1, price;
            string currencyCode = orderVM.orderHeader.OrderCurrencyCode;
            string currencyName = StaticData.getCurrencyName(currencyCode);
            double totalBill = 0;
            foreach (var item in orderVM.orderDetails)
            {
                price = item.Product.price;
                finalPrice_1=price - price * item.Product.discountRatio / 100;
                finalPrice_1.ToString("N1");
                double finalPrice = finalPrice_1;
                
                 
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




