using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BinaryDecimalStore.Models.View_Models;

public class ManageRoleVM
{
    
    // public string Id { get; set; }
    // [Required]
    // public string Name { get; set; }
    // [Required]
    // public string Role { get; set; }
    //
    // public string? CompanyName { get; set; }
    // public int? companyID { get; set; }
    
    public ExtendIdentity extendUser { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> Companies { get; set; }
    [ValidateNever]
    public IEnumerable<SelectListItem> Roles { get; set; }
    
    
    
    
}