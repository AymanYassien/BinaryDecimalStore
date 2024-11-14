using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BinaryDecimalStore.Models;

public class orderHeader
{
    public int Id { get; set; }
    
    public string ExtendUserId { get; set; }
    [ForeignKey("ExtendUserId")]
    [ValidateNever]
    public ExtendIdentity ExtendIdentity { get; set; }
    
    public DateTime OrderDate { get; set;}
    public DateTime ShippingDate { get; set; }
    public double OrderTotal { get; set; }
    
    public string? OrderStatus {get; set; }
    public string? PaymentStatus {get; set; }
    public string? TrackingNumber {get; set; }
    public string? Carrier {get; set; }
    
    
    public DateTime PaymentDate {get; set;}
    public DateOnly PaymentDuetDate {get; set;}
    
    public string? SessionId { get; set; }
    public string? PaymentIntentId { get; set; }
    
    [Required]    
    public string Name {get; set;}
    [Required]
    public string Address {get; set;}
    [Required]
    public string? Code {get; set;}
    [Required]
    public string City {get; set;}
    [Required]
    public string State {get; set;}
    [Required]
    public string PhoneNumber {get; set;} 
    
    [Required]
    [MaxLength(4)]
    public string OrderCurrencyCode {get; set;}        
                  
}                 