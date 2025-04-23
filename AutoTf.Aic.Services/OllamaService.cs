using System.Text.Json.Nodes;
using Microsoft.Extensions.Hosting;
using OllamaSharp;
using OllamaSharp.Models;
using OllamaSharp.Models.Chat;

namespace AutoTf.Aic.Services;

public class OllamaService : IHostedService
{
    private Uri _uri = new Uri("http://localhost:11434");
    private IOllamaApiClient _ollama;

    private List<Message> _previousMessages = new List<Message>();
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartupModel();
        return Task.CompletedTask;
    }

    private void StartupModel()
    {
        _ollama = new OllamaApiClient(_uri, "phi3");
        // TODO: Preheat model like this?
        _ollama.ChatAsync(new ChatRequest
        {
            Model = "phi3:mini",
            Stream = false,
            Messages = new List<Message>(),
            Options = new RequestOptions
            {
                Temperature = .1f,
            },
            KeepAlive = "24h",
            Format = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["isSafe"] = new JsonObject
                    {
                        ["type"] = "boolean"
                    }
                },
                ["required"] = new JsonArray { "isSafe" }
            },
        });
        _previousMessages.Add(new Message(ChatRole.System, "Respond as short as possible."));
    }

    public async Task<string> GetIsSafe(string context, bool keep = false)
    {
        List<Message> tempMessages = new List<Message>(_previousMessages);
        tempMessages.Add(new Message(ChatRole.User, context));

        IAsyncEnumerable<ChatResponseStream?> rawResponse = _ollama.ChatAsync(new ChatRequest()
        {
            Model = "qwen2.5:0.5b",
            Messages = tempMessages,
            Options = new RequestOptions()
            {
                Temperature = 0f,
                Seed = 0,
                RepeatPenalty = 1f
            },
            Format = new JsonObject
            {
                ["type"] = "object",
                ["properties"] = new JsonObject
                {
                    ["isSafe"] = new JsonObject
                    {
                        ["type"] = "boolean"
                    }
                },
                ["required"] = new JsonArray { "isSafe" }
            },
            Stream = false,
            KeepAlive = "24h"
        });
        
        string fullResponse = string.Empty;
        
        await foreach (ChatResponseStream? response in rawResponse)
        {
            if(response == null)
                continue;
            // TODO: Implement and check for tool calls in the future.
            fullResponse += response.Message.Content;
            
            tempMessages.Add(response.Message);
        }

        // TODO: Concurrency
        if (keep)
            _previousMessages = tempMessages;
        
        return fullResponse;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}