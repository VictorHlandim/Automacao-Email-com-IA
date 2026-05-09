using AutomacaoEmail.Models;
using System.Globalization;
using System.Text;
using System.Text.Json;

EnviadorDeEmail Send = new EnviadorDeEmail("PORTA SMTP", "USUARIO", "SENHA");

Console.Clear();
Console.WriteLine("Digite sua ação para o agente:\n");
string userInput = Console.ReadLine() ?? "";

string prompt = 
$@"Você é um assistente pessoal que tem acesso as seguintes ferramentas: 
1- Email;
A ação do usuário é: {userInput}. Responda APENAS com um objeto JSON de Formatting.Indented no formato exato: 
{{
""tool"": ""nome da ferramenta a ser usada"",
""parameters"":{{
""emails"": [""email1"", ""email2""],
""assunto"": ""assunto do email"",
""corpo"": ""corpo do email""
}}
}}
Não adicione texto extra, comentários ou blocos de código.";

#region Descerialização do JSON

OllamaClient ollama = new OllamaClient();
string resposta = await ollama.AskAsync(prompt);

Console.WriteLine("\nIniciando a escrita do JSON...");

File.WriteAllText(@"LOCALIZAÇÃO/DO/ARQUIVO.json", resposta);

Console.WriteLine("\nJSON escrito com sucesso.");

Console.WriteLine("\nComeçando deserialização do JSON...");
string jsonFromFile = File.ReadAllText(@"LOCALIZAÇÃO/DO/ARQUIVO.json");
OllamaModelResponse modelResponse = JsonSerializer.Deserialize<OllamaModelResponse>(jsonFromFile);
Console.WriteLine("\nJSON deserializado com sucesso.");

if (modelResponse == null || string.IsNullOrEmpty(modelResponse.response))
{
    Console.WriteLine("Resposta do modelo inválida ou vazia.");
    return;
}

string modelJson = modelResponse.response.Trim();
if (modelJson.StartsWith("```json") && modelJson.EndsWith("```"))
{
    int start = modelJson.IndexOf("```json") + 7;
    int end = modelJson.LastIndexOf("```");
    if (end > start)
    {
        modelJson = modelJson.Substring(start, end - start).Trim();
    }
}
AIResponse aiResponse = JsonSerializer.Deserialize<AIResponse>(modelJson);

Console.WriteLine("\nExtração do JSON concluída. Iniciando processamento...");

#endregion

void ProcessarResposta()
{
    switch(aiResponse?.tool)
    {
        case "Email":
            Email();
            break;
        default:
            Console.WriteLine("Ferramenta não reconhecida ou não iplementada ainda.");
            break;
    }
}

ProcessarResposta();

void Email(){

    string assunto = aiResponse?.parameters?.assunto ?? "Assunto não encontrado";
    string corpo = aiResponse?.parameters?.corpo ?? "Corpo do email não encontrado";
    List<string> emails = aiResponse?.parameters?.emails ?? new List<string>();

    for(int i = 0; i < emails.Count; i++)
    {
        Send.EnviarEmail(ReconhecimentoDeNome(), assunto, corpo);
    }
    Console.WriteLine("Emails mandados com sucesso!");
}

List<string> ReconhecimentoDeNome(){

    List<string> nomesConhecidos = new List<string>{"NOME1", "NOME2"};
    List<string> identificados = new List<string>();

    foreach(var nome in nomesConhecidos)
    {
        if(RemoverAcentos(aiResponse?.parameters?.emails.Any(e => e.Contains(nome, StringComparison.OrdinalIgnoreCase)) == true ? nome : "") == nome)
        {
            switch(nome)
            {
                case "NOME1":
                    identificados.Add("NOME1@gmail.com");
                    break;
                case "NOME2":
                    identificados.Add("NOME2@gmail.com");
                    break;
            }
        }
    }

    if(identificados.Count == 0)
    {
        Console.WriteLine("Nenhum nome reconhecido nos emails fornecidos.");
        return aiResponse?.parameters?.emails ?? new List<string>();
    }
    else{
        return identificados;
    }
    
    
    
}

static string RemoverAcentos(string texto)
{
    string normalized = texto.Normalize(NormalizationForm.FormD);
    StringBuilder sb = new StringBuilder();

    foreach (char c in normalized)
    {
        UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);

        if (uc != UnicodeCategory.NonSpacingMark)
        {
            sb.Append(c);
        }
    }

    return sb.ToString().Normalize(NormalizationForm.FormC);
}
