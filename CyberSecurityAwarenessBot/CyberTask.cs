
using System;

namespace CyberSecurityAwarenessBotPart2;

public class CyberTask
{
    public int TaskID { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public DateTime? ReminderDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}

