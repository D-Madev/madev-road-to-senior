namespace NotesAPI.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Data;
using NotesAPI.Models;

public class NotesService : INotesService
{
    private readonly NotesDbContext _context;
    private readonly IDistributedCache _cache;
    private const string NotesCacheKey = "all_notes";

    public NotesService(NotesDbContext context, IDistributedCache cache) => (_context, _cache) = (context, cache);

    /*
     * El acceso a la base de datos es una operación lenta (I/O Bound). 
     * Por eso, en .NET Core es una regla de oro usar programación 
     * asíncrona para no bloquear el thread del servidor.
     */
    public async Task<List<Note>> GetAllAsync()
    {
        // --- CÓDIGO DE CAOS ---
        //Random rng = new Random();
        //if (rng.Next(1, 6) == 1) // 20% de probabilidad de fallo
        //    throw new Exception("🔥 CAOS: Error crítico inesperado en el sistema de persistencia.");

        var cachedNotes = await _cache.GetStringAsync(NotesCacheKey);

        if (!string.IsNullOrEmpty(cachedNotes)) return JsonSerializer.Deserialize<List<Note>>(cachedNotes) ?? new List<Note>();
        
        // Si no está, ir a la BD
        var notes = await _context.Notes.ToListAsync();

        // Guardar en Redis por 10 minutos
        var options = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

        await _cache.SetStringAsync(NotesCacheKey, JsonSerializer.Serialize(notes), options);
        
        return notes;
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        // "El Id debe ser un número positivo mayor que cero."
        if (id <= 0) return null;
        var note = await _context.Notes.FindAsync(id);

        // "No se encontró una nota con Id = {id}."
        if (note == null) return null;
        return note;
    }

    public async Task<Note?> CreateAsync(Note newNote)
    {
        //"Debe envíar una nota, revise que se esté envíando correctamente la informacion."
        if (newNote == null) return null;

        await _context.Notes.AddAsync(newNote);
        await _context.SaveChangesAsync();

        // Invalidar la cache
        await _cache.RemoveAsync(NotesCacheKey);

        return newNote;
    }

    public async Task<bool> UpdateAsync(int id, Note noteUpdate)
    {
        // "Debe envíar una nota válida con un Id positivo mayor que cero."
        // "El Id debe ser un número positivo mayor que cero."
        if (noteUpdate == null || id <= 0) return false;

        var noteToUpdate = await _context.Notes.FindAsync(id);

        // "No se encontró una nota con Id = {noteUpdate.Id}."
        if (noteToUpdate == null) return false;

        noteToUpdate.Title = noteUpdate.Title;
        noteToUpdate.Content = noteUpdate.Content;

        await _context.SaveChangesAsync();

        // Invalidar la cache
        await _cache.RemoveAsync(NotesCacheKey);

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        // "El Id debe ser un número positivo mayor que cero."
        if (id <= 0) return false;

        var noteToDelete = await _context.Notes.FindAsync(id);

        // "No se encontró una nota con Id = {id}."
        if (noteToDelete == null) return false;

        _context.Notes.Remove(noteToDelete);
        await _context.SaveChangesAsync();

        // Invalidar la cache
        await _cache.RemoveAsync(NotesCacheKey);

        return true;
    }
}