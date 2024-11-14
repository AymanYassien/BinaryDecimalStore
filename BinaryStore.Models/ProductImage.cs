using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BinaryDecimalStore.Models;

public class ProductImage 
{
    public int id { get; set; }
    [Required]
    public string omageUrl { get; set; }
    
    public int productId { get; set; }
    [ForeignKey("productId")] 
    public Product Product { get; set; }
}