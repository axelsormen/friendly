using friendly.Models;

namespace friendly.DAL;

public interface ICommentRepository
{
    // Asynchronously retrieves all comments from the database
    Task<IEnumerable<Comment>?> GetAll();

    // Asynchronously retrieves a specific comment by its ID
    Task<Comment?> GetCommentById(int id);

    // Asynchronously creates a new comment in the database
    Task<bool> Create(Comment comment);

    // Asynchronously deletes a comment by its ID
    Task<bool> Delete(int id);
}
