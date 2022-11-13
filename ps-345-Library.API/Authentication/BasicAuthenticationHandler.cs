using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SSN_GenUtil_StandardLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Library.API.Authentication.AuthenticationUtil;
using static Library.API.Controllers.IndexPageVariablesController;

// 03/15/2022 11:05 pm - SSN - [20220315-2303] - [001] - M06-08 - Demo - Protecting your API

namespace Library.API.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly ILogger_SSN logger_SSN;

        // 11/13/2022 12:16 am - SSN - Add ILogger_SSN
        public BasicAuthenticationHandler(
                                                IOptionsMonitor<AuthenticationSchemeOptions> options,
                                                ILoggerFactory logger,
                                                UrlEncoder encoder,
                                                ISystemClock clock,
                                                ILogger_SSN logger_SSN) : base(options, logger, encoder, clock)
        {
            this.logger_SSN = logger_SSN;
        }


        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));
            }


            try
            {
                var authenticationHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authenticationHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                // 11/12/2022 1:52 pm - SSN - Replaced
                //if (username == "Pluralsight" && password == "Pluralsight")
                if (isValidCredential(username, password))
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, username)
                    };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }

                return Task.FromResult(AuthenticateResult.Fail("Invalid username or password"));


            }
            catch (Exception ex)
            {
                logger_SSN.PostException(ex, "ps-345-BasicAuthenticationHandler:20221113-0018", "Error in authentication.");
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header (2)"));

            }


        }

        private bool isValidCredential(string username, string password)
        {
            Random_Data_API_Record record = AuthenticationUtil.apiCredList.FirstOrDefault(r => r.Key == AuthenticationUtil.createDicKey(username, password)).Value;
            return record != null;
        }


    }
}
