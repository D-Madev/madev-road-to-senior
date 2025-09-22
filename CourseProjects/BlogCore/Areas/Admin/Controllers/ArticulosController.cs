using System.ComponentModel;
using BlogCore.AccesoDatos.Data.Repository.IRepository;
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

        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Articulo.GetAll(includeProperties: "Categoria") });
        }
        #endregion
    }
}
