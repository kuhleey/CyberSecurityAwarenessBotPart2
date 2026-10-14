using System;
using System.Collections.Generic;
using System.Linq;
using CyberSecurityAwarenessBot;

namespace CyberSecurityAwarenessBotPart2;

/// <summary>Contains the chatbot's conversation logic, memory, keyword matching and sentiment detection.</summary>
public sealed class ChatBotEngine
{
    private readonly Random _random = new();
    private string? _lastTopic;
    private string? _lastTip;
    private readonly Dictionary<string, List<string>> _keywordResponses = new(StringComparer.OrdinalIgnoreCase)
    {
        ["phishing"] = new() {
            "Phishing messages try to trick you into sharing information. Check the sender and destination link before clicking.",
            "Cybersecurity tip: be suspicious of urgent messages asking you to verify an account or open an attachment.",
            "Look for misspellings, unexpected attachments and links that do not match the official website."
        },
        ["password"] = new() {
            "Use a long, unique password for every account. A password manager can help you keep track of them.",
            "Turn on multi-factor authentication wherever possible, and never share a one-time code with anyone.",
            "Avoid using personal details in passwords. A long passphrase is usually easier to remember and harder to guess."
        },
        ["malware"] = new() {
            "Malware is harmful software. Keep your operating system updated and download apps only from trusted sources.",
            "Do not open unexpected attachments. If you suspect malware, disconnect from the network and contact trusted IT support.",
            "Use reputable security software and keep it updated. Be cautious with cracked software and unknown USB drives."
        },
        ["safe browsing"] = new() {
            "For safer browsing, check the website address carefully, keep your browser updated and avoid suspicious downloads.",
            "HTTPS helps protect a connection, but it does not automatically mean a website is trustworthy.",
            "Use official websites for downloads and avoid entering sensitive information on links sent by strangers."
        },
        ["privacy"] = new() {
            "Share less personal information publicly, review app permissions and use privacy settings on your accounts.",
            "Before posting, consider whether the information could help someone guess your passwords or security questions."
        },
        ["two-factor"] = new() {
            "Two-factor authentication adds another check beyond your password. An authenticator app or security key is often safer than SMS.",
            "Enable multi-factor authentication on your email first, because email can be used to reset other accounts."
        },
        ["scam"] = new() {
            "Pause before responding to unexpected requests for money or personal details. Verify the request through a separate trusted channel.",
            "Scammers often create urgency. Do not let a countdown or threat pressure you into clicking or paying."
        }
    };

    private readonly Dictionary<string, string> _memory = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string[]> _sentimentResponses = new(StringComparer.OrdinalIgnoreCase)
    {
        ["worried"] = new[] {
            "It's understandable to feel worried. Take your time, and don't click anything you're unsure about. A safe first step is to verify the message through an official channel.",
            "That sounds stressful. You do not need to act immediately. Pause, avoid sharing information, and ask a trusted person or IT support to check it."
        },
        ["frustrated"] = new[] {
            "Cybersecurity can feel overwhelming. Let's make it smaller: start by checking the sender, the link, and whether the request was expected.",
            "I hear you. One step at a time: don't click the link yet, and verify the request using the organisation's official contact details."
        },
        ["curious"] = new[] {
            "Great question! A useful way to learn is to explore one topic at a time and practise with safe, legal examples.",
            "Curiosity is a good cybersecurity habit. Keep asking questions, and only test systems you own or have permission to assess."
        }
    };

    public string WelcomeMessage =>
        "CYBERSECURITY AWARENESS BOT\n" +
        "  .-----------------------.\n" +
        "  |  THINK BEFORE YOU CLICK |\n" +
        "  '-----------------------'\n\n" +
        "Hello! I'm your Cybersecurity Awareness Bot. I can help with phishing, passwords, malware, safe browsing, privacy and scams.\n" +
        "You can also tell me your name or your cybersecurity interest, and I will try to remember it during this session. Type 'help' for examples or 'exit' to close the chat.";

    public string GetResponse(string rawInput)
    {
        string input = rawInput?.Trim() ?? string.Empty;
        if (!Validation.IsValidInput(input))
            return "Please type a message or question so I can help.";

        string lower = input.ToLowerInvariant();
        if (lower is "exit" or "quit" or "bye")
            return "Goodbye! Remember: pause, verify, and protect your information.";
        if (lower == "help")
            return "Try asking about phishing, passwords, malware, safe browsing, privacy, two-factor authentication or scams. You can also say 'my name is ...' or 'I am interested in ...'.";
        if (lower.Contains("what is my name") || lower.Contains("remember my name"))
            return _memory.TryGetValue("name", out string? name)
                ? $"Your name is {name}. I'll remember it for this chat session."
                : "You haven't told me your name yet. Try: 'my name is Kuhle'.";
        if (lower.Contains("what am i interested in") || lower.Contains("what is my interest") || lower.Contains("remember my interest"))
            return _memory.TryGetValue("interest", out string? interest)
                ? $"You told me you're interested in {interest}."
                : "You haven't told me your cybersecurity interest yet. Try: 'I am interested in ethical hacking'.";

        if (lower.StartsWith("my name is "))
        {
            string name = input[11..].Trim().Trim('.', '!', '?');
            if (name.Length == 0) return "Please include your name after 'my name is'.";
            _memory["name"] = name;
            return $"Nice to meet you, {name}! I'll remember your name while this chat is open.";
        }
        if (lower.StartsWith("i am interested in ") || lower.StartsWith("i'm interested in "))
        {
            int split = lower.StartsWith("i am interested in ") ? "i am interested in ".Length : "i'm interested in ".Length;
            string interest = input[split..].Trim().Trim('.', '!', '?');
            if (interest.Length == 0) return "Please tell me what you're interested in.";
            _memory["interest"] = interest;
            return $"I'll remember that you're interested in {interest}. I can help you explore it safely.";
        }

        // Identify the cybersecurity topic before detecting mood so a worried user
        // receives both empathy AND an actionable tip without needing to ask again.
        string? topic = FindTopic(lower);
        string? sentiment = DetectSentiment(lower);
        bool followUp = new[] { "another tip", "another one", "tell me more", "explain more", "give me more", "more details", "what else" }
            .Any(lower.Contains);
        if (followUp && topic is null) topic = _lastTopic;
        if (topic is not null)
        {
            _lastTopic = topic;
            string tip = PickTopicTip(topic, followUp);
            _lastTip = tip;
            string empathy = sentiment is null ? "" : RandomFrom(_sentimentResponses[sentiment]) + "\n\n";
            string personal = _memory.TryGetValue("interest", out string? interest) &&
                              topic.Contains(interest, StringComparison.OrdinalIgnoreCase)
                ? $"Since you're interested in {interest}: " : "";
            return empathy + personal + tip;
        }
        if (followUp)
            return "Which topic would you like another tip about? Try phishing, password, scam or privacy.";
        if (sentiment is not null)
            return RandomFrom(_sentimentResponses[sentiment]) + "\n\n" +
                   "You can ask me about phishing, passwords, scams or privacy for a practical tip.";
        // Part 1's general responses remain available through the shared chatbot.
        if (lower.Contains("how are you") || lower.Contains("purpose") ||
            lower.Contains("what do you do") || lower.Contains("what can i ask") ||
            lower.Contains("suspicious link") || lower.Contains("unknown link"))
            return ChatBot.GetPart1Response(input);

        if (lower.Contains("hello") || lower == "hi" || lower.StartsWith("hey"))
        {
            string greeting = _memory.TryGetValue("name", out string? name) ? $"Hi again, {name}!" : "Hello!";
            return greeting + " What cybersecurity topic would you like to explore?";
        }

        return "I'm not sure I understood that. Try a keyword such as phishing, password, malware, safe browsing, privacy or scam, or type 'help' to see examples.";
    }

    private string? FindTopic(string lower)
    {
        if (lower.Contains("phish")) return "phishing";
        if (lower.Contains("password") || lower.Contains("passphrase")) return "password";
        if (lower.Contains("malware") || lower.Contains("virus")) return "malware";
        if (lower.Contains("privacy") || lower.Contains("private") || lower.Contains("personal data")) return "privacy";
        if (lower.Contains("scam") || lower.Contains("fraud")) return "scam";
        if (lower.Contains("two-factor") || lower.Contains("two factor") || lower.Contains("multi-factor") || lower.Contains("mfa")) return "two-factor";
        if (lower.Contains("safe browsing") || lower.Contains("browse safely") || lower.Contains("website") || lower.Contains("link")) return "safe browsing";
        return null;
    }

    private string PickTopicTip(string topic, bool avoidRepeat)
    {
        var choices = _keywordResponses[topic];
        if (choices.Count == 1) return choices[0];
        var eligible = avoidRepeat ? choices.Where(tip => tip != _lastTip).ToList() : choices;
        return RandomFrom(eligible);
    }

    private string? DetectSentiment(string input)
    {
        if (new[] { "scared", "worried", "anxious", "afraid", "panic", "nervous", "unsafe" }.Any(input.Contains)) return "worried";
        if (new[] { "frustrated", "annoyed", "confused", "overwhelmed", "angry", "difficult" }.Any(input.Contains)) return "frustrated";
        if (new[] { "curious", "interested", "wonder", "want to learn", "excited" }.Any(input.Contains)) return "curious";
        return null;
    }

    private string RandomFrom(IReadOnlyList<string> choices) => choices[_random.Next(choices.Count)];
}
