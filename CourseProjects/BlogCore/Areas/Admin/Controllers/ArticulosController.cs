using System.ComponentModel;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
using BlogCore.Models;
using BlogCore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BlogCore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ArticulosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;
        private readonly IWebHostEnvironment _hostingEnviroment;
        public ArticulosController(IContenedorTrabajo contenedorTrabajo, IWebHostEnvironment hostingEnviroment)
        {
            _contenedorTrabajo = contenedorTrabajo;
            _hostingEnviroment = hostingEnviroment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            ArticuloViewModel ArtVM = new ArticuloViewModel()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };
            return View(ArtVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ArticuloViewModel artVM)
        {
            artVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();

            // Si el modelo es invalido, falla 
            if (ModelState.IsValid)
            {
                // Nuevo Articulo
                string rutaPrincipal = _hostingEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                // Si el articulo no es nuevo, falla 
                // Si no se ingresó un archivo, falla   
                if (artVM.Articulo.Id == 0 && archivos.Count() > 0)
                {
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imgs\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName);
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    artVM.Articulo.UrlImagen = @"\imgs\articulos\" + nombreArchivo + extension;
                    artVM.Articulo.FechaCreacion = DateTime.Now.ToString();

                    // Guardamos las operaciones
                    _contenedorTrabajo.Articulo.Add(artVM.Articulo);
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Imagen", "Debe seleccionar una imagen para poder crear el articulo.");
                }
            }
            return View(artVM);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ArticuloViewModel ArtVM = new ArticuloViewModel()
            {
                Articulo = new Models.Articulo(),
                ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias()
            };

            if (id != null) ArtVM.Articulo = _contenedorTrabajo.Articulo.Get(id.GetValueOrDefault());

            return View(ArtVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ArticuloViewModel artVM)
        {
            // Si el modelo es invalido, falla 
            if (ModelState.IsValid)
            {
                // Nuevo Articulo
                string rutaPrincipal = _hostingEnviroment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var articuloDesdeBd = _contenedorTrabajo.Articulo.Get(artVM.Articulo.Id);
                // Si el articulo no es nuevo, falla 
                // Si no se ingresó un archivo, falla   
                if (archivos.Count() > 0)
                {
                    // Nueva imagen para el articulo
                    string nombreArchivo = Guid.NewGuid().ToString();
                    var subidas = Path.Combine(rutaPrincipal, @"imgs\articulos");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    var rutaImagen = Path.Combine(rutaPrincipal, articuloDesdeBd.UrlImagen.TrimStart('\\'));
                    if (System.IO.File.Exists(rutaImagen)) System.IO.File.Delete(rutaImagen);

                    // Nuevamente subimos el archivo
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    artVM.Articulo.UrlImagen = @"\imgs\articulos\" + nombreArchivo + extension;

                    // Guardamos las operaciones
                    _contenedorTrabajo.Articulo.Update(artVM.Articulo);
                    _contenedorTrabajo.Save();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Cuando queremos conservar la imagen previa
                    artVM.Articulo.UrlImagen = articuloDesdeBd.UrlImagen;
                }
                
                _contenedorTrabajo.Articulo.Update(artVM.Articulo);
                _contenedorTrabajo.Save();
                return RedirectToAction(nameof(Index));
            }

            artVM.ListaCategorias = _contenedorTrabajo.Categoria.GetListaCategorias();
            return View(artVM);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            Articulo? articulo = _contenedorTrabajo.Articulo.Get(id);
            if (articulo == null) return Json(new { success = false, message = "Error borrando Articulo" });

            _contenedorTrabajo.Articulo.Remove(articulo);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Articulo borrado correctamente" });
        }

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Articulo.GetAll(includeProperties: "Categoria") });
        }
        #endregion
    }
}
