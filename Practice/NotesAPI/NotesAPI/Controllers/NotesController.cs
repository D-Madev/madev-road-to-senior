namespace NotesAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesAPI.Models;
using NotesAPI.Services;

/// <summary>
/// Gestiona las notas personales de los usuarios autenticados.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
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
    /// <summary>
    /// Obtiene la lista completa de notas. (Acceso público para demostración)
    /// </summary>
    /// <returns>Una lista de objetos de tipo Note.</returns>
    /// <response code="200">Devuelve la lista de notas.</response>
    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync()
    {
        // Testing the global exception handling (Q23)
        // throw new Exception("Tarea 8: Error de prueba para el manejo global de excepciones.");
        
        return Ok(await _service.GetAllAsync());
    }

    /// <summary>
    /// Obtiene una nota específica mediante su ID.
    /// </summary>
    /// <param name="id">Identificador único de la nota.</param>
    /// <response code="200">Retorna la nota solicitada.</response>
    /// <response code="404">Si la nota con el ID especificado no existe.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var note = await _service.GetByIdAsync(id);
        
        if (note == null) return NotFound($"No se encontró una nota con Id = {id}.");

        return Ok(note);
    }

    /// <summary>
    /// Crea una nueva nota en el sistema.
    /// </summary>
    /// <param name="newNote">Objeto nota a crear.</param>
    /// <response code="201">Nota creada exitosamente.</response>
    /// <response code="400">Si el modelo de la nota es inválido.</response>
    /// <response code="401">No autorizado, falta token JWT.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Note), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

    /// <summary>
    /// Actualiza una nota existente.
    /// </summary>
    /// <param name="id">ID de la nota a actualizar.</param>
    /// <param name="noteUpdate">Datos actualizados de la nota.</param>
    /// <response code="204">Actualización exitosa (sin contenido).</response>
    /// <response code="400">Si los IDs no coinciden o los datos son inválidos.</response>
    /// <response code="404">Si la nota no existe.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] Note noteUpdate)
    {
        if (id != noteUpdate.Id) return BadRequest("IDs no coinciden");

        var updated = await _service.UpdateAsync(id, noteUpdate);

        if (!updated) return NotFound($"No se encontró una nota con Id = {id}.");

        return NoContent();
    }

    /// <summary>
    /// Elimina una nota del sistema.
    /// </summary>
    /// <param name="id">ID de la nota a eliminar.</param>
    /// <response code="204">Eliminación exitosa.</response>
    /// <response code="404">Si la nota no existe.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deleted = await _service.DeleteAsync(id);

        if (!deleted) return NotFound($"No se encontró una nota con Id = {id}.");

        return NoContent();
    }
}