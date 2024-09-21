using DenysDeibuk.TaskPlanner.Domain.Models;
using DenysDeibuk.TaskPlanner.DataAccess.Abstractions;

namespace DenysDeibuk.TaskPlanner.Domain.Logic;


public class SimpleTaskPlanner
{
  public WorkItem[] CreatePlan(IWorkItemsRepository fileWorkItemsRepository)
  {
    List<WorkItem> workItemList = fileWorkItemsRepository.GetAll().ToList();

    workItemList.Sort((x, y) =>
        {
          int priorityComparison = y.Priority.CompareTo(x.Priority);
          if (priorityComparison != 0)
            return priorityComparison;

          int dueDateComparison = x.DueDate.CompareTo(y.DueDate);
          if (dueDateComparison != 0)
            return dueDateComparison;

          return string.Compare(x.Title, y.Title, StringComparison.OrdinalIgnoreCase);
        });

    return workItemList.ToArray();
  }
}

