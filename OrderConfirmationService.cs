using Binary.Utilities;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models.View_Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BinaryDecimalStore;



public class OrderConfirmationService
{
    private readonly ICustomEmailSender _emailSender;
    private readonly IUnitOfWork _unitOfWork;

    public OrderConfirmationService(ICustomEmailSender emailSender, IUnitOfWork unitOfWork)
    {
        _emailSender = emailSender;
        _unitOfWork = unitOfWork;
    }

    public async Task SendOrderConfirmationAsync(string email, string subject, string htmlMessage,  byte[] pdfBytes)
    {
        // Generate the PDF
        //byte[] pdfBytes = await OrderConfirmationPdfGenerator.GeneratePdfAsync(orderDetails);
        
        // string email = _unitOfWork.AppIdentity.get(u => u.Id == orderDetails.orderHeader.ExtendUserId).Email;
        
        await _emailSender.SendEmailAsync(email, subject, htmlMessage, attachments:  new  [] 
            { 
                new EmailAttachment(
                    fileName: "order_confirmation.pdf",
                    contentType: "application/pdf",
                    data: pdfBytes
                )
            });

        
    }
}