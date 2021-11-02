﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Scan_servise
{
    public class Service : IService
    {
        public TaskContext db = new TaskContext();
        public Task GetTask(string id)
        {
            Task task = db.Tasks.FindAsync(int.Parse(id)).GetAwaiter().GetResult();
            if (task == null)
            {
                return null;
            }
            return task;
        }
        [HttpPost]
        public Task AddTask(Task task)
        {
            task.Id = db.Tasks.Count() + 1;
            db.Tasks.Add(task);
            db.SaveChangesAsync().GetAwaiter().GetResult();
            return task;
        }
        [HttpPut]
        public Task StartScan(Task task)
        {
            Scaner scaner = new Scaner(task, db);
            return task;
        }
    }
}
