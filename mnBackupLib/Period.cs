using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace mnBackupLib
{
    /// <summary>
    /// Класс для работы с интервалами 1d, 1w
    /// </summary>
    [DataContract]
    public class Period
    {
        /// <summary>
        /// Имена периодов (Day, Week, Month, Year)
        /// </summary>
        public enum PeriodName {Day, Week, Month }

        #region Properties

        /// <summary>
        /// Имя периода
        /// </summary>
        public PeriodName IntervalName
        {
            get { return intervalName; }
        }
        [DataMember]
        PeriodName intervalName;

        /// <summary>
        /// Значение периода
        /// </summary>
        public int IntervalValue
        {
            get { return intervalValue; }
        }
        [DataMember]
        int intervalValue;

        #endregion

        #region Constructors

        public Period(PeriodName IntName, int IntValue)
        {
            intervalName = IntName;
            intervalValue = IntValue;
        }

        public Period(PeriodName IntName)
        {
            intervalName = IntName;
            intervalValue = 1;
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
        public Period(string str)
        {
            
            ParseI(str, out intervalName, out intervalValue);

        }
        
        #endregion

        public override string ToString()
        {
            return intervalValue.ToString() + intervalName.ToString().Substring(0,1);
        }
        /// <summary>
        /// Чтение периода из строки. Возвращает имя периода и значение
        /// </summary>
        /// <param name="str"></param>
        /// <param name="periodName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ParseI(string str, out PeriodName periodName,out int value)
        {
            value = 1;
            periodName = PeriodName.Day;
            bool ret = true;
            char[] Numbers = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            string Str = str.ToLower().Trim();
            int lastNumIndex = Str.LastIndexOfAny(Numbers);

            //p.intervalValue = 1;
            //p.intervalName = PeriodName.Day;

            if (lastNumIndex > -1) // Цифры есть
            {
                string strNum = Str.Substring(0, lastNumIndex + 1);
                ret=int.TryParse(strNum, out value);
            }


            if (Str.EndsWith("d") || Str.EndsWith("day")) periodName = PeriodName.Day;
            if (Str.EndsWith("w") || Str.EndsWith("week")) periodName = PeriodName.Week;
            if (Str.EndsWith("m") || Str.EndsWith("month")) periodName = PeriodName.Month;
            return ret;
        }
        /// <summary>
        /// Прочитать период из строки
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Period Parse(string str)
        {
            int val;
            PeriodName per;
            ParseI(str, out per, out val);
            Period p = new Period(per, val);
            return p;
            
        }
        /// <summary>
        /// Прочитать период из строки
        /// </summary>
        /// <param name="str"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool TryParse(string str,out Period p)
        {
            p = new Period(PeriodName.Day);
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
            Period p = obj as Period;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (intervalName == p.intervalName) && (intervalValue == p.intervalValue);
        }

        public bool Equals(Period p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (intervalName == p.intervalName) && (intervalValue == p.intervalValue);
        }

        public override int GetHashCode()
        {
            return (int)intervalName^intervalValue;
        }

        public static bool operator ==(Period a, Period b)
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
            return a.IntervalName== b.IntervalName && a.IntervalValue == b.IntervalValue;
        }

        public static bool operator !=(Period a, Period b)
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

            switch (intervalName)
            {
                case PeriodName.Day:
                    dtAdd = dt.AddDays(intervalValue * multiplier);
                    break;
                case PeriodName.Week:
                    dtAdd = dt.AddDays(intervalValue * 7 * multiplier);
                    break;
                case PeriodName.Month:
                    dtAdd = dt.AddMonths(intervalValue * multiplier);
                    break;
                default:
                    dtAdd = dt.AddDays(intervalValue * multiplier);
                    break;


            }
            return dtAdd;
        }
 
        
    }
}
