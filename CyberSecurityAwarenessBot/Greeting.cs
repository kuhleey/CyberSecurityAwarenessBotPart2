namespace CyberSecurityAwarenessBot;

/// <summary>Part 1 welcome greeting, displayed in the Windows Forms GUI.</summary>
internal static class Greeting
{
    public static string WelcomeUser(string userName) =>
        $"Hello {userName}! Welcome to the Cybersecurity Awareness Bot. " +
        "I am here to help you learn how to stay safe online.";
}
