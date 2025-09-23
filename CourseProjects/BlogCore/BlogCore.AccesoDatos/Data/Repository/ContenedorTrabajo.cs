using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Data;

namespace BlogCore.AccesoDatos.Data.Repository
{
    public class ContenedorTrabajo : IContenedorTrabajo
    {
        private readonly ApplicationDbContext _Context;
        public ContenedorTrabajo(ApplicationDbContext context)
        {
            _Context = context;
            Categoria = new CategoriaRepository(_Context);
            Articulo = new ArticuloRepository(_Context);
            Slider = new SliderRepository(_Context);
        }

        public ICategoriaRepository Categoria { get; private set; }
        public IArticuloRepository Articulo { get; private set; }
        public ISliderRepository Slider { get; private set; }

        public void Dispose()
        {
            _Context.Dispose();
        }

        public void Save()
        {
            _Context.SaveChanges();
        }
    }
}
