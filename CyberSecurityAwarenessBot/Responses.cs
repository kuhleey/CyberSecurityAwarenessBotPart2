using System;

namespace CyberSecurityAwarenessBot
{
    internal class Responses
    {
        public static string GetResponse(string input)
        {
            // Responses to common questions about cybersecurity awareness
            input = input?.ToLower() ?? string.Empty;

            if (input.Contains("how are you"))
            {
                return "I'm doing great! I'm ready to help you learn about cybersecurity.";
            }
            else if (input.Contains("purpose") || input.Contains("what do you do"))
            {
                return "My purpose is to educate South African citizens about cybersecurity threats and help them stay safe online.";
            }
            else if (input.Contains("what can i ask") || input.Contains("help"))
            {
                return "You can ask me about password safety, phishing, suspicious links, safe browsing, and online security.";
            }
            else if (input.Contains("password"))
            {
                return "A strong password should be long, unique, and contain a mixture of uppercase letters, lowercase letters, numbers, and symbols. Avoid using the same password everywhere.";
            }
            else if (input.Contains("phishing"))
            {
                return "Phishing is a cyberattack where criminals pretend to be trusted organisations to trick you into sharing personal information like passwords or banking details.";
            }
            else if (input.Contains("link") || input.Contains("website"))
            {
                return "Before clicking links, check the website address carefully. Avoid suspicious links from unknown emails or messages.";
            }

            return "I'm sorry, I don't understand your question. Please ask me something about cybersecurity awareness.";
        }
    }
}





