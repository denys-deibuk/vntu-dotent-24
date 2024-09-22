namespace DenysDeibuk.TaskPlanner.DataAccess;

using System;
using System.Collections.Generic;
using System.IO;
using DenysDeibuk.TaskPlanner.DataAccess.Abstractions;
using DenysDeibuk.TaskPlanner.Domain.Models;
using Newtonsoft.Json;

public class FileWorkItemsRepository : IWorkItemsRepository
{
  private const string FileName = "work-items.json";
  private readonly Dictionary<Guid, WorkItem> _workItems = new Dictionary<Guid, WorkItem>();

  public FileWorkItemsRepository()
  {
    if (File.Exists(FileName) && new FileInfo(FileName).Length > 0)
    {
      string json = File.ReadAllText(FileName);
      var workItemArray = JsonConvert.DeserializeObject<WorkItem[]>(json);

      if (workItemArray == null) {
        return;
      }

      foreach (var workItem in workItemArray)
      {
        _workItems.Add(workItem.Id, workItem);
      }
    }
    else
    {
      _workItems = new Dictionary<Guid, WorkItem>();
    }
  }

  public Guid Add(WorkItem workItem)
  {
    var newWorkItem = (WorkItem)workItem.Clone();
    newWorkItem.Id = Guid.NewGuid();
    newWorkItem.CreationDate = DateTime.Now;

    _workItems.Add(newWorkItem.Id, newWorkItem);

    return newWorkItem.Id;
  }

  public WorkItem? Get(Guid id)
  {
    return _workItems.TryGetValue(id, out var workItem) ? workItem : null;
  }

  public WorkItem[] GetAll()
  {
    return new List<WorkItem>(_workItems.Values).ToArray();
  }

  public bool Update(WorkItem workItem)
  {
    if (_workItems.ContainsKey(workItem.Id))
    {
      _workItems[workItem.Id] = workItem;
      return true;
    }
    return false;
  }

  public bool Remove(Guid id)
  {
    return _workItems.Remove(id);
  }

  public void SaveChanges()
  {
    var workItemArray = new List<WorkItem>(_workItems.Values);
    string json = JsonConvert.SerializeObject(workItemArray, Formatting.Indented);
    File.WriteAllText(FileName, json);
  }
}
