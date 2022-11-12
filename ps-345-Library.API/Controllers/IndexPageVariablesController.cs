using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
// 11/12/2022 11:14 am - SSN - Mainly to get username and password for API

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexPageVariablesController : ControllerBase
    {
        [AllowAnonymous]
        // GET: api/<IndexPageVariablesController>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IEnumerable<string>> Get()
        {
            string randommerio_APIKey = Environment.GetEnvironmentVariable("randommerio_APIKey");
            if (string.IsNullOrEmpty(randommerio_APIKey))
            {
                return new string[] { "Missing randommerio_APIKey - 20221112-1302" };
            }

            string userName = null;
            string password = null;

            string url_forUserName = "https://randommer.io/api/Name?nameType=firstname&quantity=1";

            userName = await getNameFromAPI(randommerio_APIKey, url_forUserName);

            string url_forPassword = "https://randommer.io/api/Text/Password?length=6&hasDigits=true&hasUppercase=true&hasSpecial=false";

            password = await getNameFromAPI(randommerio_APIKey, url_forPassword);

            Startup.apiUserName = userName;
            Startup.apiPassword = password;

            return new string[] { userName, password };
        }


        private static async Task<string> getNameFromAPI(string randommerio_APIKey, string url_forUserName)
        {
            string userName = null;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Api-Key", randommerio_APIKey);

                var response = await client.GetAsync(new Uri(url_forUserName));

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                if (content.StartsWith("["))
                {
                    IEnumerable<string> userNames = JsonConvert.DeserializeObject<IEnumerable<string>>(content);
                    if (userNames.Count() > 0) userName = userNames.First();
                }
                else
                {
                    userName = JsonConvert.DeserializeObject<string>(content);
                }

            }

            return userName;
        }

        // GET api/<IndexPageVariablesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


    }
}
