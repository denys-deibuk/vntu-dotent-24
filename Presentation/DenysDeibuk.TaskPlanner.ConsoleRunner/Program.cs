using DenysDeibuk.TaskPlanner.Domain.Models;
using DenysDeibuk.TaskPlanner.Domain.Models.Enums;
using DenysDeibuk.TaskPlanner.Domain.Logic;
using DenysDeibuk.TaskPlanner.DataAccess;
using DenysDeibuk.TaskPlanner.DataAccess.Abstractions;
namespace DenysDeibuk.TaskPlanner.ConsoleRunner;

internal static class Program
{
  public static void Main(string[] args)
  {
    IWorkItemsRepository fileWorkItemsRepository = new FileWorkItemsRepository();
    SimpleTaskPlanner taskPlanner = new SimpleTaskPlanner();
    string? input;

    while (true)
    {
      Console.WriteLine("\nОберіть операцію: ");
      Console.WriteLine("[A]dd work item");
      Console.WriteLine("[B]uild a plan");
      Console.WriteLine("[M]ark work item as completed");
      Console.WriteLine("[R]emove a work item");
      Console.WriteLine("[Q]uit the app");

      input = Console.ReadLine()?.ToLower();

      if (input == null)
      {
        continue;
      }

      switch (input)
      {
        case "a":
          AddWorkItem(fileWorkItemsRepository);
          break;

        case "b":
          BuildPlan(fileWorkItemsRepository, taskPlanner);
          break;

        case "m":
          MarkWorkItemAsCompleted(fileWorkItemsRepository);
          break;

        case "r":
          RemoveWorkItem(fileWorkItemsRepository);
          break;

        case "q":
          return;

        default:
          Console.WriteLine("Неправильна команда! Спробуйте ще раз.");
          break;
      }
    }
  }

  private static void SaveChanges(IWorkItemsRepository fileWorkItemsRepository)
  {
    fileWorkItemsRepository.SaveChanges();
  }

  private static void AddWorkItem(IWorkItemsRepository fileWorkItemsRepository)
  {
    string? title;
    while (true)
    {
      Console.WriteLine("Введіть назву завдання: ");
      title = Console.ReadLine();
      if (title != null) break;
      Console.WriteLine("Спробуйте ще раз.");
    }

    string? description;
    while (true)
    {
      Console.WriteLine("Введіть опис завдання: ");
      description = Console.ReadLine();
      if (description != null) break;
      Console.WriteLine("Спробуйте ще раз.");
    }

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

    fileWorkItemsRepository.Add(workItem);
    SaveChanges(fileWorkItemsRepository);
    Console.WriteLine("Завдання додано успішно.");
  }

  private static void BuildPlan(IWorkItemsRepository fileWorkItemsRepository, SimpleTaskPlanner taskPlanner)
  {
    WorkItem[] sortedWorkItems = taskPlanner.CreatePlan(fileWorkItemsRepository);

    if (sortedWorkItems.Length == 0)
    {
      Console.WriteLine("Немає завдань для побудови плану.");
      return;
    }

    Console.WriteLine("\nСписок завдань:");
    foreach (WorkItem item in sortedWorkItems)
    {
      Console.WriteLine(item.ToString());
    }
  }

  private static void MarkWorkItemAsCompleted(IWorkItemsRepository fileWorkItemsRepository)
  {
    Console.WriteLine("Введіть ID завдання для позначення як виконаного:");
    Guid id;
    if (Guid.TryParse(Console.ReadLine(), out id))
    {
      WorkItem? workItem = fileWorkItemsRepository.Get(id);
      if (workItem != null)
      {
        workItem.IsCompleted = true;
        fileWorkItemsRepository.Update(workItem);
        SaveChanges(fileWorkItemsRepository);
        Console.WriteLine("Завдання відзначено як виконане.");
      }
      else
      {
        Console.WriteLine("Завдання з таким ID не знайдено.");
      }
    }
    else
    {
      Console.WriteLine("Неправильний формат ID!");
    }
  }

  private static void RemoveWorkItem(IWorkItemsRepository fileWorkItemsRepository)
  {
    Console.WriteLine("Введіть ID завдання для видалення:");
    Guid id;
    if (Guid.TryParse(Console.ReadLine(), out id))
    {
      bool removed = fileWorkItemsRepository.Remove(id);
      if (removed)
      {
        SaveChanges(fileWorkItemsRepository);
        Console.WriteLine("Завдання успішно видалено.");
      }
      else
      {
        Console.WriteLine("Завдання з таким ID не знайдено.");
      }
    }
    else
    {
      Console.WriteLine("Неправильний формат ID!");
    }
  }
}
