using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace friendly.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; } //hvert like har en id

        
        [ValidateNever]
        public string? UserId { get; set; } //kobler en like til en bruker, personen som liker bildet

        
        [ValidateNever]
        public virtual User? User { get; set; } //henter user objektet for personen som liker bildet

        
        [ValidateNever]
        public int? PostId { get; set; } //kobler like til et objekt

        // Navigation property to Post
        [ValidateNever]
        public virtual Post? Post { get; set; } 
    }
}