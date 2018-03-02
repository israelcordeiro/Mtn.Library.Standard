using System.Reflection;
using System;
using System.Collections.Generic;
using Mtn.Library.Enums;
using System.Diagnostics;
#if ! UseSystemExtensions
using Mtn.Library.Extensions;
#endif

namespace Mtn.Library.Service
{
    /// <summary>
    /// 
    /// </summary>
    public static class Statistics
    {
        private static SortedDictionary<string, string> memoryEntry = new SortedDictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="store"></param>
        /// <param name="mode"></param>
        /// <param name="sourceOrFile"></param>
        /// <returns></returns>
        public static Boolean Add(String value, StatisticStore store = StatisticStore.EventLog, StatisticMode mode = StatisticMode.Information, String sourceOrFile = "")
        {
            try
            {
                var storeType = Configuration.Config.GetString("Mtn.Library.ForceStatisticStore");
                if (!storeType.IsNullOrWhiteSpaceMtn())
                {
                    store = storeType.ToEnumMtn<StatisticStore>();
                }
            }
            catch
            {
            }
            try
            {
                switch (store)
                {
                    case StatisticStore.EventLog:
                        EventLogEntry(value, mode, sourceOrFile);
                        break;
                    case StatisticStore.File:
                        return FileEntry(value, mode, sourceOrFile);
                    case StatisticStore.DataBase:
                        return DataBaseEntry(value, mode, sourceOrFile);
                    case StatisticStore.Memory:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("mode");
                }
            }
            catch (Exception exp)
            {
                //ignored
                var msg = "Statistics Error (See target error) :{0} \r\n\r\n Source:{1}\r\nTarget Error:{2}".FormatMtn(exp.GetAllMessagesMtn().ToSafeStringMtn(),
                    sourceOrFile.ToSafeStringMtn(), value.ToSafeStringMtn());
                throw new Exception(msg);
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mode"></param>
        public static void MemoryEntry(String value, StatisticMode mode = StatisticMode.Information)
        {
            memoryEntry.Add(mode.ToString(), value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mode"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool FileEntry(String value, StatisticMode mode = StatisticMode.Information, String fileName = "")
        {
            if (fileName.IsNullOrWhiteSpaceMtn())
                fileName = Utils.Parameter.StatisticsFileName;

            return value.ToFileMtn(fileName, writeline: true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mode"></param>
        /// <param name="source"></param>
        /// <param name="logName"></param>
        /// <param name="eventId"></param>
        /// <param name="category"></param>
        public static void EventLogEntry(String value, StatisticMode mode = StatisticMode.Information, String source = "", String logName = "", Int32 eventId = 1, Int16 category = 1)
        {
            if (Core.Parameter.DisableEventLogStatisticOnLowPermission)
            {
                try
                {
                    if (Core.Parameter.IsFulltrust)
                    {
                        return;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }

            lock (m_lock)
            {
                if (source.IsNullOrWhiteSpaceMtn())
                    source = Utils.Parameter.EventSource;
                if (logName.IsNullOrWhiteSpaceMtn())
                    logName = Utils.Parameter.EventLogName;
                ///todo: Finalizar esta parte
                //EventLogEntryType eventType;
                //switch (mode)
                //{
                //    case StatisticMode.Log:
                //        eventType = EventLogEntryType.SuccessAudit;
                //        break;
                //    case StatisticMode.Information:
                //        eventType = EventLogEntryType.Information;
                //        break;
                //    case StatisticMode.Warn:
                //        eventType = EventLogEntryType.Warning;
                //        break;
                //    case StatisticMode.Error:
                //        eventType = EventLogEntryType.Error;
                //        break;
                //    default:
                //        throw new ArgumentOutOfRangeException("mode");
                //}
                //if (!EventLog.SourceExists(source))
                //{
                //    EventLog.CreateEventSource(source, logName);
                //}

                //EventLog.WriteEntry(source, value, eventType, eventId, category);
            }
        }

        private static readonly object m_lock = new object();
        private static readonly string m_query = "INSERT INTO EventLog(LogId,Source,ModeType, Value, Created) VALUES ('{LogId}','{Source}', '{ModeType}','{Value}','{Created}')";
        private class MtnLog
        {
            public string LogId { get; set; }
            public string Source { get; set; }
            public string ModeType { get; set; }
            public string Value { get; set; }
            public string Created { get { return CreatedType.ToString("yyyy-MM-dd HH:mm:ss"); } }
            public DateTime CreatedType { get; set; }

        }
        /// <summary>
        /// Save on database Log
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mode"></param>
        /// <param name="source"></param>
        public static Boolean DataBaseEntry(String value, StatisticMode mode = StatisticMode.Information, String source = "")
        {

            var model = new MtnLog { CreatedType = DateTime.Now, LogId = Guid.NewGuid().ToString(), ModeType = mode.ToString(), Source = source.Replace("'", "\""), Value = value.Replace("'", "\"") };
            var strQuery =
                Configuration.Config.GetConnectionString("Mtn.Library.Statistics.InsertQuery");

            strQuery = strQuery.IsNullOrWhiteSpaceMtn() ? m_query : strQuery;
            strQuery = strQuery.FormatByPropertyMtn(model);
            var useDbContext =
                Configuration.Config.GetNullableBoolean("Mtn.Library.Statistics.UseDbContext").HasValue;

            var assemblyLoad =
                Configuration.Config.GetString("Mtn.Library.Statistics.EntityAssembly");
            var cnnsStr = Mtn.Library.Configuration.Config.GetConnectionString("Mtn.Library.Statistics.ConnectionString");
            //System.Data.Entity.DbContext            DbConte
            //var db = new ObjectContext();

            //return db.ExecuteStoreCommand(strQuery) > 0;

            var objDb = Assembly.Load(assemblyLoad);
            if (objDb != null)
            {
                var objList = new object[] { cnnsStr };
                dynamic db;
                if (useDbContext)
                {

                    db = objDb.CreateInstance("System.Data.Entity.DbContext", false, BindingFlags.CreateInstance, null, objList, null, null);
                    if (db != null)
                    {
                        int ret = db.Database.ExecuteSqlCommand(strQuery);
                        return ret > 0;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    db = objDb.CreateInstance("System.Data.Objects.ObjectContext", false, BindingFlags.CreateInstance, null, objList, null, null);
                    if (db != null)
                    {

                        int ret = db.ExecuteStoreCommand(strQuery);
                        return ret > 0;
                    }
                    else
                    {
                        return false;
                    }

                }

            }
            else
            {
                throw new ArgumentNullException("Cannot instance the object : " + assemblyLoad);
            }

        }

    }
}
