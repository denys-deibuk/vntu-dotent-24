using DenysDeibuk.TaskPlanner.Domain.Models;
using DenysDeibuk.TaskPlanner.Domain.Models.Enums;
using DenysDeibuk.TaskPlanner.Domain.Logic;
namespace DenysDeibuk.TaskPlanner.ConsoleRunner;

internal static class Program
{
  public static void Main(string[] args)
  {
    List<WorkItem> workItems = new List<WorkItem>();
    string input;

    Console.WriteLine("Введіть завдання. Для завершення введіть 'exit'.");

    while (true)
    {
      Console.WriteLine("Введіть назву завдання (або 'exit' для завершення): ");
      input = Console.ReadLine();
      if (input?.ToLower() == "exit") break;

      string title = input;

      Console.WriteLine("Введіть опис завдання: ");
      string description = Console.ReadLine();

      DateTime dueDate;
      while (true)
      {
        Console.WriteLine("Введіть дату завершення (формат: dd.MM.yyyy): ");
        if (DateTime.TryParse(Console.ReadLine(), out dueDate)) break;
        Console.WriteLine("Неправильний формат дати! Спробуйте ще раз.");
      }

      Priority priority;
      while (true)
      {
        Console.WriteLine("Введіть пріоритет (None, Low, Medium, High, Urgent): ");
        if (Enum.TryParse<Priority>(Console.ReadLine(), true, out priority) && Enum.IsDefined(typeof(Priority), priority)) break;
        Console.WriteLine("Неправильний пріоритет! Спробуйте ще раз.");
      }

      Complexity complexity;
      while (true)
      {
        Console.WriteLine("Введіть складність (None, Minutes, Hours, Days, Weeks): ");
        if (Enum.TryParse<Complexity>(Console.ReadLine(), true, out complexity) && Enum.IsDefined(typeof(Complexity), complexity)) break;
        Console.WriteLine("Неправильна складність! Спробуйте ще раз.");
      }

      WorkItem workItem = new WorkItem
      {
        Title = title,
        Description = description,
        CreationDate = DateTime.Now,
        DueDate = dueDate,
        Priority = priority,
        Complexity = complexity,
        IsCompleted = false
      };

      workItems.Add(workItem);
    }

    SimpleTaskPlanner taskPlanner = new SimpleTaskPlanner();
    WorkItem[] sortedWorkItems = taskPlanner.CreatePlan(workItems.ToArray());

    Console.WriteLine("\nСписок завдань:");
    foreach (WorkItem item in sortedWorkItems)
    {
      Console.WriteLine(item.ToString());
    }
  }
}
