using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Mtn.Library.Extensions;


namespace Mtn.Library.Service
{
    /// <summary>    
    /// <para>Class Mtn.Library for job scheduling.</para>
    /// </summary>
    public static class Scheduler
    {
        private static readonly Dictionary<string, Entities.Task> MTasks = new Dictionary<string, Entities.Task>();
        private static readonly Dictionary<string, Thread> MThreads = new Dictionary<string, Thread>();
        private static readonly TimeSpan MTimeOut = TimeSpan.FromSeconds(1);

        static Scheduler()
        {
            var principal = new Thread(new ThreadStart(Work)) {IsBackground = true};
            if(!MThreads.ContainsKey("Mtn_ThreadPrincipal"))
                MThreads.Add("Mtn_ThreadPrincipal", principal);
            principal.Start();

        }
        /// <summary>
        /// <para>Add task to schedule.</para>
        /// </summary>
        /// <param name="task">
        /// <para>Task </para>
        /// </param>
        public static void AddTask(Entities.Task task)
        {
            AddTask(task, false);
        }
        /// <summary>
        /// <para>Add task to schedule.</para>
        /// </summary>
        /// <param name="task">
        /// <para>Task </para>
        /// </param>
        /// <param name="exclusiveThread">
        /// <para>Indicates if must run in a exclusive thread or in default task list.</para>
        /// </param>
        public static void AddTask(Entities.Task task, bool exclusiveThread)
        {
            string name = Guid.NewGuid().ToString();
            AddTask(task, exclusiveThread, name);
        }
        /// <summary>
        /// <para>Add task to schedule.</para>
        /// </summary>
        /// <param name="task">
        /// <para>Task </para>
        /// </param>
        /// <param name="exclusiveThread">
        /// <para>Indicates if must run in a exclusive thread or in default task list.</para>
        /// </param>
        /// <param name="name">
        /// <para>Name to task (very usefull if you need stop task in real time).</para>
        /// </param>
        public static void AddTask(Entities.Task task, bool exclusiveThread, string name)
        {
            if (exclusiveThread)
            {
                var newThread = new Thread(WorkExclusive) {IsBackground = true};
                MThreads.Add(name, newThread);
                newThread.Start(task);
            }
            else
            {
                MTasks.Add(name, task);
            }
        }
        /// <summary>
        /// <para>Remove a task from schedule.</para>
        /// </summary>        
        /// <param name="name">
        /// <para>Name to task .</para>
        /// </param>
        public static void RemoveTask(string name)
        {
            RemoveTask(name, false, false);
        }
        /// <summary>
        /// <para>Remove a task from schedule.</para>
        /// </summary>        
        /// <param name="name">
        /// <para>Name to task .</para>
        /// </param>
        /// <param name="abort">
        /// <para>Indicates if abort the thread or interrupt only.</para>
        /// </param>
        public static void RemoveTask(string name, bool abort)
        {
            RemoveTask(name, abort, false);
        }
        /// <summary>
        /// <para>Remove a task from schedule.</para>
        /// </summary>        
        /// <param name="name">
        /// <para>Name to task .</para>
        /// </param>
        /// <param name="abort">
        /// <para>Indicates if abort the thread or interrupt only.</para>
        /// </param>
        /// <param name="exclusiveThread">
        /// <para>Indicates if thread is a exclusive thread or are in default task list.</para>
        /// </param>
        public static void RemoveTask(string name, bool abort, bool exclusiveThread)
        {
            try
            {
                if (exclusiveThread)
                {
                    var thread = MThreads[name];
                    if (abort)
                        thread.Abort();
                    else
                        thread.Interrupt();

                    MThreads.Remove(name);
                }
                else
                {
                    MTasks.Remove(name);
                }
            }
            catch (Exception ex)
            {
                Service.Statistics.Add(ex.GetAllMessagesMtn());
            }

        }

        private static void WorkExclusive(object boxObj)
        {
            Thread.Sleep(50);
            for (; ; )
            {
                try
                {
                    Entities.Task task = (Entities.Task)boxObj;
                    if (task.UseLimit)
                    {
                        if (task.WorkLimit > 0)
                            task.WorkLimit--;
                        else
                            return;
                    }

                    if (task.WorkInterval != TimeSpan.Zero && task.LastExecution.Add(task.WorkInterval) < DateTime.Now)
                        return;

                    if (task.WorkDate < DateTime.Now)
                        return;

                    task.Job();
                    task.LastExecution = DateTime.Now;
                }
                catch (ThreadAbortException)
                {   
                }
                catch (Exception ex)
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                }
                finally
                {
                    try
                    {
                        Mtn.Library.Service.Cache.ResetCache();
                    }
                    catch { }

                }
                Thread.Sleep(MTimeOut);
            }
        }

        private static void Work()
        {
            try
            {
                Thread.Sleep(50);
                for (; ; )
                {
                    foreach (var task in MTasks.OrderBy(t => t.Value.Hierarchy))
                    {
                        try
                        {
                            if (task.Value.UseLimit)
                            {
                                if (task.Value.WorkLimit > 0)
                                {
                                    task.Value.WorkLimit--;
                                    if (task.Value.WorkLimit == 0)
                                        MTasks.Remove(task.Key);
                                }
                                else
                                {
                                    MTasks.Remove(task.Key);
                                    continue;
                                }
                            }

                            if (task.Value.WorkInterval != TimeSpan.Zero && task.Value.LastExecution.Add(task.Value.WorkInterval) > DateTime.Now)
                                continue;

                            if (task.Value.WorkInterval == TimeSpan.Zero && task.Value.UseLimit == false && task.Value.WorkDate >= DateTime.Now)
                                MTasks.Remove(task.Key);
                            else if (task.Value.WorkDate > DateTime.Now)
                                continue;

                            task.Value.Job();
                            task.Value.LastExecution = DateTime.Now;
                        }
                        catch (ThreadAbortException)
                        {   
                            // Ignore
                            try
                            {
                                Mtn.Library.Service.Cache.ResetCache();
                            }
                            catch
                            {
                                // ignored
                            }
                        }
                        catch (Exception ex)
                        {
                            Service.Statistics.Add(ex.GetAllMessagesMtn());
                        }
                    }

                    try
                    {
                        Thread.Sleep(MTimeOut);
                    }
                    catch (ThreadAbortException)
                    {
                        
                        // Ignore
                        try
                        {
                            Mtn.Library.Service.Cache.ResetCache();
                        }
                        catch
                        {
                            // ignored
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Service.Statistics.Add(ex.GetAllMessagesMtn());
                    }
                    finally
                    {
                        try
                        {
                            Mtn.Library.Service.Cache.ResetCache();
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
            }
            catch (ThreadAbortException)
            {
                // reseto o cache para evitar dores de cabeça
                
            }
            catch (Exception ex)
            {
                try
                {
                    Service.Statistics.Add(ex.GetAllMessagesMtn());
                    Mtn.Library.Service.Cache.ResetCache();
                }
                catch
                {
                    // ignored
                }
            }
            finally
            {
                try
                {
                    Mtn.Library.Service.Cache.ResetCache();
                }
                catch
                {
                    // ignored
                }
                finally
                {
                }

            }
        }
    }
}
