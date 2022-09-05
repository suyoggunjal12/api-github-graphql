using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GraphQL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContributorController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<List<Node>>> GetContributorsAsync(string Owner, string Repo, string Branch)
        {

            try
            {
                List<Root> lst = new List<Root>();

                List<Node> Nodelst = new List<Node>();

                var httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://api.github.com/graphql")
                };

                var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

                httpClient.DefaultRequestHeaders.Add("User-Agent", "Graphql");

                string basicValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{MyConfig.GetSection("GithubAccount").Value}:{MyConfig.GetSection("PersonalToken").Value}"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicValue);

                if (!string.IsNullOrEmpty(Owner) && !string.IsNullOrEmpty(Repo) && !string.IsNullOrEmpty(Branch))
                {
                    var query = new GraphQLRequest
                    {
                        Query = @"
                query ownerQuery($Owner: String! $Name: String! $Expression: String!) {
                  repository(owner: $Owner name: $Name) {nameWithOwner
                   object(expression: $Expression){commitUrl ... on Commit {
        oid history(first: 100, since: ""2022-08-22T04:37:29Z""){totalCount nodes{author{user{login}}  committedDate additions deletions}}}} }}",
                        Variables = new { Owner = Owner, Name = Repo, Expression = Branch }
                    };

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        Content = new StringContent(JsonConvert.SerializeObject(query), Encoding.UTF8, "application/json")
                    };

                    using (var response = await httpClient.SendAsync(request))
                    {
                        response.EnsureSuccessStatusCode();

                        var responseString = await response.Content.ReadAsStringAsync();
                        lst.Add(JsonConvert.DeserializeObject<Root>(responseString));
                    }

                    var x = lst.Select(x => x.data.repository.@object.history.nodes.OrderBy(g => g.author.user.login)
                    .GroupBy(t => t.author.user.login).SelectMany(r => r).
                    Where(r => r.author.user.login.Count() > 1).ToList());

                    foreach (var item in x)
                    {
                        Nodelst.AddRange((IEnumerable<Node>)item);
                    }
                }


                return Nodelst;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
