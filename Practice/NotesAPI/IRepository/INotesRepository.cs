namespace NotesAPI.IRepository;

using NotesAPI.Models;

public interface INotesRepository
{
    List<Note> GetAll();
}
