using System;
using System.IO;
using System.Media;

namespace CyberSecurityAwarenessBot;


internal static class VoiceGreeting
{
    public static void PlayGreeting()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Audio", "VoiceGreeting.wav");
        if (!File.Exists(path))
            throw new FileNotFoundException("VoiceGreeting.wav was not copied to the output Audio folder.", path);
        using var player = new SoundPlayer(path);
        player.Load();
        player.PlaySync();
    }
}
