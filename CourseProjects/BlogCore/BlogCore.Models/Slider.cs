using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogCore.Models
{
    public class Slider
    {
        [Key]
        public int Id { get; set; }

        [NotNull]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [NotNull]
        public bool estado { get; set; } = false;

        [NotNull]
        [DataType(DataType.ImageUrl)]
        [Required(ErrorMessage = "La imagen es obligatoria.")]
        public string UrlImagen { get; set; } = string.Empty;
    }
}
