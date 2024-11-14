using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BinaryDecimalStore.Models;


public class Comment
{
    public int Id { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product { get; set; }
    
    [Required]
    [StringLength(1000)]
    public string UserComment { get; set; }
    
    public DateTime DateCommentAdded { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    [ForeignKey("UserId")]
    [ValidateNever]
    public virtual ExtendIdentity User { get; set; }

}
