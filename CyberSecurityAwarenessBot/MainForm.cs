using System;
using System.Drawing;
using System.IO;
using CyberSecurityAwarenessBot;
using System.Windows.Forms;

namespace CyberSecurityAwarenessBotPart2;

public sealed class MainForm : Form
{
    private readonly ChatBotEngine _bot = new();
    private readonly RichTextBox _chat = new();
    private readonly TextBox _input = new();
    private readonly Button _sendButton = new();
    private readonly Button _voiceButton = new();
    private readonly TextBox _nameInput = new();
    private readonly Button _greetButton = new();

    public MainForm()
    {
        Text = "Cybersecurity Awareness Bot";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(720, 540);
        Size = new Size(900, 680);
        BackColor = Color.FromArgb(28, 20, 37);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F);

        var heading = new Label
        {
            Text = "CYBERSECURITY AWARENESS BOT",
            Dock = DockStyle.Top,
            Height = 54,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 170, 220),
            BackColor = Color.FromArgb(45, 29, 57)
        };
        _chat.Dock = DockStyle.Fill;
        _chat.ReadOnly = true;
        _chat.BackColor = Color.FromArgb(245, 240, 248);
        _chat.ForeColor = Color.FromArgb(40, 30, 48);
        _chat.BorderStyle = BorderStyle.None;
        _chat.Font = new Font("Consolas", 10F);
        _chat.DetectUrls = true;

        var logo = new Label
        {
            Text = AsciiArt.Logo,
            Dock = DockStyle.Top,
            Height = 168,
            Font = new Font("Consolas", 8F),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(204, 226, 255),
            BackColor = Color.FromArgb(35, 24, 49)
        };

        var namePanel = new Panel { Dock = DockStyle.Top, Height = 48, Padding = new Padding(10, 7, 10, 7), BackColor = Color.FromArgb(45, 29, 57) };
        _nameInput.Dock = DockStyle.Fill;
        _nameInput.PlaceholderText = "Enter your name";
        _greetButton.Text = "Greet me";
        _greetButton.Width = 100;
        _greetButton.Dock = DockStyle.Right;
        namePanel.Controls.Add(_nameInput);
        namePanel.Controls.Add(_greetButton);

        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 104, Padding = new Padding(12), BackColor = Color.FromArgb(45, 29, 57) };
        _input.Dock = DockStyle.Fill;
        _input.Font = new Font("Segoe UI", 11F);
        _input.PlaceholderText = "Type your question here… (try: phishing, password, or help)";
        _sendButton.Text = "Send";
        _sendButton.Width = 90;
        _sendButton.Dock = DockStyle.Right;
        _sendButton.BackColor = Color.FromArgb(224, 105, 177);
        _sendButton.ForeColor = Color.White;
        _sendButton.FlatStyle = FlatStyle.Flat;
        _sendButton.FlatAppearance.BorderSize = 0;
        _voiceButton.Text = "Play greeting";
        _voiceButton.Width = 115;
        _voiceButton.Dock = DockStyle.Right;
        _voiceButton.BackColor = Color.FromArgb(105, 76, 130);
        _voiceButton.ForeColor = Color.White;
        _voiceButton.FlatStyle = FlatStyle.Flat;
        _voiceButton.FlatAppearance.BorderSize = 0;

        bottom.Controls.Add(_input);
        bottom.Controls.Add(_sendButton);
        bottom.Controls.Add(_voiceButton);
        Controls.Add(_chat);
        Controls.Add(bottom);
        Controls.Add(namePanel);
        Controls.Add(logo);
        Controls.Add(heading);

        _sendButton.Click += (_, _) => SendMessage();
        _input.KeyDown += (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; SendMessage(); } };
        _voiceButton.Click += (_, _) => PlayGreeting();
        _greetButton.Click += (_, _) => GreetUser();
        Shown += (_, _) => AddBotMessage(_bot.WelcomeMessage);
    }

    private void SendMessage()
    {
        string message = _input.Text.Trim();
        if (message.Length == 0)
        {
            AddBotMessage("Please type a message before pressing Send.");
            _input.Focus();
            return;
        }

        AddUserMessage(message);
        _input.Clear();
        string response = _bot.GetResponse(message);
        AddBotMessage(response);
        if (message.Equals("exit", StringComparison.OrdinalIgnoreCase) || message.Equals("quit", StringComparison.OrdinalIgnoreCase))
        {
            AddBotMessage("You can close this window whenever you're ready.");
        }
        _input.Focus();
    }

    private void AddUserMessage(string message) => AppendMessage("YOU", message, Color.FromArgb(85, 50, 125));
    private void AddBotMessage(string message) => AppendMessage("BOT", message, Color.FromArgb(160, 50, 112));

    private void AppendMessage(string speaker, string message, Color color)
    {
        _chat.SelectionStart = _chat.TextLength;
        _chat.SelectionLength = 0;
        _chat.SelectionColor = color;
        _chat.SelectionFont = new Font(_chat.Font, FontStyle.Bold);
        _chat.AppendText(speaker + ": ");
        _chat.SelectionColor = Color.FromArgb(40, 30, 48);
        _chat.SelectionFont = new Font(_chat.Font, FontStyle.Regular);
        _chat.AppendText(message + Environment.NewLine + Environment.NewLine);
        _chat.SelectionStart = _chat.TextLength;
        _chat.ScrollToCaret();
    }

    private void GreetUser()
    {
        string name = _nameInput.Text.Trim();
        if (!Validation.IsValidInput(name))
        {
            AddBotMessage("Please enter your name before selecting Greet me.");
            return;
        }
        AddBotMessage(Greeting.WelcomeUser(name));
        AddBotMessage(_bot.GetResponse("my name is " + name));
    }

    private void PlayGreeting()
    {
        try
        {
            VoiceGreeting.PlayGreeting();
            AddBotMessage("Voice greeting played successfully.");
        }
        catch (Exception ex)
        {
            AddBotMessage("Unable to play voice greeting: " + ex.Message);
        }
    }
}
