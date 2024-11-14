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
public class CompanyController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHostEnvironment;
    }
    

    public IActionResult companies()
    {
        List<Company> companies = _unitOfWork.Company
            .getAll().ToList();
        
        
    return View(companies);
    }
    
    
    public IActionResult Upsert(int? id) 
    {
        Company company = new Company();
        if (id != 0 && id != null)
            company = _unitOfWork.Company.get(c => c.ID == id);
        
        return View(company);
    }
    
    [HttpPost]
    public IActionResult Upsert(Company company, IFormFile? file)
    {
        bool isDeleted;
        if (double.TryParse(company.Name, out double name))
            ModelState.AddModelError("name", "The Name can not a Number "); 
        if (ModelState.IsValid )
        {
            if (file != null )
            {
                if (company.Imageurl != null)
                    isDeleted = DeleteOldImage(company.Imageurl);
                company.Imageurl = createAnImagePath(file);
            }
            else if (company.Imageurl != null)
                isDeleted = DeleteOldImage(company.Imageurl);
            
            try
            {
                if (company.ID == 0)
                {
                    _unitOfWork.Company.add(company); 
                }
                else
                {   
                    _unitOfWork.Company.update(company); 
                }
                
                _unitOfWork.save(); //  save changes in set
                TempData["success"] = $"Category with Name '{company.Name}' Added Successfully";
            
                return RedirectToAction("companies");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ViewBag.ErrorMessage = e.Message;
                return View(company);
                //throw;
            }
            
        }

        return View(company);

    }
    
    public IActionResult DeleteCompany(int? ID) 
    {
        if (ID == null || ID < 1) return NotFound();

        var company = _unitOfWork.Company.get(i => i.ID == ID);
        if (company == null) return NotFound();
        
        return View(company);
    }
    
    [HttpPost]
    public IActionResult DeleteCompany(Company company) 
    {
        
        _unitOfWork.Company.remove(company); 
        _unitOfWork.save(); 
        TempData["success"] = $"Category with Name '{company.Name}' Deleted Successfully";
            
        return RedirectToAction("companies");
    }
    
    private string createAnImagePath(IFormFile File)
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (File != null)
        {
            string FileName = Guid.NewGuid().ToString();
            
            string ProductPath = Path.Combine(wwwRootPath, "images", "ProductsImages");

            //string returnedURL = Path.Combine("images", "ProductsImages", FileName +  Path.GetExtension(File.FileName));
            
            if (!Directory.Exists(ProductPath))
            {
                Directory.CreateDirectory(ProductPath);
            }

            // Add file extension to the filename
            string extension = Path.GetExtension(File.FileName);
            FileName = FileName + extension;

            // Combine path correctly for file creation
            string filePath = Path.Combine(ProductPath, FileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                File.CopyTo(fileStream);
            }

            // Return URL-style path with forward slashes for web use
            //return returnedURL;
            return "/images/ProductsImages/" + FileName;
        }
        return string.Empty;
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


    #region ApiForDataTable

    [HttpGet]
    public IActionResult getAll()
    {
        var Companies = _unitOfWork.Company.getAll();
        
        return Json(new {data = Companies});
    }
    
 
    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var CompsnyToDelete = _unitOfWork.Company.get(p => p.ID == id);
        if (CompsnyToDelete == null)
        {
            return Json(new { success = false, message = "Error While Deleting" });
        }

        DeleteOldImage(CompsnyToDelete.Imageurl);

        try
        {
            _unitOfWork.Company.remove(CompsnyToDelete);
            _unitOfWork.save();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        return Json(new { success = true, message = $"Delete Company '{CompsnyToDelete.Name}' Successfully" });

    }

    #endregion
    
    
}