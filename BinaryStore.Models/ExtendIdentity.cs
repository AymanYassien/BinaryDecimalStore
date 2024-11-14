using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;


namespace BinaryDecimalStore.Models;

public class ExtendIdentity : IdentityUser
{
    [Required]
    public string name { get; set; }

    public string? address { get; set; }
    public string? city { get; set; }
    public string? state { get; set; }
    public string? Code { get; set; }
    
    public int? CompanyID { get; set; }
    [ForeignKey("CompanyID")]
    [ValidateNever]
    public Company Company { get; set;  }
    
    
    [NotMapped]
    public string Role { get; set; }
    
    
}