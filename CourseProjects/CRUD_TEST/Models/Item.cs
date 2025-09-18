namespace CRUD_TEST.Models;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Item
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "El Identificador es obligatorio")]
    public string Name { get; set; }
    [Required(ErrorMessage = "El Nombre es obligatorio")]
    public double Price { get; set; }

    // 1 <-> 1
    [Required(ErrorMessage = "El Numero de serie es obligatorio")]
    [ForeignKey("SerialNumber")]
    public int SerialNumberId { get; set; }
    public SerialNumber SerialNumber { get; set; }

    // 1 <-> *
    [Required(ErrorMessage = "La categoria es obligatoria")]
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    // * <-> *
    public ICollection<ItemClient>? ItemClients { get; set; }

    // public Item(int id, string name, float price, SerialNumber serialNumber, Category category)
    // {
    //     Id = id;
    //     Name = name;
    //     Price = price;
    //     SerialNumber = serialNumber;
    //     Category = category;
    // }
}
