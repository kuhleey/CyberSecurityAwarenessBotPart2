
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using CyberSecurityAwarenessBot;

namespace CyberSecurityAwarenessBotPart2;

public sealed class MainForm : Form
{
    private readonly ChatBotEngine _bot = new();
    private readonly TaskRepository _taskRepository = new();
    private readonly ActivityLogger _activityLogger = new();

    private readonly RichTextBox _chat = new();
    private readonly TextBox _input = new();

    private readonly Button _sendButton = new();
    private readonly Button _voiceButton = new();
    private readonly TextBox _nameInput = new();
    private readonly Button _greetButton = new();
    private readonly Button _tasksButton = new();
    private readonly Button _quizButton = new();
    private readonly System.Windows.Forms.Timer _reminderTimer = new();
    private bool _checkingReminders = false;

    public MainForm()
    {
        // MAIN WINDOW
        Text = "Cybersecurity Awareness Bot";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(850, 540);
        Size = new Size(1000, 680);

        BackColor = Color.FromArgb(28, 20, 37);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10F);

        // HEADING
        var heading = new Label
        {
            Text = "CYBERSECURITY AWARENESS BOT",
            Dock = DockStyle.Top,
            Height = 54,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(
                "Segoe UI", 16F, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 170, 220),
            BackColor = Color.FromArgb(45, 29, 57)
        };

        // CHAT DISPLAY
        _chat.Dock = DockStyle.Fill;
        _chat.ReadOnly = true;
        _chat.BackColor = Color.FromArgb(245, 240, 248);
        _chat.ForeColor = Color.FromArgb(40, 30, 48);
        _chat.BorderStyle = BorderStyle.None;
        _chat.Font = new Font("Consolas", 10F);
        _chat.DetectUrls = true;

        // ASCII LOGO
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

        // NAME PANEL
        var namePanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(10, 7, 10, 7),
            BackColor = Color.FromArgb(45, 29, 57)
        };

        _nameInput.Dock = DockStyle.Fill;
        _nameInput.PlaceholderText = "Enter your name";

        _greetButton.Text = "Greet me";
        _greetButton.Width = 100;
        _greetButton.Dock = DockStyle.Right;
        _greetButton.BackColor =
            Color.FromArgb(105, 76, 130);
        _greetButton.ForeColor = Color.White;
        _greetButton.FlatStyle = FlatStyle.Flat;
        _greetButton.FlatAppearance.BorderSize = 0;

        namePanel.Controls.Add(_nameInput);
        namePanel.Controls.Add(_greetButton);

        // BOTTOM PANEL
        var bottom = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 104,
            Padding = new Padding(12),
            BackColor = Color.FromArgb(45, 29, 57)
        };

        // MESSAGE INPUT
        _input.Dock = DockStyle.Fill;
        _input.Font = new Font("Segoe UI", 11F);
        _input.PlaceholderText =
            "Type a question or command...";

        // SEND BUTTON
        _sendButton.Text = "Send";
        _sendButton.Width = 90;
        _sendButton.Dock = DockStyle.Right;
        _sendButton.BackColor =
            Color.FromArgb(224, 105, 177);
        _sendButton.ForeColor = Color.White;
        _sendButton.FlatStyle = FlatStyle.Flat;
        _sendButton.FlatAppearance.BorderSize = 0;

        // VOICE BUTTON
        _voiceButton.Text = "Play Greeting";
        _voiceButton.Width = 120;
        _voiceButton.Dock = DockStyle.Right;
        _voiceButton.BackColor =
            Color.FromArgb(105, 76, 130);
        _voiceButton.ForeColor = Color.White;
        _voiceButton.FlatStyle = FlatStyle.Flat;
        _voiceButton.FlatAppearance.BorderSize = 0;

        // TASKS BUTTON
        _tasksButton.Text = "My Tasks";
        _tasksButton.Width = 105;
        _tasksButton.Dock = DockStyle.Right;
        _tasksButton.BackColor =
            Color.FromArgb(150, 80, 170);
        _tasksButton.ForeColor = Color.White;
        _tasksButton.FlatStyle = FlatStyle.Flat;
        _tasksButton.FlatAppearance.BorderSize = 0;

        // QUIZ BUTTON
        _quizButton.Text = "Take Quiz";
        _quizButton.Width = 105;
        _quizButton.Dock = DockStyle.Right;
        _quizButton.BackColor =
            Color.FromArgb(180, 85, 150);
        _quizButton.ForeColor = Color.White;
        _quizButton.FlatStyle = FlatStyle.Flat;
        _quizButton.FlatAppearance.BorderSize = 0;

        // ADD BUTTONS
        bottom.Controls.Add(_input);
        bottom.Controls.Add(_sendButton);
        bottom.Controls.Add(_voiceButton);
        bottom.Controls.Add(_tasksButton);
        bottom.Controls.Add(_quizButton);

        // ADD MAIN CONTROLS
        Controls.Add(_chat);
        Controls.Add(bottom);
        Controls.Add(namePanel);
        Controls.Add(logo);
        Controls.Add(heading);

        // EVENTS
        _sendButton.Click += (_, _) => SendMessage();

        _input.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        };

        _voiceButton.Click += (_, _) => PlayGreeting();
        _greetButton.Click += (_, _) => GreetUser();


        _tasksButton.Click += (_, _) =>
        {
            _activityLogger.LogActivity(
                "Task Manager opened using GUI button");

            using var taskWindow = new TaskManagerForm();
            taskWindow.ShowDialog(this);
        };



        _quizButton.Click += (_, _) =>
        {
            _activityLogger.LogActivity(
                "Cybersecurity quiz opened using GUI button");

            using var quizWindow = new QuizForm();
            quizWindow.ShowDialog(this);
        };


        Shown += MainForm_Shown;
    }

    // STARTUP
    private async void MainForm_Shown(
        object? sender, EventArgs e)
    {
        AddBotMessage(_bot.WelcomeMessage);
        await TestDatabaseConnectionAsync();
    }

    // MYSQL CONNECTION TEST
    private async Task TestDatabaseConnectionAsync()
    {
        try
        {
            using var connection =
                DatabaseConnection.GetConnection();

            await connection.OpenAsync();

            AddBotMessage(
                "Database connection established successfully."
            );
        }
        catch (Exception ex)
        {
            AddBotMessage(
                "Unable to connect to the database."
            );

            MessageBox.Show(
                "Database connection failed:\n\n" +
                ex.Message,
                "Database Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    // SEND MESSAGE WITH NLP
    private async void SendMessage()
    {
        string message = _input.Text.Trim();

        if (message.Length == 0)
        {
            AddBotMessage("Please type a message.");
            _input.Focus();
            return;
        }

        AddUserMessage(message);
        _input.Clear();

        _sendButton.Enabled = false;

        try
        {
            BotCommand command =
                NlpCommandProcessor.Recognise(message);

            switch (command.Type)
            {
                case BotCommandType.AddTask:
                    {
                        if (string.IsNullOrWhiteSpace(
                            command.TaskTitle))
                        {
                            AddBotMessage(
                                "Please tell me what task to add.");
                            break;
                        }

                        if (command.TaskTitle.Length > 150)
                        {
                            AddBotMessage(
                                "Please use a shorter task title.");
                            break;
                        }

                        var task = new CyberTask
                        {
                            Title = command.TaskTitle,
                            Description =
                                "Created through chatbot command",
                            ReminderDate = command.ReminderDate
                        };

                        await _taskRepository.AddTaskAsync(task);
                        _activityLogger.LogActivity(
    $"Task added: {task.Title}");

                        AddBotMessage(
                            $"Task added successfully: {task.Title}");

                        if (task.ReminderDate.HasValue)
                        {
                            AddBotMessage(
                                "Reminder saved for " +
                                task.ReminderDate.Value
                                    .ToString("dd MMM yyyy HH:mm"));
                        }

                        break;
                    }

                case BotCommandType.ViewTasks:
                    {
                        AddBotMessage("Opening your tasks...");

                        using var taskWindow =
                            new TaskManagerForm();

                        taskWindow.ShowDialog(this);
                        break;
                    }

                case BotCommandType.OpenQuiz:
                    {
                        AddBotMessage(
                            "Let's test your cybersecurity knowledge!");

                        using var quizWindow = new QuizForm();
                        quizWindow.ShowDialog(this);
                        _activityLogger.LogActivity(
    "Cybersecurity quiz opened");
                        break;

                    }

                case BotCommandType.ViewActivity:
                    {
                        AddBotMessage(
                            _activityLogger.GetRecentActivities(10)
                        );
                        break;
                    }


                default:
                    {
                        
                        string response =
                            _bot.GetResponse(message);

                        AddBotMessage(response);

                        if (message.Equals(
                            "exit",
                            StringComparison.OrdinalIgnoreCase)
                            || message.Equals(
                            "quit",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            AddBotMessage(
                                "You can close this window " +
                                "whenever you're ready.");
                        }

                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            AddBotMessage(
                "Sorry, I couldn't complete that action.");

            MessageBox.Show(
                ex.Message,
                "Chatbot Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        finally
        {
            _sendButton.Enabled = true;
            _input.Focus();
        }
    }

    // USER MESSAGE
    private void AddUserMessage(string message)
    {
        AppendMessage(
            "YOU",
            message,
            Color.FromArgb(85, 50, 125)
        );
    }

    // BOT MESSAGE
    private void AddBotMessage(string message)
    {
        AppendMessage(
            "BOT",
            message,
            Color.FromArgb(160, 50, 112)
        );
    }

    // FORMAT CHAT MESSAGES
    private void AppendMessage(
        string speaker,
        string message,
        Color color)
    {
        _chat.SelectionStart = _chat.TextLength;
        _chat.SelectionLength = 0;

        _chat.SelectionColor = color;
        _chat.SelectionFont =
            new Font(_chat.Font, FontStyle.Bold);

        _chat.AppendText(speaker + ": ");

        _chat.SelectionColor =
            Color.FromArgb(40, 30, 48);

        _chat.SelectionFont =
            new Font(_chat.Font, FontStyle.Regular);

        _chat.AppendText(
            message + Environment.NewLine +
            Environment.NewLine
        );

        _chat.SelectionStart = _chat.TextLength;
        _chat.ScrollToCaret();
    }

    // GREET USER
    private void GreetUser()
    {
        string name = _nameInput.Text.Trim();

        if (!Validation.IsValidInput(name))
        {
            AddBotMessage(
                "Please enter your name before selecting Greet me."
            );
            return;
        }

        AddBotMessage(Greeting.WelcomeUser(name));

        AddBotMessage(
            _bot.GetResponse("my name is " + name)
        );
    }

    // PLAY VOICE GREETING
    private void PlayGreeting()
    {
        try
        {
            VoiceGreeting.PlayGreeting();

            AddBotMessage(
                "Voice greeting played successfully."
            );
        }
        catch (Exception ex)
        {
            AddBotMessage(
                "Unable to play voice greeting: " +
                ex.Message
            );
        }
    }
}






