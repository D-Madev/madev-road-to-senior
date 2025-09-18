namespace CRUD_TEST.Controllers;

using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using CRUD_TEST.DTOs;
using CRUD_TEST.Models;
using CRUD_TEST.Data;

public class CategoryController : Controller
{
    private readonly ILogger<CategoryController> _logger;
    private readonly AppDbContext _context;

    public CategoryController(ILogger<CategoryController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        try
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al tratar de listar las categorias.");
            return BadRequest($"Se produjo un error al tratar de listar las categorias. \n\n{ex}");
        }
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Crear(CategoryDTO catDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Crear: ModelState inválido.");
            return View(catDTO);
        }

        var normalizedName = catDTO.Name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            ModelState.AddModelError(nameof(catDTO.Name), "El nombre es obligatorio.");
            return View(catDTO);
        }

        try
        {
            bool exist = await _context.Categories
                                        .AsNoTracking()
                                        .AnyAsync(c => c.Name.ToLower() == normalizedName.ToLower());

            if (exist)
            {
                ModelState.AddModelError(nameof(catDTO.Name), "La categoria ya existe.");
                _logger.LogInformation("Crear: intento de crear categoria duplicada {Name}", normalizedName);
                return View(catDTO);
            }

            var entity = new Category { Name = normalizedName }; // ID is DB Generated
            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "DbUpdateException al crear categoría '{Name}'", normalizedName);
            ModelState.AddModelError("", "No se pudo crear la categoría. Intente nuevamente.");
            return View(catDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al tratar de crear la categoria.");
            ModelState.AddModelError("", "Se produjo un error al intentar crear la categoría.");
            return View(catDTO);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Editar(int? id)
    {
        if (!id.HasValue)
        {
            _logger.LogWarning("Editar GET: id nulo.");
            return RedirectToAction(nameof(Index));
        }

        var entity = await _context.Categories.FindAsync(id.Value);
        if (entity == null)
        {
            _logger.LogWarning("Editar GET: categoría no encontrada. Id: {Id}", id.Value);
            return RedirectToAction(nameof(Index));
        }

        var dto = new CategoryDTO
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(CategoryDTO catDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Editar POST: ModelState inválido.");
            return View(catDTO);
        }

        var normalizedName = catDTO.Name?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            ModelState.AddModelError(nameof(catDTO.Name), "El nombre es obligatorio.");
            return View(catDTO);
        }

        try
        {
            var existing = await _context.Categories.FindAsync(catDTO.Id);
            if (existing == null)
            {
                ModelState.AddModelError("", "La categoria no existe.");
                _logger.LogInformation("Editar POST: intento de editar categoría inexistente Id={Id}", catDTO.Id);
                return View(catDTO);
            }

            bool nameTaken = await _context.Categories
                                           .AsNoTracking()
                                           .AnyAsync(c => c.Id != catDTO.Id && c.Name.ToLower() == normalizedName.ToLower());
            if (nameTaken)
            {
                ModelState.AddModelError(nameof(catDTO.Name), "Ya existe otra categoría con ese nombre.");
                return View(catDTO);
            }

            existing.Name = normalizedName;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "DbUpdateException al editar categoría Id={Id}", catDTO.Id);
            ModelState.AddModelError("", "No se pudo editar la categoría. Intente nuevamente.");
            return View(catDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al editar categoría Id={Id}", catDTO.Id);
            ModelState.AddModelError("", "Se produjo un error al intentar editar la categoría.");
            return View(catDTO);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Detalles(int? id)
    {
        if (!id.HasValue)
        {
            _logger.LogWarning("Detalles GET: id nulo.");
            return RedirectToAction(nameof(Index));
        }

        var entity = await _context.Categories.FindAsync(id.Value);
        if (entity == null)
        {
            _logger.LogWarning("Detalles GET: categoría no encontrada. Id: {Id}", id.Value);
            return RedirectToAction(nameof(Index));
        }

        var dto = new CategoryDTO
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int? id)
    {
        if (!id.HasValue)
        {
            _logger.LogWarning("Eliminar GET: id nulo.");
            return RedirectToAction(nameof(Index));
        }

        var entity = await _context.Categories.FindAsync(id.Value);
        if (entity == null)
        {
            _logger.LogWarning("Eliminar GET: categoría no encontrada. Id: {Id}", id.Value);
            return RedirectToAction(nameof(Index));
        }

        var dto = new CategoryDTO
        {
            Id = entity.Id,
            Name = entity.Name
        };

        return View(dto);
    }
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(CategoryDTO catDTO)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Eliminar POST: ModelState inválido.");
            return View(catDTO);
        }

        try
        {
            var existing = await _context.Categories.FindAsync(catDTO.Id);
            if (existing == null)
            {
                ModelState.AddModelError("", "La categoria no existe.");
                _logger.LogInformation("Eliminar POST: intento de eliminar categoría inexistente Id={Id}", catDTO.Id);
                return View(catDTO);
            }

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "DbUpdateException al eliminar categoría Id={Id}", catDTO.Id);
            ModelState.AddModelError("", "No se pudo eliminar la categoría. Intente nuevamente.");
            return View(catDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar categoría Id={Id}", catDTO.Id);
            ModelState.AddModelError("", "Se produjo un error al intentar eliminar la categoría.");
            return View(catDTO);
        }
    }

}
