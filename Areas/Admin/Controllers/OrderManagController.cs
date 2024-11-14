using System.Linq.Expressions;
using System.Security.Claims;
using Binary.Utilities;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
using BinaryDecimalStore.Models.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BinaryDecimalStore.Controllers;
[Area("admin")]
[Authorize(Roles = StaticData.Role_Admin +"," + StaticData.Role_Employee)]
public class OrderManagController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    [BindProperty]
    public orderManagVM orderVM { get; set;}

    public OrderManagController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // [ValidateAntiForgeryToken]
    public IActionResult index()
    {
        return View();
    }

    public IActionResult Details(int id)
    {
        
         orderVM = new()
        {
            orderHeader = _unitOfWork.OrderHeader.get( o => o.Id == id, 
                new Expression<Func<orderHeader, object>>[]
                {
                    o => o.ExtendIdentity
                }
            ),
            orderDetails = _unitOfWork.OrderDetail.getAll(
                new Expression<Func<OrderDetail, object>>[]
                {
                    o => o.Product
                }).Where(o => o.orderHeaderId == id).ToList()
        };
        return View(orderVM);

    }

    [HttpPost]
    [Authorize(Roles = StaticData.Role_Admin +"," + StaticData.Role_Employee)]
    public IActionResult updateOrderDetail()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.get(o => o.Id == orderVM.orderHeader.Id);

        orderHeaderFromDb.Name = orderVM.orderHeader.Name;
        orderHeaderFromDb.PhoneNumber = orderVM.orderHeader.PhoneNumber;
        orderHeaderFromDb.Address = orderVM.orderHeader.Address;
        orderHeaderFromDb.City = orderVM.orderHeader.City;
        orderHeaderFromDb.State = orderVM.orderHeader.State;
        orderHeaderFromDb.Code = orderVM.orderHeader.Code;
        if (!String.IsNullOrEmpty(orderVM.orderHeader.Carrier))
        {
            orderHeaderFromDb.Carrier = orderVM.orderHeader.TrackingNumber;
        }
        
        if (!String.IsNullOrEmpty(orderVM.orderHeader.TrackingNumber))
        {
            orderHeaderFromDb.Carrier = orderVM.orderHeader.TrackingNumber;
        }
        
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.save();

        TempData["success"] = "Order Details Updated Successfully.";

        return RedirectToAction(nameof(Details), new { id = orderHeaderFromDb.Id });


    }

    [HttpPost]
    [Authorize(Roles = StaticData.Role_Admin +"," + StaticData.Role_Employee)]
    public IActionResult startProcessing()
    {
        _unitOfWork.OrderHeader.UpdateOrderStatus(orderVM.orderHeader.Id, StatusesStaticData.OrderStatusProcessing);
        _unitOfWork.save();
        TempData["success"] = "Order Details Updated Successfully.";

        
         return RedirectToAction(nameof(Details), new { id = orderVM.orderHeader.Id });
        
    }

    [HttpPost]
    [Authorize(Roles = StaticData.Role_Admin + "," + StaticData.Role_Employee)]
    public IActionResult cancelOrder()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.get(o => o.Id == orderVM.orderHeader.Id);

        if (orderHeaderFromDb.PaymentStatus == StatusesStaticData.PaymentStatusApproved)
        {
            var options = new RefundCreateOptions
            {
                Reason = RefundReasons.RequestedByCustomer,
                PaymentIntent = orderHeaderFromDb.PaymentIntentId
            };

            var service = new RefundService();
            Refund refund = service.Create(options);
            
            _unitOfWork.OrderHeader.UpdateOrderStatus(orderHeaderFromDb.Id, StatusesStaticData.OrderStatusCanceled, StatusesStaticData.OrderStatusRefund);

        }
        else
        {
            _unitOfWork.OrderHeader.UpdateOrderStatus(orderHeaderFromDb.Id, StatusesStaticData.OrderStatusCanceled, StatusesStaticData.OrderStatusCanceled);
        }
        
        
        _unitOfWork.save();
        TempData["success"] = "Order Canceled Successfully.";

        
        return RedirectToAction(nameof(Details), new { id = orderVM.orderHeader.Id });

    }
    
    [HttpPost]
    [Authorize(Roles = StaticData.Role_Admin +"," + StaticData.Role_Employee)]
    public IActionResult shipOrder()
    {
        var orderHeaderFromDb = _unitOfWork.OrderHeader.get(o => o.Id == orderVM.orderHeader.Id);

        orderHeaderFromDb.TrackingNumber = orderVM.orderHeader.TrackingNumber;
        orderHeaderFromDb.Carrier = orderVM.orderHeader.Carrier;
        orderHeaderFromDb.OrderStatus = StatusesStaticData.OrderStatusShipped ;
        orderHeaderFromDb.ShippingDate = DateTime.Today ;
        if (orderHeaderFromDb.PaymentStatus == StatusesStaticData.PaymentStatusApprovedForDelayPayment)
            orderHeaderFromDb.PaymentDuetDate = DateOnly.FromDateTime(DateTime.Today.AddDays(30));
        
        _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
        _unitOfWork.save();
        TempData["success"] = "Order Shipped Successfully.";

        
         return RedirectToAction(nameof(Details), new { id = orderVM.orderHeader.Id });
        
    }

    [HttpPost]
    [ActionName("Details")]
    public IActionResult Details_PayNow()
    {
        orderVM.orderHeader = _unitOfWork.OrderHeader.get(o => o.Id == orderVM.orderHeader.Id,
            new Expression<Func<orderHeader, object>>[]
            {
                o => o.ExtendIdentity
            });
        
        orderVM.orderDetails = _unitOfWork.OrderDetail.getAll(
            new Expression<Func<OrderDetail, object>>[]
            {
                o => o.Product
            }).Where(o => o.orderHeaderId  == orderVM.orderHeader.Id).ToList();
        
        var domain = Request.Scheme+ "://"+ Request.Host.Value +"/";

        var options = new SessionCreateOptions
        {
            SuccessUrl = domain+ $"admin/OrderManag/PaymentConfirmation?id={orderVM.orderHeader.Id}",
            CancelUrl = domain+ $"admin/OrderManag/Details?id={orderVM.orderHeader.Id}",
            LineItems = new List<SessionLineItemOptions>(),
            Mode = "payment",
        };
                
        foreach (var item in orderVM.orderDetails)
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
                Quantity = item.count
            };
            options.LineItems.Add(sessionLineItem);
        }

        var service = new SessionService();
        Session session = service.Create(options);
        _unitOfWork.OrderHeader.UpdateStripePayment(orderVM.orderHeader.Id, session.Id, session.PaymentIntentId);
        _unitOfWork.save();
        Response.Headers.Add("Location", session.Url);
        return new StatusCodeResult(303);
        

    
    }
    
    
    public IActionResult PaymentConfirmation(int id)
    {
        orderHeader order = _unitOfWork.OrderHeader.get(
        
            O => O.Id == id
            );

        if (order.PaymentStatus == StatusesStaticData.PaymentStatusApprovedForDelayPayment)
        {
            var service = new SessionService();
            Session session = service.Get(order.SessionId);
            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateOrderStatus(id, order.OrderStatus, StatusesStaticData.PaymentStatusApproved );
                _unitOfWork.OrderHeader.UpdateStripePayment(id, session.Id, session.PaymentIntentId);
                _unitOfWork.save();
            }

        }

       
        return View(id);
    }
    
    
    
    #region api

    [HttpGet]
    public IActionResult getAll(string status)
    {
        IEnumerable<orderHeader> ordersHeader;
       
        //  get orders based on role 
        if (User.IsInRole(StaticData.Role_Admin) || User.IsInRole(StaticData.Role_Employee))
        {
            ordersHeader = _unitOfWork.OrderHeader.getAll(new Expression<Func<orderHeader, object>>[]
                {
                    o => o.ExtendIdentity
                }
            ).ToList();
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            string userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            ordersHeader = _unitOfWork.OrderHeader.getAll(new Expression<Func<orderHeader, object>>[]
            {
                o => o.ExtendIdentity
            }).Where(o => o.ExtendUserId == userId);

        }
        
        //  get Currency
        foreach (var order in ordersHeader)
        {
            order.OrderCurrencyCode = StaticData.getPriceWithCurrency(order.OrderTotal,
                StaticData.getCurrencyName(order.OrderCurrencyCode));

        }

        //  Filter based on statue 
        switch (status)
        {
            case "pending":
                ordersHeader = ordersHeader.Where(o => o.PaymentStatus == StatusesStaticData.PaymentStatusApprovedForDelayPayment ||
                                                       o.OrderStatus == StatusesStaticData.OrderStatusPending);
                break;
            case "inprocess":
                ordersHeader = ordersHeader.Where(o => o.PaymentStatus == StatusesStaticData.OrderStatusProcessing);
                break;
            case "completed": 
                ordersHeader = ordersHeader.Where(o => o.OrderStatus == StatusesStaticData.OrderStatusShipped 
                                                        );
                break;
            case "approved":
                ordersHeader = ordersHeader.Where(o => o.PaymentStatus == StatusesStaticData.OrderStatusApproved ||
                                                       o.PaymentStatus == StatusesStaticData.PaymentStatusApproved);
                break;
        }
        

        return Json(new { data = ordersHeader });
    }
    
    
    #endregion
}