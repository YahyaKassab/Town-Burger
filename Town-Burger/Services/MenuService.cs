using Town_Burger.Models;
using Town_Burger.Models.Context;

namespace Town_Burger.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItem>> GetFullMenu();
    }
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;

        public MenuService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<MenuItem>> GetFullMenu()
        {
            throw new NotImplementedException();
        }

    }
}
