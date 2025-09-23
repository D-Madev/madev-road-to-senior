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
    internal class SliderRepository : Repository<Slider>, ISliderRepository
    {
        private readonly ApplicationDbContext _context;
        public SliderRepository(ApplicationDbContext context) : base(context) => _context = context;
        public void Update(Slider slider)
        {
            var obj = _context.Sliders.FirstOrDefault(s => s.Id == slider.Id);
            if (obj == null) return;

            obj.Nombre = slider.Nombre;
            obj.estado = slider.estado;
            obj.UrlImagen = slider.UrlImagen;

            _context.SaveChanges();
        }
    }
}
