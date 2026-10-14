namespace CyberSecurityAwarenessBot;


internal static class Validation
{
    public static bool IsValidInput(string? input) => !string.IsNullOrWhiteSpace(input);
}
