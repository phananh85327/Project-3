using Back_Site.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Back_Site.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly DataContext _context;

        public ManagementController(DataContext context)
        {
            _context = context;
        }

        // GET: api/<ManagementController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var users = await _context.UserData.ToArrayAsync();

            return Ok(users);
        }

        // GET api/<ManagementController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ManagementController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ManagementController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ManagementController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
