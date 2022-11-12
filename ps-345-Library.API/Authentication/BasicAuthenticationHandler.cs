﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

// 03/15/2022 11:05 pm - SSN - [20220315-2303] - [001] - M06-08 - Demo - Protecting your API

namespace Library.API.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
                                                IOptionsMonitor<AuthenticationSchemeOptions> options,
                                                ILoggerFactory logger,
                                                UrlEncoder encoder,
                                                ISystemClock clock) : base(options, logger, encoder, clock)
        {

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
                if (username == Startup.apiUserName && password == Startup.apiPassword)
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

                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header (2)"));

            }


        }


    }
}
