using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using SampleMvcApp.Models;

namespace SampleMvcApp.Controllers
{
    public class HomeController : Controller
    {
        readonly IConfiguration _configuration;
        private List<ReportItem> reportItems = new List<ReportItem>();

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // If the user is authenticated, then this is how you can get the access_token and id_token
            if (User.Identity.IsAuthenticated)
            {
                string accessToken = await HttpContext.GetTokenAsync("access_token");

                // if you need to check the access token expiration time, use this value
                // provided on the authorization response and stored.
                // do not attempt to inspect/decode the access token
                DateTime accessTokenExpiresAt = DateTime.Parse(
                    await HttpContext.GetTokenAsync("expires_at"),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind);

                string idToken = await HttpContext.GetTokenAsync("id_token");

                // Now you can use them. For more info on when and how to use the
                // access_token and id_token, see https://auth0.com/docs/tokens


                //Retrive rules 
                //Get the management API access token
                var client = new RestClient("https://" + _configuration["Auth0:Domain"] + ".auth0.com/oauth/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter("application/x-www-form-urlencoded", "grant_type=client_credentials&client_id=" + _configuration["Auth0:ClientId"] + "&client_secret=" + _configuration["Auth0:ClientSecret"] + "&audience=https://" + _configuration["Auth0:Domain"] + ".auth0.com/api/v2/", ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);

                //Get the management api rules
                dynamic mgmtApiRulesObject = JsonConvert.DeserializeObject(response.Content);
                var client2 = new RestClient("https://" + _configuration["Auth0:Domain"] + ".auth0.com/api/v2/rules");
                var request2 = new RestRequest(Method.GET);
                request2.AddHeader("content-type", "application/json");
                request2.AddHeader("authorization", "Bearer " + mgmtApiRulesObject.access_token);
                IRestResponse response2 = client2.Execute(request2);
                List<Rule> rules = JsonConvert.DeserializeObject<List<Rule>>(response2.Content);

                //Get the management api clients
                dynamic mgmtApiClientsObject = JsonConvert.DeserializeObject(response.Content);
                var client3 = new RestClient("https://" + _configuration["Auth0:Domain"] + ".auth0.com/api/v2/clients");
                var request3 = new RestRequest(Method.GET);
                request3.AddHeader("content-type", "application/json");
                request3.AddHeader("authorization", "Bearer " + mgmtApiClientsObject.access_token);
                IRestResponse response3 = client3.Execute(request3);
                List<Client> clients = JsonConvert.DeserializeObject<List<Client>>(response3.Content);

                //Search for clients which are mentioned in rules and build report items 
                foreach (Client c in clients)
                {
                    Regex rx = new Regex("(context\\.clientName)\\s*(===|==|!==|!=)\\s*('" + Regex.Escape(c.Name) + "')");
                    IEnumerable<Rule> clientRules = from r in rules where rx.IsMatch(r.Script) select r;

                    if (clientRules.Any())
                    {
                        foreach (Rule r in clientRules)
                        {
                            reportItems.Add(new ReportItem
                            {
                                ClientID = c.Client_ID,
                                ClientName = c.Name,
                                RuleName = r.Name,
                                RuleScript = r.Script,
                                RuleID = r.ID                                
                            });
                        }
                    }
                }

                return View("Report", reportItems);
            }
            
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
