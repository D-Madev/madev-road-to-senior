namespace CRUD_TEST.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Client
{
  [Key]
  [DisplayName("N°")]
  public int Id { get; set; }
  [DisplayName("Nombre")]
  public string Name { get; set; }

  // * <-> *
  public ICollection<ItemClient>? ItemClients { get; set; }

  public Client(int id, string name)
  {
    Id = id;
    Name = name;
  }
}