using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
// 03/12/2022 03:44 pm - SSN - [20220312-1544] - [001] - M04-06 - Demo - Working with API conventions

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [20220312-1544] - [001]
    // 03/12/2022 09:19 pm - SSN - [20220312-1849] - [003] - M04-07 - Creating custom conventions
    //[ApiConventionType(typeof(DefaultApiConventions))]
    [ApiConventionType(typeof(CustomConventions.CustomConvention))]
    public class ConventionTestsontroller : ControllerBase
    {
        // GET: api/<ConventionTestsontroller>
        [HttpGet]

        //////////////////////  [20220312-1544] - [001]
        //////////////////////  [ApiConventionMethod(typeof(DefaultApiConventions), nameof (DefaultApiConventions.Get))]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ConventionTestsontroller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


        // POST api/<ConventionTestsontroller>
        [HttpPost]
        // 03/12/2022 06:49 pm - SSN - [20220312-1849] - [001] - M04-07 - Creating custom conventions
        // Produces only 200 without the CustomConvention
        // [20220312-1849] - [003]
        //[ApiConventionMethod(typeof(CustomConventions.CustomConvention), nameof(CustomConventions.CustomConvention.Insert))]
        //public void Post([FromBody] string value)
        public void InsertTest([FromBody] string value)
        {
        }


        // PUT api/<ConventionTestsontroller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ConventionTestsontroller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
