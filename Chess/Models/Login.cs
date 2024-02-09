using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class Login
    {
        [Key]
        public string? Name { get; set; }
        public string? Password { get; set; }
    }
}
