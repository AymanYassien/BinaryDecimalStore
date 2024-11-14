using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BinaryDecimalStore.Models;

public class Product
{
    [Key]
    public int ID { get; set;}
    
    [Required]
    public string Name { get; set;}
    
    [Required]
    public string Description { get; set;}
    
    //[Required]
    [Range(1, 100000)]
    public double price { get; set;}

    //[Required] // not work else this is nullable (double? ), cause default double is zero
    [Range(0, 100)]
    [Display(Name = "Discount Ratio")]
    public double discountRatio { get; set;}
    
    [Display(Name = "Category Name")]
    public int CategoryID { get; set; }
    [ForeignKey("CategoryID")]    // some say this is not necessary
    [ValidateNever]
    public Categorey Categorey { get; set;}  // Navigation Prop
    //[ValidateNever]
    //public string Imageurl { get; set; } 


    [ValidateNever]
    public List<ProductImage> productImages { get; set;  }
    



}