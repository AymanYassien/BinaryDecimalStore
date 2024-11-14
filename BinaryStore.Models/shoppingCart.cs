using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BinaryDecimalStore.Models;

public class ShoppingCart
{
    public int ShoppingCartId { get; set; }
    
    
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product { get; set; }
    
    [Range(1, 100, ErrorMessage = "Enter a Value Between 1 and 100")]
    public int Quantity { get; set; }
    
    
    public string AppUserId { get; set; }
    
    [ForeignKey("AppUserId")]
    [ValidateNever]
    public ExtendIdentity ExtendIdentity { get; set; }
    
    
}