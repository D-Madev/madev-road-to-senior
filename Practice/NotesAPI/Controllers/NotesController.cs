namespace NotesAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using NotesAPI.IRepository;

[ApiController]            // 1. Tells ASP.NET Core: This is an API class.
[Route("[controller]")]    // 2. Defines the URL path (e.g., /api/notes).
public class NotesController : ControllerBase
{
    private readonly INotesRepository _repository;
 // private readonly DbContext _context;

    public NotesController(INotesRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(_repository.GetAll());
    }
}