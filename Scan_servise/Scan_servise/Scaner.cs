using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Scan_servise
{
    class Scaner
    {
        object locker = new object();
        Task Task { get; set; }
        TaskContext Db { get; set; }
        Thread[] Threads { get; set; }
        string TextFromFile { get; set; }
        DirectoryInfo Directory { get; set; }
        int errorsCount { get; set; }
        int JSCount { get; set; }
        int RMCount { get; set; }
        int RunCount { get; set; }
        string jsString = "<script>evil_script()</script>";
        string rmString = @"rm -rf %userprofile%\Documents";
        string runString = "Rundll32 sus.dll SusEntry";
        Stopwatch stopWatch = new Stopwatch();

        public Scaner(Task task, TaskContext db)
        {
            Directory = new DirectoryInfo(task.DirectoryName);
            errorsCount = 0;
            JSCount = 0;
            RMCount = 0;
            RunCount = 0;
            Task = task;
            Db = db;
            Scan();
        }

        private void Scan()
        {
            stopWatch.Start();
            List<FileInfo> fileList = Directory.GetFiles().ToList();
            Threads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < fileList.Count; ++i)
            {
                for (int j = 0; j < Environment.ProcessorCount; ++j)
                    if (i + j < fileList.Count)
                        AddToThreads(fileList[i + j], j);
                for (int j = 0; j < Environment.ProcessorCount; ++j)
                    if (Threads[j] != null)
                        Threads[j].Join();
                i += Environment.ProcessorCount - 1;
            }
            stopWatch.Stop();
            OutputResult(fileList.Count, String.Format("{0:00}:{1:00}:{2:00}", stopWatch.Elapsed.Hours,
                stopWatch.Elapsed.Minutes, stopWatch.Elapsed.Seconds));
        }

        private void SearchingThreat(string extension)
        {
            if (extension == ".js")
            {
                if (TextFromFile.Contains(jsString))
                    JSCount++;
            }
            else if (TextFromFile.Contains(rmString))
                RMCount++;
            else if (TextFromFile.Contains(runString))
                RunCount++;
        }

        private void OutputResult(int filesCount, string exectionTime)
        {
            Db.Tasks.Where(t => t.Id == Task.Id).FirstOrDefault().Status =
                "====== Scan result ======\r\n\r\nDirectory: " + Directory.FullName + "\r\nProcessed files: " + filesCount + "\r\nJS detects: "
                + JSCount + "\r\nrm -rf detects: " + RMCount + "\r\nRundll32 detects: " + RunCount + "\r\nErrors: "
                + errorsCount + "\r\nExection time: " + exectionTime + "\r\n=========================";
            Db.SaveChangesAsync().GetAwaiter().GetResult();
        }

        private void AddToThreads(FileInfo file, int index)
        {
            Thread myThread = new Thread(new ParameterizedThreadStart(ProcessingFile));
            Threads[index] = myThread;
            myThread.Start(file);
        }

        private void ProcessingFile(object obj)
        {
            lock (locker)
            {
                FileInfo file = (FileInfo)obj;
                try
                {
                    using (var reader = new StreamReader(file.FullName))
                    {
                        TextFromFile = reader.ReadToEnd();
                    }
                    SearchingThreat(file.Extension);
                }
                catch (Exception)
                {
                    errorsCount++;
                }
            }
        }
    }
}
