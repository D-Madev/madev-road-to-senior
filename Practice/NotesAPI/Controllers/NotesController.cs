namespace NotesAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesAPI.Models;
using NotesAPI.Services;

[Authorize]
[ApiController]
[Route("[controller]")]
public class NotesController : ControllerBase
{
    // private readonly INotesRepository _repository;
    // private readonly NotesDbContext _context;
    private readonly INotesService _service;

    public NotesController(INotesService service) => _service = service;

    /*
     * El acceso a la base de datos es una operación lenta (I/O Bound). 
     * Por eso, en .NET Core es una regla de oro usar programación 
     * asíncrona para no bloquear el thread del servidor.
     */
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        // Testing the global exception handling (Q23)
        // throw new Exception("Tarea 8: Error de prueba para el manejo global de excepciones.");
        
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var note = await _service.GetByIdAsync(id);
        
        if (note == null) return NotFound($"No se encontró una nota con Id = {id}.");

        return Ok(note);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Note newNote)
    {
        /* 
         * Si el JSON que envía el cliente no se puede deserializar (ej: el Content-Type es incorrecto), 
         * el Model Binder lo intentará poner como null. 
         * No obstante, si el JSON es inválido, el atributo [ApiController] 
         * ya intercepta esto y devuelve un error 400 Bad Request automáticamente, 
         * antes de que tu código se ejecute.
        */
        var createdNote = await _service.CreateAsync(newNote);

        if (createdNote == null) return NotFound("Error al crear la nota.");

        // Devolver 201 Created (Convención RESTful)
        return Created($"/{ControllerContext.RouteData.Values["controller"]}/{newNote.Id}", newNote);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] Note noteUpdate)
    {
        if (id != noteUpdate.Id) return BadRequest("IDs no coinciden");

        var updated = await _service.UpdateAsync(id, noteUpdate);

        if (!updated) return NotFound($"No se encontró una nota con Id = {id}.");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted) return NotFound($"No se encontró una nota con Id = {id}.");

        return NoContent();
    }
}