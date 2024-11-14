using System.Linq.Expressions;
using BinaryDecimalStore.BinaryStore.DataAccess.Repository.IRepository;
using BinaryDecimalStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BinaryDecimalStore.Controllers;


[Authorize]
[Area("customer")]

public class CommentsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ExtendIdentity> _userManager;

    public CommentsController(IUnitOfWork unitOfWork, UserManager<ExtendIdentity> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetProductComments(int productId)
    {
        try
        {
            var comments = _unitOfWork.Comment.getAll(new Expression<Func<Comment, object>>[]
                { c => c.User }).Where(c => c.ProductId == productId).ToList();
            var commentDtos = comments.Select(c => new
            {
                id = c.Id ,
                userComment = c.UserComment,
                dateCommentAdded = c.DateCommentAdded,
                userName = c.User?.name,
                userId = c.UserId
            });
            
            
            return Json(commentDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while retrieving comments.");
        }
    }
    
    
    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] Comment commentDto)
    {
        // commentDto.UserId = User.Identity.
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized("User not found.");

            var comment = new Comment
            {
                ProductId = commentDto.ProductId,
                UserComment = commentDto.UserComment,
                UserId = user.Id,
                DateCommentAdded = DateTime.UtcNow
            };

             _unitOfWork.Comment.add(comment);
             _unitOfWork.save();

            return Json(new
            {
                id = comment.Id,
                userComment = comment.UserComment,
                dateCommentAdded = comment.DateCommentAdded,
                userName = user.name,
                userId = user.Id
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while adding the comment.");
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateComment(int id, [FromBody] string newComment)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var comment =  _unitOfWork.Comment.get(c => c.Id == id);
            if (comment == null)
                return NotFound();

            if (comment.UserId != user.Id)
                return Forbid();

            comment.UserComment = newComment;
            _unitOfWork.Comment.Update(comment);
            _unitOfWork.save();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while updating the comment.");
        }
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var comment =  _unitOfWork.Comment.get(c => c.Id == id);
            if (comment == null)
                return NotFound();

            if (comment.UserId != user.Id)
                return Forbid();

            _unitOfWork.Comment.remove(comment);
            _unitOfWork.save();

            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while deleting the comment.");
        }
    }
}
