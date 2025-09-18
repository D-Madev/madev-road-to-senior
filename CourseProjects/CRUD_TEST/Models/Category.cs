namespace CRUD_TEST.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class Category
{
  [DisplayName("N°")]
  public int Id { get; set; }
  [Required(ErrorMessage = "El nombre es obligatorio")]
  [DisplayName("Nombre")]
  public string Name { get; set; }

  // * <-> 1
  public ICollection<Item>? Items { get; set; }

  // public Category(int id, string name)
  // {
  //   Id = id;
  //   Name = name;
  // }
}
