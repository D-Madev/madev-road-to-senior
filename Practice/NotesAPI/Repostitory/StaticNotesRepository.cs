namespace NotesAPI.Repository;

using NotesAPI.IRepository;
using NotesAPI.Models;

public class StaticNotesRepository : INotesRepository
{
    private readonly List<Note> _notes = new List<Note>
    {
        new Note { Id = 1, Title = "Note 1", Content = "Content 1" },
        new Note { Id = 2, Title = "Note 2", Content = "Content 2" },
        new Note { Id = 3, Title = "Note 3", Content = "Content 3" }
    };

    public List<Note> GetAll()
    {
        return _notes;
    }
}
