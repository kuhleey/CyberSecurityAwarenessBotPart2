namespace CyberSecurityAwarenessBot;

/// <summary>Part 1 input validation reused by the GUI and engine.</summary>
internal static class Validation
{
    public static bool IsValidInput(string? input) => !string.IsNullOrWhiteSpace(input);
}
