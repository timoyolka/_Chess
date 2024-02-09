using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Chess.Models
{
    public class Player
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Email { get; set; }
        public bool AtGame { get; set; }
        public bool? isWhite { get; set; }
        public int? gameId { get; set; }
        public string? UserConnectionId {  get; set; }
    }
}
