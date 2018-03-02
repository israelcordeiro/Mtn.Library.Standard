using System;


namespace Mtn.Library.Entities
{
    /// <summary>
    /// <para>Representig a task to be used in scheduler.</para>
    /// </summary>
    public class Task
    {
        /// <summary>
        /// <para>Void method to be executed.</para>
        /// </summary>
        public Action Job { get; set; }
        /// <summary>
        /// <para>How often should time be called.</para>
        /// </summary>
        public TimeSpan WorkInterval { get; set; }
        /// <summary>
        /// <para>When it should be called.</para>
        /// </summary>
        public DateTime WorkDate { get; set; }
        /// <summary>
        /// <para>Last Execution.</para>
        /// </summary>
        public DateTime LastExecution { get; set; }
        /// <summary>
        /// <para>How many times can be called.</para>
        /// </summary>
        public int WorkLimit { get; set; }
        /// <summary>
        /// <para>Indicates whether to use the limiting parameter WorkLimit.</para>
        /// </summary>
        public bool UseLimit { get; set; }
        /// <summary>
        /// <para>Hierarchy, degree of importance on other tasks.</para>
        /// </summary>
        public int Hierarchy { get; set; }
        
        
        /// <summary>
        /// Constructor
        /// </summary>
        public Task()
        {
            WorkInterval = TimeSpan.Zero;
            WorkDate = new DateTime(DateTime.Now.Year - 1, 1, 1);
            Hierarchy = 999999;
            LastExecution = WorkDate;
            WorkLimit = 1;
        }
    }
}
