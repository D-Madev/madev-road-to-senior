namespace CRUD_TEST.Controllers;


using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using CRUD_TEST.Models;
using CRUD_TEST.Data;
using CRUD_TEST.DTOs;

public class ItemController : Controller
{
    private readonly ILogger<ItemController> _logger;
    private readonly AppDbContext _context;

    public ItemController(ILogger<ItemController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var items = await _context.Items
                                            .Include(i => i.Category)
                                            .Include(sn => sn.SerialNumber)
                                            .ToListAsync();
            return View(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al tratar de listar los items.");
            return BadRequest($"Se produjo un error al tratar de listar los items. \n\n{ex}");
        }
    }

    // [HttpGet, ActionName("Crear")]
    // public async Task<IActionResult> Create() { }

    [HttpGet]
    public async Task<IActionResult> Update(int? id)
    {
        if (!id.HasValue)
        {
            _logger.LogWarning("Editar GET: id nulo.");
            return RedirectToAction(nameof(Index));
        }

        var entity = await _context.Items
                                         .AsNoTracking() // read-only
                                         .Include(i => i.Category)
                                         .Include(i => i.SerialNumber)
                                         .FirstOrDefaultAsync(i => i.Id == id.Value);
        if (entity == null)
        {
            _logger.LogWarning("Editar GET: item no encontrado. Id: {Id}", id.Value);
            return RedirectToAction(nameof(Index));
        }
        return View(entity);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (!id.HasValue)
        {
            _logger.LogWarning("Details GET: id nulo.");
            return RedirectToAction(nameof(Index));
        }

        var entity = await _context.Items
                                         .AsNoTracking() // read-only
                                         .Include(i => i.Category)
                                         .Include(i => i.SerialNumber)
                                         .FirstOrDefaultAsync(i => i.Id == id.Value);
        if (entity == null)
        {
            _logger.LogWarning("Details GET: item no encontrado. Id: {Id}", id.Value);
            return RedirectToAction(nameof(Index));
        }
        return View(entity);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (!id.HasValue)
        {
            _logger.LogWarning("Eliminar GET: id nulo.");
            return RedirectToAction(nameof(Index));
        }

        var entity = await _context.Items
                                    .AsNoTracking() // read-only
                                    .Include(i => i.Category)
                                    .Include(i => i.SerialNumber)
                                    .FirstOrDefaultAsync(i => i.Id == id.Value);
        if (entity == null)
        {
            _logger.LogWarning("Eliminar GET: Item no encontrado. Id: {Id}", id.Value);
            return RedirectToAction(nameof(Index));
        }

        var dto = new ItemDTO
        {
            Id = entity.Id,
            Name = entity.Name,
            Price = entity.Price,
            SerialNumberId = entity.SerialNumberId,
            SerialNumber = entity.SerialNumber,
            CategoryId = entity.CategoryId,
            Category = entity.Category,
        };

        return View(dto);   
    }
}
