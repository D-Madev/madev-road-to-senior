using System.ComponentModel.DataAnnotations;

namespace NotesAPI.Models;

public class Note
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo titulo es obligatorio, revise tenerlo completo.")]
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}