using friendly.Models;

namespace friendly.DAL;

public interface IUserRepository
{
    Task<IEnumerable<User>?> GetAll();
    Task<User?> GetUserById(int id);
    Task<bool> Create(User user);
    Task<bool> Update(User user);
    Task<bool> Delete(int id);
}