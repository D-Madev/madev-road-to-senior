using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogCore.Models.ViewModels
{
    public class ArticuloViewModel
    {
        public required Articulo Articulo { get; set; }

        public IEnumerable<SelectListItem> ListaCategorias { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
