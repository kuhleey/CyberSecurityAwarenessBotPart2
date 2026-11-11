
using System;
using System.Text.RegularExpressions;

namespace CyberSecurityAwarenessBotPart2;

public enum BotCommandType
{
    None,
    AddTask,
    ViewTasks,
    OpenQuiz,
    ViewActivity
}

public class BotCommand
{
    public BotCommandType Type { get; set; }
    public string TaskTitle { get; set; } = "";
    public DateTime? ReminderDate { get; set; }
}

public static class NlpCommandProcessor
{
    public static BotCommand Recognise(string input)
    {
        string text = input.Trim().ToLowerInvariant();

        // Remove extra spaces
        text = Regex.Replace(text, @"\s+", " ");

        // ACTIVITY LOG
        if (Regex.IsMatch(text,
            @"\b(activity log|recent activity|what have you done)\b"))
        {
            return new BotCommand
            {
                Type = BotCommandType.ViewActivity
            };
        }

        // QUIZ COMMANDS
        if (Regex.IsMatch(text,
            @"\b(quiz|mini.game|test my knowledge)\b"))
        {
            return new BotCommand
            {
                Type = BotCommandType.OpenQuiz
            };
        }

        // VIEW TASKS
        if (Regex.IsMatch(text,
            @"\b(show|view|list|see|display)\b") &&
            Regex.IsMatch(text, @"\b(tasks|reminders)\b"))
        {
            return new BotCommand
            {
                Type = BotCommandType.ViewTasks
            };
        }

        // ADD TASK OR REMINDER
        Match match = Regex.Match(text,
            @"\b(?:remind me to|remember to|add (?:a )?task to|create (?:a )?task to|i need to)\s+(.+)",
            RegexOptions.IgnoreCase);

        if (match.Success)
        {
            string title = match.Groups[1].Value.Trim();
            DateTime? reminder = null;

            // Recognise simple relative dates
            if (Regex.IsMatch(title, @"\btomorrow\b"))
            {
                reminder = DateTime.Today
                    .AddDays(1)
                    .AddHours(9);

                title = Regex.Replace(title,
                    @"\s+tomorrow\b", "",
                    RegexOptions.IgnoreCase);
            }
            else if (Regex.IsMatch(title, @"\btoday\b"))
            {
                reminder = DateTime.Now.AddHours(1);

                title = Regex.Replace(title,
                    @"\s+today\b", "",
                    RegexOptions.IgnoreCase);
            }
            else if (Regex.IsMatch(title, @"\bnext week\b"))
            {
                reminder = DateTime.Today
                    .AddDays(7)
                    .AddHours(9);

                title = Regex.Replace(title,
                    @"\s+next week\b", "",
                    RegexOptions.IgnoreCase);
            }

            title = title.Trim(' ', '.', '!', '?');

            return new BotCommand
            {
                Type = BotCommandType.AddTask,
                TaskTitle = title,
                ReminderDate = reminder
            };
        }

        return new BotCommand
        {
            Type = BotCommandType.None
        };
    }
}

