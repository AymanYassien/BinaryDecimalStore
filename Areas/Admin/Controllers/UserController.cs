using System.Linq.Expressions;
using System.Net;
using Binary.Utilities;
using BinaryDecimalStore.BinaryStore.DataAccess.DbContext;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
using BinaryDecimalStore.Models.View_Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;

namespace BinaryDecimalStore.Controllers;

[Area("Admin")]
[Authorize(Roles = StaticData.Role_Admin)]
public class UserController : Controller
{
   
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly BinaryStoreDbContext _db;
    
    private readonly UserManager<ExtendIdentity> _userManager;
    
    public UserController(IUnitOfWork unitOfWork, BinaryStoreDbContext db, RoleManager<IdentityRole> roleManager, UserManager<ExtendIdentity> userManager)
    {
        _unitOfWork = unitOfWork;
        _roleManager = roleManager;
        _userManager = userManager;

        _db = db;
    }
    
    public IActionResult index()
    {
         return View();
    }

    [HttpGet]
    public IActionResult manageRole(string id)
    {
        var user = _unitOfWork.AppIdentity.get(u => u.Id == id);
        var userRole = _db.UserRoles.FirstOrDefault(r => r.UserId == user.Id);
        
        var role =  _db.Roles.FirstOrDefault(r => r.Id == userRole.RoleId).Name;
        var roleByUserManager =  _userManager.GetRolesAsync(user).GetAwaiter().GetResult()
            .FirstOrDefault();

        user.Role = role;
        
        
        ManageRoleVM UserVM = new ManageRoleVM
        {
            extendUser = user,
            Companies = _unitOfWork.Company.getAll().Select(c => new SelectListItem
                {Text = c.Name, Value =  c.ID.ToString(), Selected = user.CompanyID == c.ID}
            ),
            
            Roles = _roleManager.Roles.Select(r => new SelectListItem
                {Text = r.Name, Value =  r.Name, Selected = user.Role == r.Name })
        };
        return View(UserVM);
    }
    
    [HttpPost]
    public IActionResult manageRole(ManageRoleVM UserVM)
    {
        var userRole = _db.UserRoles.FirstOrDefault(r => r.UserId == UserVM.extendUser.Id).RoleId;
        var oldRole = _db.Roles.FirstOrDefault(r => r.Id == userRole).Name;

        if (!(UserVM.extendUser.Role == oldRole))
        {

            var user = _db.AppUsers.FirstOrDefault(u => u.Id == UserVM.extendUser.Id);
            if (UserVM.extendUser.Role == StaticData.Role_Company)
            {
                user.CompanyID = UserVM.extendUser.CompanyID;
            }

            if (oldRole == StaticData.Role_Company)
            {
                user.CompanyID = null;
            }
            
            _db.SaveChanges();
            _userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
            _userManager.AddToRoleAsync(user, UserVM.extendUser.Role).GetAwaiter().GetResult();
            
            
            TempData["success"] = "User Role was updated Successfully";
        }
        
        
        return Redirect(nameof(index));
    }
    
    // data // controller // display
    
    #region ApiForDataTable

    [HttpGet]
    public IActionResult getAll()
    {
        var users = _unitOfWork.AppIdentity.getAll();
        foreach (ExtendIdentity user in users)
            if (user.CompanyID != null)
                user.Company = _db.Companies.FirstOrDefault(c => c.ID == user.CompanyID);
        

        var userRoles =  _db.UserRoles.ToList();
        var roles            = _db.Roles.ToList();
        
        
        foreach (var user in users)
        {
            var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
            var role = roles.FirstOrDefault(r => r.Id == roleId).Name;

            user.Role = role;
            if (user.Company == null)
            {
                user.Company = new Company { Name = " -- " };

            }
        }
        
        return Json(new {data = users});
    }
    
    [HttpPost]
    public IActionResult lockUnlock([FromBody] string id)
    {
        var user = _db.AppUsers.FirstOrDefault(u => u.Id == id);
        if (user == null )
        {
            return Json(new { success = false, message = "Error while Locking/Unlocking" });
        }

        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
        {
            user.LockoutEnd = DateTime.Now;
        }
        else
        {
            user.LockoutEnd = DateTime.Now.AddDays(222);
        }
        _unitOfWork.AppIdentity.update(user);

        _db.SaveChanges();
        
        return Json(new { success = true, message = "Locking/Unlocking Successful" });
    }
    
    #endregion
    
    
}