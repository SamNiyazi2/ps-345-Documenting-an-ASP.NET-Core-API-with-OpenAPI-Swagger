using Library.API.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SSN_GenUtil_StandardLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static Library.API.Authentication.AuthenticationUtil;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
// 11/12/2022 11:14 am - SSN - Mainly to get username and password for API

namespace Library.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexPageVariablesController : ControllerBase
    {
        private readonly ILogger_SSN logger;

        public IndexPageVariablesController(ILogger_SSN logger)
        {
            this.logger = logger;
        }


        [AllowAnonymous]
        // GET: api/<IndexPageVariablesController>
        [HttpGet]
        [Produces("application/json")]
        public async Task<IEnumerable<string>> Get()
        {

            logger.TrackEvent("ps-345-20221112-2049:IndexPageVar: Get");

            string userName = null;
            string password = null;

            try
            {

                string randommerio_APIKey = Environment.GetEnvironmentVariable("randommerio_APIKey");
                if (string.IsNullOrEmpty(randommerio_APIKey))
                {
                    return new string[] { "Missing randommerio_APIKey - 20221112-1302" };
                }


                // 11/12/2022 11:36 pm - SSN - randommer.io give 503 in Azure. Replace API with random-data-api.com
                // string url_forUserName = "https://randommer.io/api/Name?nameType=firstname&quantity=1";
                // userName = await getNameFromAPI(randommerio_APIKey, url_forUserName);

                // string url_forPassword = "https://randommer.io/api/Text/Password?length=6&hasDigits=true&hasUppercase=true&hasSpecial=false";
                // password = await getNameFromAPI(randommerio_APIKey, url_forPassword);


                string url_forUserName = "https://random-data-api.com/api/v2/users";
                Random_Data_API_Record apiRecord = await getNameFromRandom_data_api(url_forUserName);
                if (apiRecord != null)
                {
                    userName = apiRecord.First_Name;
                    password = apiRecord.Password;
                }

                AuthenticationUtil.apiCredList.AddOrUpdate(AuthenticationUtil.createDicKey(userName, password), new Random_Data_API_Record { First_Name = userName, Password = password }, addOrUpdateCredList);

            }
            catch (Exception ex)
            {

                await logger.PostException(ex, "ps-345-20221112-2050:IndexPageVar", "Get error");

            }


            return new string[] { userName, password };
        }



        private Random_Data_API_Record addOrUpdateCredList(string key, Random_Data_API_Record newRecord)
        {
            return newRecord;
        }

        private static async Task<string> getNameFromAPI_RandommerIO(string randommerio_APIKey, string url_forUserName)
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


        private static async Task<Random_Data_API_Record> getNameFromRandom_data_api(string url_forUserName)
        {
            Random_Data_API_Record returnRecord = null;

            using (var client = new HttpClient())
            {

                var response = await client.GetAsync(new Uri(url_forUserName));

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                returnRecord = JsonConvert.DeserializeObject<Random_Data_API_Record>(content);

            }

            return returnRecord;
        }


        // GET api/<IndexPageVariablesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }


    }
}
