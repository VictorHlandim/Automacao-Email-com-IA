using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutomacaoEmail.Models
{
    public class AIResponse
    {
        public string tool {get; set;}
        public EmailParameters parameters {get; set;}
    }
    public class EmailParameters
    {
        public List<string> emails {get; set;}
        public string assunto {get; set;}
        public string corpo {get; set;}
    }

    public class OllamaModelResponse
    {
        public string model { get; set; }
        public string created_at { get; set; }
        public bool done { get; set; }
        public string response { get; set; }
        public string done_reason { get; set; }
        public JsonElement context { get; set; }

    }
    
}