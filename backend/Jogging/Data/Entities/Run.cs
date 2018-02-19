using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Jogging.Data.Entities
{
    public class Run
    {
        [Key]
        public string Id { get; set; }

        public string UserId { get; set; }

        public int Amount { get; set; }

        [Required]
        public DateTime? Date { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Distance must be positive value")]
        public int? Distance { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be positive value")]
        public int? Duration { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}