namespace CRUD_TEST.DTOs;

using CRUD_TEST.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ItemDTO
{
    public int? Id { get; set; }
    [Required(ErrorMessage = "El Nombre es obligatorio")]
    public string Name { get; set; }

    [Required(ErrorMessage = "El Precio es obligatorio")]
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
}
