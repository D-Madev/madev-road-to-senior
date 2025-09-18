namespace CRUD_TEST.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ItemClient
{
  [Key]
  [Required(ErrorMessage = "El identificador es obligatorio")]
  [DisplayName("N°")]
  public int Id { get; set; }

  [Required(ErrorMessage = "El Item es obligatorio")]
  [DisplayName("Item")]
  public int ItemId { get; set; }
  [ForeignKey("ItemId")]
  public Item Item { get; set; }

  [Required(ErrorMessage = "El Cliente es obligatorio")]
  [DisplayName("Cliente")]
  public int ClientId { get; set; }
  [ForeignKey("ClientId")]
  public Client Client { get; set; }
}