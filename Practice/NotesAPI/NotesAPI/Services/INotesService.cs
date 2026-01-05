namespace NotesAPI.Services;

using NotesAPI.Models;

public interface INotesService
{
    Task<List<Note>> GetAllAsync();
    Task<Note?> GetByIdAsync(int id);
    Task<Note?> CreateAsync(Note newNote);
    Task<bool> UpdateAsync(int id, Note noteUpdate);
    Task<bool> DeleteAsync(int id);
}
