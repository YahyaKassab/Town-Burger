using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Town_Burger.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize("Admin")]
    public class AdminController : ControllerBase
    {
    }
}
