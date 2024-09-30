using friendly.Models;

namespace friendly.DAL;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAll();
    Task<User?> GetUserById(int id);
    Task Create(User user);
    Task Update(User user);
    Task<bool> Delete(int id);
}