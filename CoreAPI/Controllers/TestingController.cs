using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingController : ControllerBase
    {
        private IConfiguration _config;

        public TestingController(IConfiguration config)
        {
            _config = config;
        }
        // GET: api/Testing
        [HttpGet]
        public IEnumerable<string> Get()
        {
            using (SqlConnection conn = new SqlConnection())
            {
                using (SqlDataAdapter dat = new SqlDataAdapter("", conn))
                {

                }
            }
            return new string[] { "", "" };
        }

        // GET: api/Testing/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Testing
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

    }
}
