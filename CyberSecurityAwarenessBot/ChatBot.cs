namespace CyberSecurityAwarenessBot;

/// <summary>Part 1 chatbot adapter used by the combined GUI application.</summary>
internal static class ChatBot
{
    public static string GetPart1Response(string input) => Responses.GetResponse(input);
}
