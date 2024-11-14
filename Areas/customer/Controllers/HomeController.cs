using System.Diagnostics;
using System.Linq.Expressions;
using System.Security.Claims;
using Binary.Utilities;
using Microsoft.AspNetCore.Mvc;
using BinaryDecimalStore.Models;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BinaryDecimalStore.Controllers;

[Area("customer")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ExtendIdentity> _userManager;

    
    public HomeController(ILogger<HomeController> logger, UserManager<ExtendIdentity> userManager, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _userManager = userManager;

    }

    
    public IActionResult Index()
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = 
            claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null) 
        {
            HttpContext.Session.SetInt32(StaticData.SessionCART,
                getShoppingCartNumber(claim.Value));
            
        }

        if (User.IsInRole(StaticData.Role_Admin) ||
            User.IsInRole(StaticData.Role_Company) ||
            User.IsInRole(StaticData.Role_Customer) ||
            User.IsInRole(StaticData.Role_Employee)
            )
        {
            TempData["user"] = _unitOfWork.AppIdentity.get(u => u.Email == User.Identity.Name).name;
        }

        
        
        List<Product> products = _unitOfWork.Product.getAll(new Expression<Func<Product, object>>[]
        {
            p => p.Categorey,
            p => p.productImages
        }).ToList();
        
        return View(products);
    }
    public async Task<IActionResult> Details(int id)
    {
        ExtendIdentity? user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();

        string? userId = user.Id;

        if (userId == null) userId = "-1";
        
        Product product = _unitOfWork.Product.get(p => p.ID == id, new Expression<Func<Product, object>>[]
        {
            p => p.Categorey,
            p => p.productImages
        });

        ShoppingCart shoppingCar = new ShoppingCart
        {
            Product   = product,
            ProductId = product.ID,
            Quantity  = 1 ,
            AppUserId = userId,
            
            
            
        };
       
        return View(shoppingCar);
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

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var userId = 
            claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

        shoppingCart.AppUserId = userId;
        var shoppingCartFromDb = _unitOfWork.shoppingCart.get(d
            => d.AppUserId == shoppingCart.AppUserId
               && d.ProductId == shoppingCart.ProductId);
        if (shoppingCartFromDb != null)
        {
            shoppingCartFromDb.Quantity += shoppingCart.Quantity;
            _unitOfWork.shoppingCart.Update(shoppingCartFromDb);
            // if not call as no tracking it will update automatically
        }
        else
            _unitOfWork.shoppingCart.add(shoppingCart);
        
        _unitOfWork.save();
        HttpContext.Session.SetInt32(StaticData.SessionCART, getShoppingCartNumber(userId));
        
        
        
        
        TempData["success"] = "Cart Updated Successfully";
        
        return RedirectToAction(nameof(Index));
    }
    public IActionResult ChangeCurrency(string currency)
    {
        StaticData.CurrentCurrency = currency; // Update the static variable
        return RedirectToAction(nameof(Index), "Home", new { currencyChanged = true });
    }


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
   
    
    
}