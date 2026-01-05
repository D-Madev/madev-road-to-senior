namespace NotesAPI.Repostitories;

using NotesAPI.Models;

public interface INotesRepository
{
    List<Note> GetAll();
}
