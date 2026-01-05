namespace NotesAPI.Models;

using System.ComponentModel.DataAnnotations;
public class Note
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo título es obligatorio, revise si está completo.")]
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}