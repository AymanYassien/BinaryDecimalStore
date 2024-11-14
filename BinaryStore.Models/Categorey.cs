using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace BinaryDecimalStore.Models;

public class Categorey
{
    
     public int ID { get; set; }
    
    [Required (ErrorMessage = "The Name must Not Empty")] 
    [DisplayName("Category Name")]
    [Length(3, 30, ErrorMessage = "Category name Characters must Between 3 - 30")]
    public string name { get; set; }
    
    
    [DisplayName("Description")]
    public string Description { get; set; }
    
    
    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "Display order must between 1 - 100")]
   // [Remote(action: "isValidDisplayOrder", controller: "Categorey", AdditionalFields = "ID",
     //   ErrorMessage = "This Display order is already assigned.")] 
    // this error message will display in sql server validation only
   
    public int DisplayOrder { get; set; }
}