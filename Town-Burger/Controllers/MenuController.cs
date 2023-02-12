using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Models.Dto;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("GetFullMenu")]
        public async Task<IActionResult> GetFullMenu()
        {
            var result = await _menuService.GetFullMenu();
            if(result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("AddMenuItem")]
        public async Task<IActionResult> AddItem(MenuItemDto model)
        {
            var result = await _menuService.AddMenuItemAsync(model);
            if(result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("GetItemById")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _menuService.GetMenuItemById(id);
            if(!result.IsSuccess)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("GetByType")]
        public async Task<IActionResult> GetByType(string type)
        {
            var result = await _menuService.GetByType(type);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
