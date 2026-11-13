using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberSecurityAwarenessBotPart2;

public sealed class TaskManagerForm : Form
{
    private readonly TaskRepository _repository = new();
    private readonly ActivityLogger _activityLogger;

    private readonly TextBox _titleInput = new();
    private readonly TextBox _descriptionInput = new();
    private readonly CheckBox _useReminder = new();
    private readonly DateTimePicker _reminderPicker = new();
    private readonly DataGridView _taskGrid = new();

    private readonly Button _addButton = new();
    private readonly Button _completeButton = new();
    private readonly Button _deleteButton = new();
    private readonly Button _refreshButton = new();

    private readonly Color _pink =
        Color.FromArgb(224, 105, 177);

    private readonly Color _purple =
        Color.FromArgb(45, 29, 57);

    public TaskManagerForm(ActivityLogger activityLogger)
    {
        _activityLogger = activityLogger;
        
        Text = "Cybersecurity Task Assistant";
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(950, 700);
        MinimumSize = new Size(760, 600);
        BackColor = Color.FromArgb(28, 20, 37);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 10);

        var heading = new Label
        {
            Text = "MY CYBERSECURITY TASKS",
            Dock = DockStyle.Top,
            Height = 55,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(
                "Segoe UI", 17, FontStyle.Bold),
            ForeColor = Color.FromArgb(255, 170, 220),
            BackColor = _purple
        };

        var formPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 190,
            Padding = new Padding(15),
            ColumnCount = 2,
            RowCount = 4,
            BackColor = _purple
        };

        formPanel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 130));
        formPanel.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100));

        _titleInput.Dock = DockStyle.Fill;
        _titleInput.PlaceholderText =
            "e.g. Enable two-factor authentication";

        _descriptionInput.Dock = DockStyle.Fill;
        _descriptionInput.PlaceholderText =
            "Describe your cybersecurity task";

        _useReminder.Text = "Set reminder";
        _useReminder.ForeColor = Color.White;
        _useReminder.AutoSize = true;

        _reminderPicker.Format =
            DateTimePickerFormat.Custom;
        _reminderPicker.CustomFormat =
            "yyyy-MM-dd HH:mm";
        _reminderPicker.ShowUpDown = true;
        _reminderPicker.Value = DateTime.Now.AddDays(1);
        _reminderPicker.Enabled = false;
        _reminderPicker.Dock = DockStyle.Fill;

        _useReminder.CheckedChanged += (_, _) =>
        {
            _reminderPicker.Enabled =
                _useReminder.Checked;
        };

        formPanel.Controls.Add(
            MakeLabel("Task title:"), 0, 0);
        formPanel.Controls.Add(_titleInput, 1, 0);

        formPanel.Controls.Add(
            MakeLabel("Description:"), 0, 1);
        formPanel.Controls.Add(_descriptionInput, 1, 1);

        formPanel.Controls.Add(_useReminder, 0, 2);
        formPanel.Controls.Add(_reminderPicker, 1, 2);

        _addButton.Text = "Add Task";
        StyleButton(_addButton, _pink);
        formPanel.Controls.Add(_addButton, 1, 3);

        _taskGrid.Dock = DockStyle.Fill;
        _taskGrid.ReadOnly = true;
        _taskGrid.AllowUserToAddRows = false;
        _taskGrid.AllowUserToDeleteRows = false;
        _taskGrid.SelectionMode =
            DataGridViewSelectionMode.FullRowSelect;
        _taskGrid.MultiSelect = false;
        _taskGrid.AutoSizeColumnsMode =
            DataGridViewAutoSizeColumnsMode.Fill;
        _taskGrid.BackgroundColor =
            Color.FromArgb(245, 240, 248);
        _taskGrid.RowHeadersVisible = false;

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 65,
            Padding = new Padding(12),
            BackColor = _purple
        };

        _completeButton.Text = "Mark Completed";
        _deleteButton.Text = "Delete Task";
        _refreshButton.Text = "Refresh";

        StyleButton(_completeButton, _pink);
        StyleButton(
            _deleteButton, Color.FromArgb(150, 65, 95));
        StyleButton(
            _refreshButton, Color.FromArgb(105, 76, 130));

        buttonPanel.Controls.Add(_completeButton);
        buttonPanel.Controls.Add(_deleteButton);
        buttonPanel.Controls.Add(_refreshButton);

        Controls.Add(_taskGrid);
        Controls.Add(buttonPanel);
        Controls.Add(formPanel);
        Controls.Add(heading);

        _addButton.Click += async (_, _) =>
            await AddTaskAsync();

        _completeButton.Click += async (_, _) =>
            await CompleteSelectedTaskAsync();

        _deleteButton.Click += async (_, _) =>
            await DeleteSelectedTaskAsync();

        _refreshButton.Click += async (_, _) =>
            await LoadTasksAsync();

        Shown += async (_, _) =>
            await LoadTasksAsync();
    }

    private Label MakeLabel(string text)
    {
        return new Label
        {
            Text = text,
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
    }

    private void StyleButton(Button button, Color color)
    {
        button.BackColor = color;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.Width = 145;
        button.Height = 36;
    }

    private async Task LoadTasksAsync()
    {
        try
        {
            var tasks = await _repository.GetTasksAsync();
            _taskGrid.DataSource = null;
            _taskGrid.DataSource = tasks;

            _taskGrid.Columns[nameof(CyberTask.TaskID)]
                .Visible = false;

            _taskGrid.Columns[nameof(CyberTask.Title)]
                .HeaderText = "Task";

            _taskGrid.Columns[nameof(CyberTask.ReminderDate)]
                .HeaderText = "Reminder";

            _taskGrid.Columns[nameof(CyberTask.IsCompleted)]
                .HeaderText = "Completed";
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private async Task AddTaskAsync()
    {
        string title = _titleInput.Text.Trim();
        string description =
            _descriptionInput.Text.Trim();

        if (title.Length == 0 ||
            description.Length == 0)
        {
            MessageBox.Show(
                "Please enter a title and description.");
            return;
        }

        if (title.Length > 150)
        {
            MessageBox.Show(
                "The title cannot exceed 150 characters.");
            return;
        }

        DateTime? reminder = _useReminder.Checked
            ? _reminderPicker.Value
            : null;

        if (reminder.HasValue &&
            reminder.Value <= DateTime.Now)
        {
            MessageBox.Show(
                "Please choose a future reminder date.");
            return;
        }

        try
        {
            var task = new CyberTask
            {
                Title = title,
                Description = description,
                ReminderDate = reminder.HasValue ? reminder.Value : null,
            };
            await _repository.AddTaskAsync(task);
            _activityLogger.LogActivity($"Task created: {task.Title}");

            MessageBox.Show("Task saved successfully!");

            _titleInput.Clear();
            _descriptionInput.Clear();
            _useReminder.Checked = false;

            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private int? GetSelectedTaskId()
    {
        if (_taskGrid.CurrentRow?.DataBoundItem
            is CyberTask task)
        {
            return task.TaskID;
        }

        MessageBox.Show("Please select a task first.");
        return null;
    }

    private async Task CompleteSelectedTaskAsync()
    {
        int? id = GetSelectedTaskId();
        if (id == null) return;

        try
        {
            await _repository.CompleteTaskAsync(id.Value);
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private async Task DeleteSelectedTaskAsync()
    {
        int? id = GetSelectedTaskId();
        if (id == null) return;

        var result = MessageBox.Show(
            "Are you sure you want to delete this task?",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result != DialogResult.Yes) return;

        try
        {
            await _repository.DeleteTaskAsync(id.Value);
            await LoadTasksAsync();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ShowError(Exception ex)
    {
        MessageBox.Show(
            ex.Message,
            "Database Error",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}

