using friendly.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace friendly.DAL
{
    public interface ILikeRepository
    {
        Task<IEnumerable<Like>?> GetAll();
        Task<Like?> GetLikeById(int id);
        Task<bool> Create(Like like);
        Task<bool> Delete(int id);
    }
}