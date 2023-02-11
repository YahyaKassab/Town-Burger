using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItem>> GetFullMenu();
        Task<GenericResponse<MenuItem>> AddMenuItemAsync(MenuItemDto model);
    }
    public class MenuService : IMenuService
    {
        private readonly AppDbContext _context;

        public MenuService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse<MenuItem>> AddMenuItemAsync(MenuItemDto model)
        {
            try
            {
                var menuItem = new MenuItem()
                {
                    Title = model.Title,
                    Type = model.Type,
                    Description = model.Description,
                    ImageSource = model.ImageSource,
                    Price = model.Price,
                };
                var result = await _context.MenuItems.AddAsync(menuItem);
                await _context.SaveChangesAsync();
                return new GenericResponse<MenuItem>
                {
                    IsSuccess = true,
                    Message = "MenuItem Added Successfully",
                    Result = menuItem
                };

            }
            catch(Exception ex)
            {
                return new GenericResponse<MenuItem>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
                
        }

        public async Task<IEnumerable<MenuItem>> GetFullMenu()
        {
            throw new NotImplementedException();
        }

    }
}
