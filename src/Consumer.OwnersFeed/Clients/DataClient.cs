using Consumer.Contracts.Interfaces;
using Consumer.Contracts.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Consumer.OwnersFeed.Clients
{
    public class DataClient : IDataClient
    {
        private readonly HttpClient _client;

        public DataClient(HttpClient client, IConfiguration configuration)
        {
            _client = client;
            _client.BaseAddress = new Uri(configuration["url"]);
        }

        public async Task<IEnumerable<Owner>> GetOwners()
        {
            var response = await _client.GetAsync("");

            response.EnsureSuccessStatusCode();
            return JsonSerializer.Deserialize<IEnumerable<Owner>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

    }
}
