using Town_Burger.Models;
using Town_Burger.Models.Context;
using Town_Burger.Models.Dto;
using Town_Burger.Models.Responses;

namespace Town_Burger.Services
{
    public interface IMenuService
    {

        //add from the parent table not from the child indepenedently 

        Task<GenericResponse<MenuItem>> GetMenuItemById(int id);
        Task<GenericResponse<IEnumerable<MenuItem>>> GetFullMenu();
        Task<GenericResponse<MenuItem>> AddMenuItemAsync(MenuItemDto model);
        Task<GenericResponse<IEnumerable<MenuItem>>> GetByType(string type);
        GenericResponse<MenuItem> UpdateMenuItem(MenuItem item);
        Task<GenericResponse<MenuItem>> DeleteMenuItem(int itemId);
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

        

        public async Task<GenericResponse<IEnumerable<MenuItem>>> GetByType(string type)
        {
            var items = _context.MenuItems.Where(e=>e.Type.ToLower() == type.ToLower()).ToList();
            if (items.Count < 1)
                return new GenericResponse<IEnumerable<MenuItem>>()
                {
                    IsSuccess = false,
                    Message = "No Items With this type"
                };
            return new GenericResponse<IEnumerable<MenuItem>>()
            {
                IsSuccess = true,
                Message = $"{type} Items Fetched Successfully",
                Result = items
            };
        }

        public async Task<GenericResponse<IEnumerable<MenuItem>>> GetFullMenu()
        {
            var fullMenu = _context.MenuItems.ToList();
            if (fullMenu.Count < 1)
                return new GenericResponse<IEnumerable<MenuItem>>
                {
                    IsSuccess = false,
                    Message = "No Items In The Menu"
                };
            return new GenericResponse<IEnumerable<MenuItem>>
            {
                IsSuccess = true,
                Message = "Items Fetched Successfully",
                Result = fullMenu
            };

        }
        public async Task<GenericResponse<MenuItem>> GetMenuItemById(int id)
        {
            var item = await _context.MenuItems.FindAsync(id);
            if (item == null)
                return new GenericResponse<MenuItem>
                {
                    IsSuccess = false,
                    Message = "No Menu Item with this id"
                };
            return new GenericResponse<MenuItem>
            {
                IsSuccess = true,
                Message = "Menu Item Got Successfully",
                Result = item
            };
        }

        public GenericResponse<MenuItem> UpdateMenuItem(MenuItem item)
        {
            try
            {
                _context.Update(item);
                _context.SaveChanges();
                return new GenericResponse<MenuItem>
                {
                    IsSuccess = true,
                    Message = "Item Updated Successfully",
                    Result = item
                };

            }catch (Exception ex)
            {
                return new GenericResponse<MenuItem>
                {
                    IsSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<GenericResponse<MenuItem>> DeleteMenuItem(int itemId)
        {
            try
            {
                var item = await _context.MenuItems.FindAsync(itemId);
                if (item == null)
                    return new GenericResponse<MenuItem>
                    {
                        IsSuccess = false,
                        Message = "Item Not Found"
                    };
                _context.Remove(item);
                await _context.SaveChangesAsync();
                return new GenericResponse<MenuItem>
                {
                    IsSuccess = true,
                    Message = "Item Deleted Successfully",
                    Result = item
                };
            }
            catch (Exception ex)
            {
                return new GenericResponse<MenuItem>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }

        }
    }
}
