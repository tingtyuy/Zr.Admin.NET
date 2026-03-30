using OllamaSharp;

public interface IOllamaService
{
    /// <summary>
    /// 发送消息并获取完整回复
    /// </summary>
    Task<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送带有系统提示的消息
    /// </summary>
    Task<string> SendMessageWithSystemPromptAsync(string message, string systemPrompt, CancellationToken cancellationToken = default);
}