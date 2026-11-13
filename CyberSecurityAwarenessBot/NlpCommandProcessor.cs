
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
        string text = Regex.Replace(
            input.Trim().ToLowerInvariant(),
            @"\s+", " ");

        // ACTIVITY LOG
        if (Regex.IsMatch(text,
            @"\b(activity log|recent activity|what have you done)\b"))
        {
            return new BotCommand
            {
                Type = BotCommandType.ViewActivity
            };
        }

        // QUIZ
        if (Regex.IsMatch(text,
            @"\b(quiz|quizzes|mini.game)\b") ||
            text.Contains("test my knowledge") ||
            text.Contains("test me"))
        {
            return new BotCommand
            {
                Type = BotCommandType.OpenQuiz
            };
        }

        // VIEW TASKS
        if (Regex.IsMatch(text,
            @"\b(show|view|list|see|display)\b") &&
            Regex.IsMatch(text,
            @"\b(tasks|reminders)\b"))
        {
            return new BotCommand
            {
                Type = BotCommandType.ViewTasks
            };
        }

        // CREATE TASK OR REMINDER
        Match match = Regex.Match(
            text,
            @"\b(?:remind me to|" +
            @"remind me about|" +
            @"set (?:a )?reminder to|" +
            @"set (?:a )?reminder for|" +
            @"remember to|" +
            @"add (?:a )?task to|" +
            @"create (?:a )?task to|" +
            @"i need to)\s+(.+)",
            RegexOptions.IgnoreCase
        );

        if (match.Success)
        {
            string title = match.Groups[1].Value.Trim();
            DateTime? reminder = null;

            // TOMORROW
            if (Regex.IsMatch(title, @"\btomorrow\b"))
            {
                reminder = DateTime.Today
                    .AddDays(1)
                    .AddHours(9);

                title = Regex.Replace(
                    title,
                    @"\s+tomorrow\b",
                    "",
                    RegexOptions.IgnoreCase);
            }

            // TODAY
            else if (Regex.IsMatch(title, @"\btoday\b"))
            {
                reminder = DateTime.Now.AddHours(1);

                title = Regex.Replace(
                    title,
                    @"\s+today\b",
                    "",
                    RegexOptions.IgnoreCase);
            }

            // NEXT WEEK
            else if (Regex.IsMatch(title, @"\bnext week\b"))
            {
                reminder = DateTime.Today
                    .AddDays(7)
                    .AddHours(9);

                title = Regex.Replace(
                    title,
                    @"\s+next week\b",
                    "",
                    RegexOptions.IgnoreCase);
            }

            // IN X MINUTES
            else
            {
                Match minutes = Regex.Match(
                    title,
                    @"\bin (\d+) minutes?\b");

                if (minutes.Success)
                {
                    int amount = int.Parse(
                        minutes.Groups[1].Value);

                    reminder = DateTime.Now.AddMinutes(amount);

                    title = title.Replace(
                        minutes.Value, "");
                }
                else
                {
                    // IN X HOURS
                    Match hours = Regex.Match(
                        title,
                        @"\bin (\d+) hours?\b");

                    if (hours.Success)
                    {
                        int amount = int.Parse(
                            hours.Groups[1].Value);

                        reminder = DateTime.Now.AddHours(amount);

                        title = title.Replace(
                            hours.Value, "");
                    }
                }
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


