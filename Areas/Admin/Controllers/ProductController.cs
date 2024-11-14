using System.Linq.Expressions;
using System.Net;
using Binary.Utilities;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
using BinaryDecimalStore.Models.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace BinaryDecimalStore.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticData.Role_Admin)]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    
    string wwwRootPath ;

    public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
         
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
        wwwRootPath = _webHostEnvironment.WebRootPath;
    }
    

    public IActionResult Index()
    {
        List<Product> products = _unitOfWork.Product
            .getAll(new Expression<Func<Product, object>>[]
                {
                    p => p.Categorey
                }
            ).ToList();
        
        
    return View(products);
    }
    
    public IActionResult Upsert(int? id) 
    {
        /*
        IEnumerable<SelectListItem> _CategoryList = _unitOfWork.Category
            .getAll().Select(c => new SelectListItem(c.name, c.ID.ToString())
                /*{
                    Text = c.name,
                    Value = c.ID.ToString()
                }/
            );
            
        ViewBag.CategoryList = _CategoryList;              // non use
        ViewData["CategoryListData"] = _CategoryList;      // non use
        */
       
        ProductViewModel ProductVM = new ProductViewModel
        {
            CategoryList = _unitOfWork.Category.getAll().Select(c => new SelectListItem
                {Text = c.name, Value = c.ID.ToString()}),
               Product = new Product()
        };
        
        if (id != 0 && id != null)
        {
            ProductVM.Product = _unitOfWork.Product.get(p => p.ID == id, new Expression<Func<Product, object>>[]
            {
              p => p.productImages  
            });
        }
        
        
        return View(ProductVM);
    }
    [HttpPost]
    public IActionResult Upsert(ProductViewModel newProduct, List<IFormFile> files)
    {
        bool isDeleted;
        if (double.TryParse(newProduct.Product.Name, out double name))
            ModelState.AddModelError("name", "The Name can not a Number ");
        if (ModelState.IsValid)
        {
            if (newProduct.Product.ID == 0)
                _unitOfWork.Product.add(newProduct.Product);
            else
                _unitOfWork.Product.update(newProduct.Product); // add to set
            _unitOfWork.save(); //  save changes in set

            if (files != null)
            {

                foreach (IFormFile file in files)
                {
                    ProductImage n = createAnImagePath(file, newProduct.Product.ID);

                    if (newProduct.Product.productImages == null)
                        newProduct.Product.productImages = new List<ProductImage>();

                    newProduct.Product.productImages.Add(n);

                }

                _unitOfWork.Product.update(newProduct.Product);
                _unitOfWork.save();


            }

            TempData["success"] = $"Category with Name '{newProduct.Product.Name}' Added / Updated Successfully";
            return RedirectToAction("Index");
        }
        // if (newProduct.Product.Imageurl != null)
            //     isDeleted = DeleteOldImage(newProduct.Product.Imageurl);
            // newProduct.Product.Imageurl = createAnImagePath(file);
            // else if (newProduct.Product.Imageurl != null)
            //     isDeleted = DeleteOldImage(newProduct.Product.Imageurl);

            newProduct.CategoryList = _unitOfWork.Category.getAll().Select(u => new SelectListItem
            {
                Text = u.name,
                Value = u.ID.ToString()
            });
        

        return View(newProduct);

    }
    
    public IActionResult DeleteProduct(int? ID) 
    {
        if (ID == null || ID < 1) return NotFound();

        var product = _unitOfWork.Product.get(i => i.ID == ID);
        if (product == null) return NotFound();
        
        return View(product);
    }
    [HttpPost]
    public IActionResult DeleteProduct(Product newProduct) 
    {
        
        _unitOfWork.Product.remove(newProduct); 
        _unitOfWork.save(); 
        TempData["success"] = $"Category with Name '{newProduct.Name}' Deleted Successfully";
            
        return RedirectToAction("Index");
    }
    
    public IActionResult DeleteImage(int imageId)
    {
        var imageToDelete = _unitOfWork.ProductImage.get(i => i.id == imageId);
        if (imageToDelete != null && !string.IsNullOrEmpty(imageToDelete.omageUrl))
            DeleteOldImage(imageToDelete.omageUrl);
        
        _unitOfWork.ProductImage.remove(imageToDelete);
        _unitOfWork.save();

        TempData["success"] = "Image Deleted Successfully";

        return RedirectToAction(nameof(Upsert), new { id = imageToDelete.productId });

    }
    private ProductImage createAnImagePath(IFormFile file, int product_id)   
    {
        
        ProductImage productImage = null;
        
        if (file != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string productPath = Path.Combine("images", "products", $"product-{product_id}");
            string finalPath = Path.Combine(wwwRootPath, productPath);
        
            // Create directory if it doesn't exist
            if (!Directory.Exists(finalPath))
            {
                Directory.CreateDirectory(finalPath);
            }
        
            // Combine the final path with the filename for the actual file location
            string filePath = Path.Combine(finalPath, fileName);
        
            // Save the file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            productImage = new ProductImage
            {
                omageUrl = $"/{productPath}/{fileName}".Replace("\\", "/"),  // Fixed typo and ensure forward slashes
                productId = product_id
            };
        }
        return productImage;
    }
    private bool DeleteOldImage(string pathToDelete)
    {
        if (pathToDelete != null)
        {
            var oldImage = Path.Combine(_webHostEnvironment.WebRootPath,
                pathToDelete.Replace("//", "/"));
            if (System.IO.File.Exists(oldImage))
            {
                try
                {
                    System.IO.File.Delete(oldImage);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
                
            }

            return true;
        }

        return false;
    }

    private bool DeleteFullDirectory(int ProductId)
    {
       
       
        string productPath = Path.Combine("images", "products", $"product-{ProductId}");
        string finalPath = Path.Combine(wwwRootPath, productPath);

        if (Directory.Exists(finalPath))
        {
            string[] filesPaths = Directory.GetFiles(finalPath);
            foreach (var filePath in filesPaths)
                System.IO.File.Delete(filePath);
            
            Directory.Delete(finalPath);
            return true;
        }

        return false;
    }
    

    #region ApiForDataTable

    [HttpGet]
    public IActionResult getAll()
    {
        var Products = _unitOfWork.Product.getAll(new Expression<Func<Product, object>>[] 
        {
            p => p.Categorey
            
        }).Select(p => new {
                p.ID,
                p.Name,
                Categorey = new { p.Categorey.name },
                p.price,
                p.discountRatio
            });
        
        return Json(new {data = Products});
    }
    
 
    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var ProductToDelete = _unitOfWork.Product.get(p => p.ID == id);
        if (ProductToDelete == null)
        {
            return Json(new { success = false, message = "Error While Deleting" });
        }

        DeleteFullDirectory(id);

        try
        {
            _unitOfWork.Product.remove(ProductToDelete);
            _unitOfWork.save();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return Json(new { success = true, message = $"Delete Product '{ProductToDelete.Name}' Successfully" });

    }

    #endregion
    
    
}