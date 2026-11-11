
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

namespace CyberSecurityAwarenessBotPart2;

public class TaskRepository
{
    public async Task AddTaskAsync(CyberTask task)
    {
        using var connection =
            DatabaseConnection.GetConnection();

        await connection.OpenAsync();

        const string sql = """
            INSERT INTO CyberTasks
                (Title, Description, ReminderDate)
            VALUES
                (@title, @description, @reminder);
            """;

        using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "@title", task.Title);

        command.Parameters.AddWithValue(
            "@description", task.Description);

        command.Parameters.AddWithValue(
            "@reminder",
            (object?)task.ReminderDate ?? DBNull.Value);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<CyberTask>> GetTasksAsync()
    {
        var tasks = new List<CyberTask>();

        using var connection =
            DatabaseConnection.GetConnection();

        await connection.OpenAsync();

        const string sql = """
            SELECT TaskID, Title, Description,
                   ReminderDate, IsCompleted, CreatedAt
            FROM CyberTasks
            ORDER BY CreatedAt DESC, TaskID DESC;
            """;

        using var command =
            new MySqlCommand(sql, connection);

        using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            tasks.Add(new CyberTask
            {
                TaskID = reader.GetInt32("TaskID"),
                Title = reader.GetString("Title"),
                Description =
                    reader.GetString("Description"),
                ReminderDate =
                    reader.IsDBNull(
                        reader.GetOrdinal("ReminderDate"))
                    ? null
                    : reader.GetDateTime("ReminderDate"),
                IsCompleted =
                    reader.GetBoolean("IsCompleted"),
                CreatedAt =
                    reader.GetDateTime("CreatedAt")
            });
        }

        return tasks;
    }

    public async Task CompleteTaskAsync(int taskId)
    {
        using var connection =
            DatabaseConnection.GetConnection();

        await connection.OpenAsync();

        const string sql = """
            UPDATE CyberTasks
            SET IsCompleted = TRUE
            WHERE TaskID = @id;
            """;

        using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", taskId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        using var connection =
            DatabaseConnection.GetConnection();

        await connection.OpenAsync();

        const string sql = """
            DELETE FROM CyberTasks
            WHERE TaskID = @id;
            """;

        using var command =
            new MySqlCommand(sql, connection);

        command.Parameters.AddWithValue("@id", taskId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<CyberTask>> GetDueRemindersAsync()
    {
        var reminders = new List<CyberTask>();

        using var connection = DatabaseConnection.GetConnection();
        await connection.OpenAsync();

        const string sql = @"
        SELECT TaskID, Title, Description, ReminderDate,
               IsCompleted, CreatedAt
        FROM CyberTasks
        WHERE ReminderDate IS NOT NULL
          AND ReminderDate <= NOW()
          AND ReminderShown = FALSE
          AND IsCompleted = FALSE
        ORDER BY ReminderDate;";

        using var command = new MySqlCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            reminders.Add(new CyberTask
            {
                TaskID = reader.GetInt32("TaskID"),
                Title = reader.GetString("Title"),
                Description = reader.GetString("Description"),
                ReminderDate = reader.IsDBNull(
                    reader.GetOrdinal("ReminderDate"))
                    ? null
                    : reader.GetDateTime("ReminderDate"),
                IsCompleted = reader.GetBoolean("IsCompleted"),
                CreatedAt = reader.GetDateTime("CreatedAt")
            });
        }

        return reminders;
    }

    public async Task MarkReminderShownAsync(int taskId)
    {
        using var connection = DatabaseConnection.GetConnection();
        await connection.OpenAsync();

        const string sql = @"
        UPDATE CyberTasks
        SET ReminderShown = TRUE
        WHERE TaskID = @TaskID
          AND ReminderShown = FALSE;";

        using var command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@TaskID", taskId);

        await command.ExecuteNonQueryAsync();
    }

}

