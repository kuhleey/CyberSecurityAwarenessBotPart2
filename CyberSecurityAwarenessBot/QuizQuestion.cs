
using System.Collections.Generic;

namespace CyberSecurityAwarenessBotPart2;

public class QuizQuestion
{
    public string Question { get; set; } = "";
    public List<string> Options { get; set; } = new();
    public int CorrectAnswer { get; set; }
    public string Explanation { get; set; } = "";
}
