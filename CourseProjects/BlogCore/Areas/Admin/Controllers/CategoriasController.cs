using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriasController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo; 
        public CategoriasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Categoria newCategoria)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            _contenedorTrabajo.Categoria.Add(newCategoria);
            _contenedorTrabajo.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Categoria? categoria = _contenedorTrabajo.Categoria.Get(id);
            // validamos que se pase una categoria
            if (categoria == null) return RedirectToAction(nameof(Index));
            return View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Categoria newCategoria)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            _contenedorTrabajo.Categoria.Update(newCategoria);
            _contenedorTrabajo.Save();
            return RedirectToAction(nameof(Index));
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Categoria? categoria = _contenedorTrabajo.Categoria.Get(id);
            if (categoria == null) return Json(new { success = false, message = "Error borrando Categoría"});

            _contenedorTrabajo.Categoria.Remove(categoria);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Categoría borrada correctamente" });
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Categoria.GetAll() });
        }
        #endregion
    }
}
