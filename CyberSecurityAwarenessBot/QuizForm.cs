
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CyberSecurityAwarenessBotPart2;

public sealed class QuizForm : Form
{
    private readonly List<QuizQuestion> _questions = new()
    {
        new() {
            Question = "What should you do if an email asks for your password?",
            Options = new() { "Reply with your password", "Delete the email", "Report it as phishing", "Forward it to friends" },
            CorrectAnswer = 2,
            Explanation = "Reporting phishing helps protect others from scams."
        },
        new() {
            Question = "A strong password should contain a mix of letters, numbers and symbols.",
            Options = new() { "True", "False" },
            CorrectAnswer = 0,
            Explanation = "Long, unique passwords are harder to guess."
        },
        new() {
            Question = "What does 2FA stand for?",
            Options = new() { "Two-Factor Authentication", "Two File Access", "Total Firewall Access", "Two-Factor Application" },
            CorrectAnswer = 0,
            Explanation = "2FA adds a second verification step to protect your account."
        },
        new() {
            Question = "Is it safe to use the same password for every account?",
            Options = new() { "True", "False" },
            CorrectAnswer = 1,
            Explanation = "Reusing passwords puts multiple accounts at risk."
        },
        new() {
            Question = "What is phishing?",
            Options = new() { "A computer update", "An attempt to steal information through deception", "A firewall", "A password manager" },
            CorrectAnswer = 1,
            Explanation = "Phishing tricks people into revealing sensitive information."
        },
        new() {
            Question = "What should you do before clicking an unfamiliar link?",
            Options = new() { "Click immediately", "Share it", "Verify its source", "Disable antivirus" },
            CorrectAnswer = 2,
            Explanation = "Always verify suspicious links before opening them."
        },
        new() {
            Question = "Public Wi-Fi is always safe for banking.",
            Options = new() { "True", "False" },
            CorrectAnswer = 1,
            Explanation = "Untrusted public networks can expose you to security risks."
        },
        new() {
            Question = "What is malware?",
            Options = new() { "Helpful software", "Malicious software", "A secure browser", "A backup tool" },
            CorrectAnswer = 1,
            Explanation = "Malware is software designed to harm or compromise systems."
        },
        new() {
            Question = "Why should you update your software?",
            Options = new() { "To remove all files", "To fix security vulnerabilities", "To disable protection", "To share passwords" },
            CorrectAnswer = 1,
            Explanation = "Software updates often contain important security patches."
        },
        new() {
            Question = "Social engineering involves manipulating people to reveal information.",
            Options = new() { "True", "False" },
            CorrectAnswer = 0,
            Explanation = "Social engineering exploits human trust rather than only technical weaknesses."
        },
        new() {
            Question = "What is a password manager used for?",
            Options = new() { "Sharing passwords publicly", "Storing and generating passwords securely", "Disabling 2FA", "Removing antivirus" },
            CorrectAnswer = 1,
            Explanation = "Password managers help create and store unique passwords."
        },
        new() {
            Question = "What should you do if your account is compromised?",
            Options = new() { "Ignore it", "Share your password", "Change your password and secure the account", "Delete your browser" },
            CorrectAnswer = 2,
            Explanation = "Secure the account, revoke suspicious sessions and enable MFA."
        }
    };

    private int _currentQuestion;
    private int _score;
    private bool _answered;

    private readonly Label _progressLabel = new();
    private readonly Label _scoreLabel = new();
    private readonly Label _questionLabel = new();
    private readonly Label _feedbackLabel = new();
    private readonly FlowLayoutPanel _optionsPanel = new();
    private readonly Button _nextButton = new();

    private readonly Color _pink = Color.FromArgb(224, 105, 177);
    private readonly Color _purple = Color.FromArgb(45, 29, 57);

    public QuizForm()
    {
        Text = "Cybersecurity Quiz";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(800, 650);
        MinimumSize = new Size(650, 550);
        BackColor = Color.FromArgb(28, 20, 37);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 11);

        var heading = new Label
        {
            Text = "CYBERSECURITY QUIZ",
            Dock = DockStyle.Top,
            Height = 65,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 170, 220),
            BackColor = _purple
        };

        var statusPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 45,
            BackColor = _purple
        };

        _progressLabel.Dock = DockStyle.Left;
        _progressLabel.Width = 250;
        _progressLabel.TextAlign = ContentAlignment.MiddleCenter;

        _scoreLabel.Dock = DockStyle.Right;
        _scoreLabel.Width = 170;
        _scoreLabel.TextAlign = ContentAlignment.MiddleCenter;

        statusPanel.Controls.Add(_progressLabel);
        statusPanel.Controls.Add(_scoreLabel);

        _questionLabel.Dock = DockStyle.Top;
        _questionLabel.Height = 110;
        _questionLabel.TextAlign = ContentAlignment.MiddleCenter;
        _questionLabel.Font = new Font("Segoe UI", 15, FontStyle.Bold);
        _questionLabel.Padding = new Padding(20);

        _optionsPanel.Dock = DockStyle.Fill;
        _optionsPanel.FlowDirection = FlowDirection.TopDown;
        _optionsPanel.WrapContents = false;
        _optionsPanel.AutoScroll = true;
        _optionsPanel.Padding = new Padding(25);

        _feedbackLabel.Dock = DockStyle.Bottom;
        _feedbackLabel.Height = 90;
        _feedbackLabel.TextAlign = ContentAlignment.MiddleCenter;
        _feedbackLabel.Padding = new Padding(15);

        _nextButton.Text = "Next Question";
        _nextButton.Dock = DockStyle.Bottom;
        _nextButton.Height = 55;
        _nextButton.BackColor = _pink;
        _nextButton.ForeColor = Color.White;
        _nextButton.FlatStyle = FlatStyle.Flat;
        _nextButton.FlatAppearance.BorderSize = 0;
        _nextButton.Enabled = false;
        _nextButton.Click += (_, _) => NextQuestion();

        Controls.Add(_optionsPanel);
        Controls.Add(_feedbackLabel);
        Controls.Add(_nextButton);
        Controls.Add(_questionLabel);
        Controls.Add(statusPanel);
        Controls.Add(heading);

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        _answered = false;
        _nextButton.Enabled = false;
        _feedbackLabel.Text = "";

        var question = _questions[_currentQuestion];

        _progressLabel.Text =
            $"Question {_currentQuestion + 1} of {_questions.Count}";
        _scoreLabel.Text = $"Score: {_score}";
        _questionLabel.Text = question.Question;

        _optionsPanel.Controls.Clear();

        for (int i = 0; i < question.Options.Count; i++)
        {
            int selectedIndex = i;

            var button = new Button
            {
                Text = question.Options[i],
                Width = 660,
                Height = 55,
                Margin = new Padding(5, 8, 5, 8),
                BackColor = _purple,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11)
            };

            button.FlatAppearance.BorderColor = _pink;
            button.Click += (_, _) => CheckAnswer(selectedIndex);

            _optionsPanel.Controls.Add(button);
        }
    }

    private void CheckAnswer(int selectedIndex)
    {
        if (_answered) return;

        _answered = true;
        var question = _questions[_currentQuestion];

        bool correct = selectedIndex == question.CorrectAnswer;

        if (correct)
        {
            _score++;
            _feedbackLabel.ForeColor = Color.LightGreen;
            _feedbackLabel.Text = "Correct! " + question.Explanation;
        }
        else
        {
            _feedbackLabel.ForeColor = Color.FromArgb(255, 170, 190);
            _feedbackLabel.Text =
                "Incorrect. Correct answer: " +
                question.Options[question.CorrectAnswer] +
                ". " + question.Explanation;
        }

        _scoreLabel.Text = $"Score: {_score}";

        foreach (Control control in _optionsPanel.Controls)
        {
            control.Enabled = false;
        }

        _nextButton.Enabled = true;
        _nextButton.Text =
            _currentQuestion == _questions.Count - 1
            ? "View Results"
            : "Next Question";
    }

    private void NextQuestion()
    {
        _currentQuestion++;

        if (_currentQuestion >= _questions.Count)
        {
            ShowResults();
        }
        else
        {
            ShowQuestion();
        }
    }

    private void ShowResults()
    {
        double percentage = _score * 100.0 / _questions.Count;

        string message = percentage >= 80
            ? "Great job! You're a cybersecurity pro!"
            : percentage >= 50
                ? "Good effort! Keep improving your cybersecurity knowledge!"
                : "Keep learning to stay safe online!";

        MessageBox.Show(
            $"Final Score: {_score}/{_questions.Count}\n" +
            $"Percentage: {percentage:F0}%\n\n{message}",
            "Quiz Results",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        Close();
    }
}

