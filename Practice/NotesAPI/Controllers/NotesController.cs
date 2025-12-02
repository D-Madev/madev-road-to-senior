namespace NotesAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Data;
using NotesAPI.IRepository;
using NotesAPI.Models;

[ApiController]            // 1. Tells ASP.NET Core: This is an API class.
[Route("[controller]")]    // 2. Defines the URL path (e.g., /api/notes).
public class NotesController : ControllerBase
{
    private readonly NotesDbContext _context;
    // private readonly INotesRepository _repository;

    public NotesController(NotesDbContext context) => _context = context;

    /*
     * El acceso a la base de datos es una operación lenta (I/O Bound). 
     * Por eso, en .NET Core es una regla de oro usar programación 
     * asíncrona para no bloquear el thread del servidor.
     */
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        // Testing the global exception handling (Q23)
        // throw new Exception("Tarea 8: Error de prueba para el manejo global de excepciones.");
        
        return Ok(await _context.Notes.ToListAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        if (id <= 0) return BadRequest("El Id debe ser un número positivo mayor que cero.");

        var note = await _context.Notes.FindAsync(id);
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
        if (newNote == null) return BadRequest("Debe envíar una nota, revise que se esté envíando correctamente la informacion.");

        await _context.Notes.AddAsync(newNote);
        await _context.SaveChangesAsync();

        // Devolver 201 Created (Convención RESTful)
        return Created($"/{ControllerContext.RouteData.Values["controller"]}/{newNote.Id}", newNote);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync([FromBody] Note noteUpdate)
    {
        if (noteUpdate == null || noteUpdate.Id <= 0) return BadRequest("Debe envíar una nota válida con un Id positivo mayor que cero.");
        
        var noteToUpdate = await _context.Notes.FindAsync(noteUpdate.Id);
  
        if (noteToUpdate == null) return NotFound($"No se encontró una nota con Id = {noteUpdate.Id}.");
        
        noteToUpdate.Title = noteUpdate.Title;
        noteToUpdate.Content = noteUpdate.Content;
        
        //_context.Notes.Update(noteToUpdate);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        if (id <= 0) return BadRequest("El Id debe ser un número positivo mayor que cero.");

        var noteToDelete = await _context.Notes.FindAsync(id);
        if (noteToDelete == null) return NotFound($"No se encontró una nota con Id = {id}.");

        _context.Notes.Remove(noteToDelete);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}