using System;

namespace Mtn.Library.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class DoubleDateInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public enum FormatType
        {
            /// <summary>
            /// 
            /// </summary>
            None = 1,
            /// <summary>
            /// 
            /// </summary>
            ZeroSec = 2,
            /// <summary>
            /// 
            /// </summary>
            ZeroMinute = 3,
            /// <summary>
            /// 
            /// </summary>
            Today = 4,
            /// <summary>
            /// 
            /// </summary>
            FisrtDayOnMonth =5
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime Current { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CurrentChanged { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDate"></param>
        public DoubleDateInfo(FormatType typeDate)
        {
            Current = DateTime.Now;
            switch (typeDate)
            {
                case FormatType.None:
                    CurrentChanged = Current;
                    break;
                case FormatType.ZeroSec:
                    CurrentChanged = new DateTime(Current.Year, Current.Month, Current.Day, Current.Hour, Current.Minute, 0);
                    break;
                case FormatType.ZeroMinute:
                    CurrentChanged = new DateTime(Current.Year, Current.Month, Current.Day, Current.Hour, 0, 0);
                    break;
                case FormatType.Today:
                    CurrentChanged = new DateTime(Current.Year, Current.Month, Current.Day, 0, 0, 0);
                    break;
                case FormatType.FisrtDayOnMonth:
                    CurrentChanged = new DateTime(Current.Year, Current.Month, 1, 0, 0, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("typeDate");
            }
            
        }
    }
}