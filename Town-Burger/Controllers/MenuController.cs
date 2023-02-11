using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Town_Burger.Models.Dto;
using Town_Burger.Services;

namespace Town_Burger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
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
    }
}
