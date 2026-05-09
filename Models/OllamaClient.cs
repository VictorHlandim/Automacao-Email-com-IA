using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace AutomacaoEmail.Models
{
    public class OllamaClient
    {
        private readonly HttpClient _httpClient;
        public OllamaClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> AskAsync(string prompt)
        {   
            var response =await _httpClient.PostAsJsonAsync("LOCALIZAÇÃO/DO/AGENTE/LOCAL/PORTA",
                new
                {
                    model = "qwen2.5-coder",
                    prompt = prompt,
                    stream = false
                });

            var result =
                await response.Content.ReadAsStringAsync();

            return result;
        }
    }
    
}