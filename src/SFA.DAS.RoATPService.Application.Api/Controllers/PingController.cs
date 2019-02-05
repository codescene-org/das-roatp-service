namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class PingController : Controller
    {
        [HttpGet("/Ping")]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}