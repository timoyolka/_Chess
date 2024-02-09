using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chess.Models
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }
        [Required]
        public string? Board { get; set; }
        public int? White { get; set; }
        public string? WhiteConnectionId {  get; set; }
        public int? Black { get; set; }
        public string? BlackConnectionId { get; set; }
        public int? CurrentPlayer { get; set; }
        [Required]
        public bool IsPlaying { get; set; }
    }
}
