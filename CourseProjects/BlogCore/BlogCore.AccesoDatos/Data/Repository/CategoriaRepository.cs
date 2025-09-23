using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BlogCore.AccesoDatos.Data.Repository
{
    internal class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoriaRepository(ApplicationDbContext context) : base(context) => _context = context;

        public IEnumerable<SelectListItem> GetListaCategorias()
        {
            return _context.Categorias.Select(i => new SelectListItem()
            {
                Text = i.Nombre,
                Value = i.Id.ToString()
            });
        }

        public void Update(Categoria categoria)
        {
            var obj = _context.Categorias.FirstOrDefault(s => s.Id == categoria.Id);
            if (obj == null) return;

            obj.Nombre = categoria.Nombre;
            obj.Orden = categoria.Orden;

            _context.SaveChanges();
        }
    }
}
