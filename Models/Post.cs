using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace friendly.Models
{
    public class Post
{
    public int PostId { get; set; }
    
    [ValidateNever]
    public string PostImagePath { get; set; }  // Store the image file path

    [Required]
    public string Caption { get; set; }

    [ValidateNever]
    public string PostDate { get; set; } 

    [Required]
    public int UserId { get; set; }

    [ValidateNever]
    public virtual User User { get; set; }
    [ValidateNever]
    public virtual List<Comment>? Comments { get; set; }
    }
}
