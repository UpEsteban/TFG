using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using TFG.Domain.Shared.Abstractions.Repositories;
using TFG.Domain.Shared.Models;

namespace TFG.Infraestructure.ExternalApi.Edamam
{
    public class EdamamRepository : IEdamamRepository
    {
        private readonly IConfiguration Configuration;

        private readonly string url = "/search?q={2}&app_id={0}&app_key={1}";

        private HttpClient Client { get; }

        public EdamamRepository(IConfiguration Configuration)
        {
            this.Configuration = Configuration;

            var baseUrl = Configuration["Edamam_url"];

            Client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        /// <summary>
        /// Call to edamam and get recipes with recipe name
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public EdamamRecipeDTO GetRecipeByName(string text)
        {
            var appId = Configuration["Edamam_app_id"];
            var appkey = Configuration["Edamam_app_key"];

            var response = Client.GetAsync(string.Format(url, appId, appkey, text)).Result;

            EdamamRecipeDTO res = JsonConvert.DeserializeObject<EdamamRecipeDTO>(response.Content.ReadAsStringAsync().Result);

            return res;
        }
    }
}
