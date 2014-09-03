using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mnBackupLib
{
    /// <summary>
    /// Статус бэкапа. Error - были ошибки, Fatal - не стартовал вообще
    /// </summary>
    public enum StatusBackup { OK, Warning, Error, Fatal }
    
    /// <summary>
    /// Инофрмация об ошибке, статусе
    /// </summary>
    public class StatusInfo<T> where T : System.IComparable
    {
        
        /// <summary>
        /// Текущий статус
        /// </summary>
        public T Status
        {
            get { return _status; }
        }

        private T _status;


        
        /// <summary>
        /// Инициализация заданным значением
        /// </summary>
        /// <param name="status"></param>
        public StatusInfo(T status)
        {
            _status = status;
        }
        /// <summary>
        /// Обновляет статус. Если новый статус хуже (больше) он становится текущим
        /// </summary>
        /// <param name="status"></param>
        public void AddStatus(T status)
        {
            //int i = status.ToInt32( ToInt32();

            if (_status.CompareTo(status) < 0)
                _status = status;
        }

    }
}
