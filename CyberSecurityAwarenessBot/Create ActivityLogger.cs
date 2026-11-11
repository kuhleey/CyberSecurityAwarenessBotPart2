
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityAwarenessBotPart2;

public class ActivityLogger
{
    private readonly List<string> _activities = new();

    public void LogActivity(string description)
    {
        string entry =
            $"{DateTime.Now:dd MMM yyyy HH:mm:ss} - {description}";

        _activities.Add(entry);
    }

    public string GetRecentActivities(int count = 10)
    {
        if (_activities.Count == 0)
        {
            return "No activities have been recorded yet.";
        }

        var recent = _activities
            .AsEnumerable()
            .Reverse()
            .Take(count)
            .ToList();

        string result = "Your recent activities:\n\n";

        for (int i = 0; i < recent.Count; i++)
        {
            result += $"{i + 1}. {recent[i]}\n";
        }

        return result;
    }
}

