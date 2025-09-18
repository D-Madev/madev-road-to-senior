namespace CRUD_TEST.DTOs;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class CategoryDTO
{
  public int? Id { get; set; }
  [Required(ErrorMessage = "El nombre es obligatorio")]
  public string Name { get; set; }
}
