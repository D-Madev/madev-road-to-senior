using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;
using BlogCore.Models;

namespace BlogCore.AccesoDatos.Data.Repository
{
    internal class ArticuloRepository : Repository<Articulo>, IArticuloRepository
    {
        private readonly ApplicationDbContext _context;
        public ArticuloRepository(ApplicationDbContext context) : base(context) => _context = context;
        public void Update(Articulo articulo)
        {
            var obj = _context.Articulos.FirstOrDefault(s => s.Id == articulo.Id);
            if (obj == null) return;

            obj.Nombre = articulo.Nombre;
            obj.Descripcion = articulo.Descripcion;
            obj.UrlImagen = articulo.UrlImagen;
            obj.CategoriaId = articulo.CategoriaId;

            _context.SaveChanges();
        }
    }
}
