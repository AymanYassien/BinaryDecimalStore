using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BinaryDecimalStore.Models;

public class Company
{
    public int ID { get; set; }
    
    [Required]
    public string? Name {get; set; }
    public string? State {get; set; }
    public string? City {get; set; }
    public string? Address {get; set; }
    public string? Code {get; set; }
    public string? PhoneNumber {get; set; }
    
    [ValidateNever]
    public string Imageurl { get; set; } 
    
    
    
}                  