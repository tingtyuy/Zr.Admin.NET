using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using OllamaSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.ServiceCore.Services
{
    public class OllamaService : IOllamaService
    {
        private readonly OllamaApiClient _client;
        private readonly ILogger<OllamaService> _logger;
        private string _selectedModel;

        public OllamaService(IConfiguration configuration, ILogger<OllamaService> logger)
        {
            _logger = logger;

            // 从配置文件读取设置
            var ollamaUrl = configuration["Ollama:Url"] ?? "http://localhost:11434";
            _selectedModel = configuration["Ollama:Model"] ?? "gemma2:2b";

            // 创建客户端
            _client = new OllamaApiClient(new Uri(ollamaUrl));
            _client.SelectedModel = _selectedModel;

            _logger.LogInformation("OllamaService 初始化完成，模型: {Model}, URL: {Url}", _selectedModel, ollamaUrl);
        }

        public async Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogDebug("发送消息: {Message}", message);

                var response = new StringBuilder();

                // 修正：使用 GenerateAsync 方法，返回 IAsyncEnumerable<GenerateResponseStream?>
                await foreach (var stream in _client.GenerateAsync(message, cancellationToken: cancellationToken))
                {
                    if (stream?.Response != null)
                    {
                        response.Append(stream.Response);
                    }
                }

                var result = response.ToString();
                _logger.LogDebug("收到回复: {Response}", result);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "调用Ollama失败");
                throw new OllamaServiceException("调用Ollama服务失败", ex);
            }
        }

        public async Task<string> SendMessageWithSystemPromptAsync(string message, string systemPrompt, CancellationToken cancellationToken = default)
        {
            try
            {
                // 使用 GenerateRequest 来传递系统提示
                var request = new GenerateRequest
                {
                    Prompt = message,
                    System = systemPrompt,
                    Model = _selectedModel,
                    Stream = true
                };

                var response = new StringBuilder();

                await foreach (var stream in _client.GenerateAsync(request, cancellationToken))
                {
                    if (stream?.Response != null)
                    {
                        response.Append(stream.Response);
                    }
                }

                return response.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "调用Ollama失败");
                throw new OllamaServiceException("调用Ollama服务失败", ex);
            }
        }
    }

    // 自定义异常
    public class OllamaServiceException : Exception
    {
        public OllamaServiceException(string message) : base(message) { }
        public OllamaServiceException(string message, Exception inner) : base(message, inner) { }
    }
}
