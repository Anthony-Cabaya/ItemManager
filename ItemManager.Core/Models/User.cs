using System.ComponentModel.DataAnnotations;

namespace ItemManager.Core.Models
{
    public class User
    {
        public int UserID { get; set; }

        [Required, StringLength(200)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Password { get; set; } = string.Empty;

        [Required, StringLength(200)]
        public string Role { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } 
    }
}
