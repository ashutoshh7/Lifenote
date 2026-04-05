using System.ComponentModel.DataAnnotations;

namespace Lifenote.Application.DTOs.Note
{
    public class CreateNoteDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Category { get; set; }

        public List<string>? Tags { get; set; } = new List<string>();

        public bool IsPinned { get; set; } = false;
    }
}
