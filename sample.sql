-- Sample SQL Server script for a simple task tracker database.
-- Run this in SQL Server Management Studio, Azure Data Studio, or sqlcmd.

IF DB_ID(N'TaskTrackerSample') IS NULL
BEGIN
    CREATE DATABASE TaskTrackerSample;
END;
GO

USE TaskTrackerSample;
GO

IF OBJECT_ID(N'dbo.Tasks', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Tasks;
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Users;
END;
GO

CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) CONSTRAINT PK_Users PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL CONSTRAINT UQ_Users_Email UNIQUE,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE dbo.Tasks (
    Id INT IDENTITY(1,1) CONSTRAINT PK_Tasks PRIMARY KEY,
    UserId INT NOT NULL,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(500) NULL,
    Priority NVARCHAR(20) NOT NULL,
    IsComplete BIT NOT NULL CONSTRAINT DF_Tasks_IsComplete DEFAULT 0,
    DueDate DATE NULL,
    CreatedAt DATETIME2 NOT NULL CONSTRAINT DF_Tasks_CreatedAt DEFAULT SYSUTCDATETIME(),
    CONSTRAINT FK_Tasks_Users
        FOREIGN KEY (UserId) REFERENCES dbo.Users(Id),
    CONSTRAINT CK_Tasks_Priority
        CHECK (Priority IN (N'Low', N'Medium', N'High'))
);
GO

INSERT INTO dbo.Users (FullName, Email)
VALUES
    (N'Ibrahim Ahmed', N'ibrahim@example.com'),
    (N'Taylor Morgan', N'taylor@example.com');
GO

INSERT INTO dbo.Tasks (UserId, Title, Description, Priority, IsComplete, DueDate)
VALUES
    (1, N'Create project', N'Set up the .NET console app', N'High', 1, '2026-05-01'),
    (1, N'Write sample code', N'Add an interactive task tracker', N'Medium', 0, '2026-05-08'),
    (2, N'Review SQL schema', N'Check tables, constraints, and queries', N'Low', 0, NULL);
GO

-- List open tasks with assigned user details.
SELECT
    t.Id,
    t.Title,
    t.Priority,
    t.DueDate,
    u.FullName AS AssignedTo
FROM dbo.Tasks AS t
INNER JOIN dbo.Users AS u ON u.Id = t.UserId
WHERE t.IsComplete = 0
ORDER BY
    t.DueDate,
    CASE t.Priority
        WHEN N'High' THEN 3
        WHEN N'Medium' THEN 2
        WHEN N'Low' THEN 1
    END DESC;
GO

-- Count tasks by completion status.
SELECT
    CASE IsComplete
        WHEN 1 THEN N'Complete'
        ELSE N'Open'
    END AS Status,
    COUNT(*) AS TaskCount
FROM dbo.Tasks
GROUP BY IsComplete;
GO

-- Mark a task complete.
UPDATE dbo.Tasks
SET IsComplete = 1
WHERE Id = 2;
GO

-- Delete completed tasks older than a chosen date.
DELETE FROM dbo.Tasks
WHERE IsComplete = 1
  AND CreatedAt < '2026-05-01';
GO
