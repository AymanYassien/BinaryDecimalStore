using Binary.Utilities;
using BinaryDecimalStore.DbContext;
using BinaryDecimalStore.Models;
using Microsoft.AspNetCore.Mvc;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;

namespace BinaryDecimalStore.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticData.Role_Admin)]
public class CategoreyController : Controller
{

    private readonly IUnitOfWork _unitOfWork;

    public CategoreyController(IUnitOfWork unitOfWork)  => _unitOfWork = unitOfWork;
    public IActionResult Index()
    {
        List<Categorey> categoriesList = _unitOfWork.Category.getAll().ToList();
        return View(categoriesList);
    }

    public IActionResult addNewCategory()
    {
        return View();
    }

    [HttpPost]
    public IActionResult addNewCategory(Categorey newCategory)
    {
        if (double.TryParse(newCategory.name, out double name))
            ModelState.AddModelError("name", "The Name can not a Number "); 
        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.Category.add(newCategory); // add to set
                _unitOfWork.save(); //  save changes in set
                TempData["success"] = $"Category with Name '{newCategory.name}' Added Successfully";
            
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ViewBag.ErrorMessage = $"Display Order '{newCategory.DisplayOrder}' " +
                                       $" was assigned to another Category";
                return View(newCategory);
                //throw;
            }
            
        }

        return View(newCategory);

    }

    public IActionResult isValidDisplayOrder(int DisplayOrder, int ID = 1)
    {
        if (ID == 0) ID = 1;
        var  old = _unitOfWork.Category.get(i => i.ID == ID).DisplayOrder;
        var cat = _unitOfWork.Category.get(i => i.ID == ID);
        if (old != null)
        {
            if (old == DisplayOrder) return Json(true);
        }
       
        
        if (cat == null) return Json(true);
        return Json($"This Display order '{DisplayOrder}' is already assigned.");
    }

    public IActionResult EditCategory(int? ID) //matching name
    {

        
        var cat = _unitOfWork.Category.get(i => i.ID == ID);
        if (cat == null) return NotFound();
        
        return View(cat);
    }
    
    [HttpPost]
    public IActionResult EditCategory(Categorey newCategory)
    {
        if (double.TryParse(newCategory.name, out double name))
            ModelState.AddModelError("name", "The Name can not a Number "); 
        if (ModelState.IsValid)
        {
            try
            {
                _unitOfWork.Category.update(newCategory); 
                _unitOfWork.save(); 
                TempData["success"] = $"Category with Name '{newCategory.name}' Updated Successfully";
            
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = $"Display Order '{newCategory.DisplayOrder}' " +
                                       "was assigned to another Category";
               
                return View( newCategory);
            }
        }

        return View(newCategory);

    }
    
    public IActionResult DeleteCategory(int? ID) 
    {
        if (ID == null || ID < 1) return NotFound();

        var cat = _unitOfWork.Category.get(i => i.ID == ID);
        if (cat == null) return NotFound();
        
        return View(cat);
    }
    
    [HttpPost]
    public IActionResult DeleteCategory(Categorey newCategory) 
    {
        
        _unitOfWork.Category.remove(newCategory); 
        _unitOfWork.save(); 
        TempData["success"] = $"Category with Name '{newCategory.name}' Deleted Successfully";
            
        return RedirectToAction("Index");
    }
    
   
}