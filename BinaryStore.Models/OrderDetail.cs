using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
// using Microsoft.Build.Framework;

namespace BinaryDecimalStore.Models;

public class OrderDetail
{
    public int Id { get; set; }
    [Required]
    public int orderHeaderId { get; set;}
    [ValidateNever]
    [ForeignKey("orderHeaderId")]
    public orderHeader OrderHeader { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product { get; set; }
    
    public int count { get; set; }
    public double priceWhenOrder { set; get; } // log if product Price Update
    
    
}