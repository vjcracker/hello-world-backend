using Microsoft.AspNetCore.Mvc;

namespace HelloWorldApp.controllers
{
    // [ApiController]
    [Route("[controller]")]
    public class HelloWorldcontroller : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new {message = "Hello World!"});
        }
    }
}