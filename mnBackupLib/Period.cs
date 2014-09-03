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
            
            char[] Numbers = {'0','1', '2','3','4','5','6','7','8','9' };
            string Str = str.ToLower().Trim();
            int lastNumIndex = Str.LastIndexOfAny(Numbers);
            
            intervalValue = 1;
            intervalName = PeriodName.Day;

            if (lastNumIndex > -1) // Цифры есть
            {
                string strNum = Str.Substring(0, lastNumIndex + 1);
                int.TryParse(strNum, out intervalValue);
            }
            

            if (Str.EndsWith("d") || Str.EndsWith("day")) intervalName = PeriodName.Day;
            if (Str.EndsWith("w") || Str.EndsWith("week")) intervalName = PeriodName.Week;
            if (Str.EndsWith("m") || Str.EndsWith("month")) intervalName = PeriodName.Month;
        }
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
