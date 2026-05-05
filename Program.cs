List<TaskItem> tasks = new()
{
    new TaskItem(1, "Create project", "Set up the .NET console app", Priority.High, true),
    new TaskItem(2, "Write sample code", "Add a useful interactive example", Priority.Medium),
    new TaskItem(3, "Run the app", "Test the program with dotnet run", Priority.Low)
};

int nextId = tasks.Max(task => task.Id) + 1;

Console.WriteLine("Task Tracker");
Console.WriteLine("============");

bool running = true;
while (running)
{
    ShowMenu();

    string choice = ReadRequired("Choose an option: ");
    Console.WriteLine();

    switch (choice)
    {
        case "1":
            ListTasks(tasks);
            break;
        case "2":
            AddTask(tasks, ref nextId);
            break;
        case "3":
            CompleteTask(tasks);
            break;
        case "4":
            DeleteTask(tasks);
            break;
        case "5":
            ShowSummary(tasks);
            break;
        case "0":
            running = false;
            break;
        default:
            Console.WriteLine("Unknown option. Try again.");
            break;
    }

    Console.WriteLine();
}

Console.WriteLine("Goodbye.");

static void ShowMenu()
{
    Console.WriteLine("1. List tasks");
    Console.WriteLine("2. Add task");
    Console.WriteLine("3. Complete task");
    Console.WriteLine("4. Delete task");
    Console.WriteLine("5. Show summary");
    Console.WriteLine("0. Exit");
}

static void ListTasks(List<TaskItem> tasks)
{
    if (tasks.Count == 0)
    {
        Console.WriteLine("No tasks found.");
        return;
    }

    foreach (TaskItem task in tasks.OrderBy(task => task.IsComplete).ThenByDescending(task => task.Priority))
    {
        string status = task.IsComplete ? "Done" : "Open";
        Console.WriteLine($"#{task.Id} [{status}] {task.Title}");
        Console.WriteLine($"    Priority: {task.Priority}");
        Console.WriteLine($"    Notes: {task.Description}");
    }
}

static void AddTask(List<TaskItem> tasks, ref int nextId)
{
    string title = ReadRequired("Title: ");
    string description = ReadRequired("Description: ");
    Priority priority = ReadPriority();

    tasks.Add(new TaskItem(nextId, title, description, priority));
    Console.WriteLine($"Added task #{nextId}.");
    nextId++;
}

static void CompleteTask(List<TaskItem> tasks)
{
    int id = ReadInt("Task ID to complete: ");
    int index = tasks.FindIndex(task => task.Id == id);

    if (index == -1)
    {
        Console.WriteLine("Task not found.");
        return;
    }

    TaskItem task = tasks[index];
    tasks[index] = task with { IsComplete = true };
    Console.WriteLine($"Completed task #{id}: {task.Title}");
}

static void DeleteTask(List<TaskItem> tasks)
{
    int id = ReadInt("Task ID to delete: ");
    TaskItem? task = tasks.FirstOrDefault(task => task.Id == id);

    if (task is null)
    {
        Console.WriteLine("Task not found.");
        return;
    }

    tasks.Remove(task);
    Console.WriteLine($"Deleted task #{id}: {task.Title}");
}

static void ShowSummary(List<TaskItem> tasks)
{
    int completed = tasks.Count(task => task.IsComplete);
    int open = tasks.Count - completed;

    Console.WriteLine($"Total tasks: {tasks.Count}");
    Console.WriteLine($"Open tasks: {open}");
    Console.WriteLine($"Completed tasks: {completed}");

    foreach (Priority priority in Enum.GetValues<Priority>())
    {
        int count = tasks.Count(task => task.Priority == priority && !task.IsComplete);
        Console.WriteLine($"Open {priority} priority: {count}");
    }
}

static string ReadRequired(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }

        Console.WriteLine("Please enter a value.");
    }
}

static int ReadInt(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (int.TryParse(input, out int value))
        {
            return value;
        }

        Console.WriteLine("Please enter a valid number.");
    }
}

static Priority ReadPriority()
{
    while (true)
    {
        Console.Write("Priority (low, medium, high): ");
        string? input = Console.ReadLine();

        if (Enum.TryParse(input, ignoreCase: true, out Priority priority))
        {
            return priority;
        }

        Console.WriteLine("Please enter low, medium, or high.");
    }
}

public enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3
}

public record TaskItem(
    int Id,
    string Title,
    string Description,
    Priority Priority,
    bool IsComplete = false);
