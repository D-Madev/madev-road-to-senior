namespace NotesAPI.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Data;
using NotesAPI.Models;

public class NotesService : INotesService
{
    private readonly NotesDbContext _context;

    public NotesService(NotesDbContext context) => _context = context;

    /*
     * El acceso a la base de datos es una operación lenta (I/O Bound). 
     * Por eso, en .NET Core es una regla de oro usar programación 
     * asíncrona para no bloquear el thread del servidor.
     */
    public async Task<List<Note>> GetAllAsync()
    {
        return await _context.Notes.ToListAsync();
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
        return true;
    }
}