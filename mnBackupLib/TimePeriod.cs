using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace mnBackupLib
{
    [DataContract]
    public class TimePeriod
    {
        [DataContract]
        public struct TimeRange
        {
            [DataMember]
            public int Days { get; set; }
            [DataMember]
            public int Weeks { get; set; }
            [DataMember]
            public int Months { get; set; }
            [DataMember]
            public int Years { get; set; }
        }

        [DataMember]
        public TimeRange Range;

        #region Constructors

        public TimePeriod()
        {
            Range=new TimeRange();
            
            Range.Days = 0;
            Range.Weeks = 0;
            Range.Months = 0;
            Range.Years = 0;
        }
        
        

        public TimePeriod(TimeRange range)
        {
            Range=range;
        }
        
        
        /// <summary>
        /// Чтение периода из строки
        /// Возможные варианты:
        /// Просто число
        /// 1m,1w,1d
        /// m, month
        /// w,week
        /// d, day
        /// </summary>
        /// <param name="str"></param>
        public TimePeriod(string str)
        {
            
            ParseI(str, out Range);

        }
        
        #endregion

        public bool isEmpty()
        {
            
            return Range.Days + Range.Weeks + Range.Months + Range.Years == 0;
        }

        public override string ToString()
        {
            StringBuilder str=new StringBuilder();
            if (Range.Days > 0)
                str.AppendFormat("{0}d", Range.Days);
            if (Range.Weeks > 0)
                str.AppendFormat("{0}w", Range.Weeks);
            if (Range.Months > 0)
                str.AppendFormat("{0}m", Range.Months);
            if (Range.Years > 0)
                str.AppendFormat("{0}y", Range.Years);
            
            return str.ToString();
            
        }
        /// <summary>
        /// Чтение периода из строки. Возвращает имя периода и значение
        /// </summary>
        /// <param name="str"></param>
        /// <param name="periodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ParseI(string str, out TimeRange range)
        {
            range=new TimeRange();
            int value=1;
            bool hasNumbers;
            bool ret = true;
            char[] Numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string Str = str.ToLower().Trim();
            int lastNumIndex = Str.LastIndexOfAny(Numbers);

            //p.intervalValue = 1;
            //p.intervalName = PeriodName.Day;

            if (lastNumIndex > -1) // Цифры есть
            {
                string strNum = Str.Substring(0, lastNumIndex + 1);
                ret = int.TryParse(strNum, out value);
                if (!ret) value = 0;
                hasNumbers = true;
            }
            else
            {
                hasNumbers = false;
            }



            if (Str.EndsWith("d") || Str.EndsWith("day")) range.Days = value;
            else if (Str.EndsWith("w") || Str.EndsWith("week")) range.Weeks = value;
            else if (Str.EndsWith("m") || Str.EndsWith("month")) range.Months = value;
            else if (Str.EndsWith("y") || Str.EndsWith("year")) range.Years = value;
            else if (hasNumbers) range.Days = value;
            
            return ret;
        }
        /// <summary>
        /// Прочитать период из строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static TimePeriod Parse(string str)
        {
            TimeRange tr;
            ParseI(str, out tr);
            TimePeriod p = new TimePeriod(tr);
            return p;
            
        }
        /// <summary>
        /// Прочитать период из строки
        /// </summary>
        /// <param name="str"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool TryParse(string str,out TimePeriod p)
        {
            p = new TimePeriod();
            try
            {
                p = Parse(str);
                return true;
            }
            catch
            {
                return false;
            }

        }

        #region Equals ovveride

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            TimePeriod p = obj as TimePeriod;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Range.Days == p.Range.Days) && (Range.Weeks == p.Range.Weeks) && (Range.Months == p.Range.Months) && (Range.Years == p.Range.Years);
        }

        public bool Equals(TimePeriod p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (Range.Days == p.Range.Days) && (Range.Weeks == p.Range.Weeks) && (Range.Months == p.Range.Months) && (Range.Years == p.Range.Years);
        }

        public override int GetHashCode()
        {
            
            return Range.Days + Range.Weeks * 7 + Range.Months * 30 + Range.Years * 365;
        }

        public static bool operator ==(TimePeriod a, TimePeriod b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return (a.Range.Days == b.Range.Days) && (a.Range.Weeks == b.Range.Weeks) && (a.Range.Months == b.Range.Months) && (a.Range.Years == b.Range.Years);
        }

        public static bool operator !=(TimePeriod a, TimePeriod b)
        {
            return !(a == b);
        }


        #endregion
        /// <summary>
        /// Прошел ли интервал с указанной даты
        /// </summary>
        /// <param name="dt">Дата в прошлом для отсчета интервала</param>
        /// <returns></returns>
        public bool IsInInterval(DateTime dt)
        {
            DateTime dtAdd=AddToDate(dt);
            
            if (dtAdd <= DateTime.Today)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        
        /// <summary>
        /// Прибавить интервал к дате
        /// </summary>
        /// <param name="dt">Начальная дата</param>
        /// <returns>Дата с прибавленным интервалом</returns>
        public DateTime AddToDate(DateTime dt)
        {
            return AddSubToDate(dt, 1);
        }
        /// <summary>
        /// Отнять интервал от даты
        /// </summary>
        /// <param name="dt">Исходная дата</param>
        /// <returns>Дата за минусом интервала</returns>
        public DateTime SubFromDate(DateTime dt)
        {
            return AddSubToDate(dt, -1);
        }
        /// <summary>
        /// Прибавить или отнять интервал от даты.
        /// </summary>
        /// <param name="dt">Исходная дата</param>
        /// <param name="multiplier">1 - прибавить,  -1 - отнять</param>
        /// <returns>Результат</returns>
        private DateTime AddSubToDate(DateTime dt, int multiplier)
        {
            DateTime dtAdd;
            if (multiplier < 0) multiplier = -1;
            if (multiplier > 0) multiplier = 1;
            if (multiplier == 0) multiplier = 1;

            dtAdd = dt.AddDays(Range.Days * multiplier);
            dtAdd = dtAdd.AddDays(Range.Weeks * 7 * multiplier);
            dtAdd = dtAdd.AddMonths(Range.Months * multiplier);
            dtAdd = dtAdd.AddYears(Range.Years * multiplier);

            
            return dtAdd;
        }

        
 

    }
}
