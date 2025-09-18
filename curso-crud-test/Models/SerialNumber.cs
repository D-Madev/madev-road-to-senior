namespace CRUD_TEST.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SerialNumber
{
    [Key]
    [DisplayName("N°")]
    public int Id { get; set; }
    [Required]
    [DisplayName("Nombre")]
    public string Name { get; set; }
    [Required]
    [DisplayName("Descripcion")]
    public string Description { get; set; }

    [ForeignKey("Item")]
    public int? ItemId { get; set; }
    public Item? Item { get; set; }

    // public SerialNumber(int id, string name, string description)
    // {
    //     Id = id;
    //     Name = name;
    //     Description = description;
    // }
}
