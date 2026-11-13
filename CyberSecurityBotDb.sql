

CREATE TABLE IF NOT EXISTS CyberTasks (
    TaskID INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(150) NOT NULL,
    Description TEXT NOT NULL,
    ReminderDate DATETIME NULL,
    IsCompleted BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);
 
INSERT INTO CyberTasks
    (Title, Description, ReminderDate)
VALUES
    (
        'Enable two-factor authentication',
        'Secure my account using 2FA',
        '2026-11-15 09:00:00'
    );SELECT * FROM CyberTasks;
UPDATE CyberTasks
SET IsCompleted = TRUE
WHERE TaskID = 1;

SELECT * FROM CyberTasks;
USE CyberSecurityBotDB;

SELECT TaskID, Title, ReminderDate, IsCompleted
FROM CyberTasks
ORDER BY TaskID DESC;

USE CyberSecurityBotDB;

ALTER TABLE CyberTasks
ADD COLUMN ReminderShown BOOLEAN NOT NULL DEFAULT FALSE;
USE CyberSecurityBotDB;

SELECT
    TaskID,
    Title,
    ReminderDate,
    ReminderShown
FROM CyberTasks
ORDER BY TaskID DESC;
USE CyberSecurityBotDB;

DESCRIBE CyberTasks;

USE CyberSecurityBotDB;