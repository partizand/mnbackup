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
    public class StatusInfo
    {
        
        /// <summary>
        /// Текущий статус
        /// </summary>
        public StatusBackup Status
        {
            get { return _status; }
        }

        private StatusBackup _status;


        /// <summary>
        /// Инициализация значением ОК
        /// </summary>
        public StatusInfo()
        {
            _status = StatusBackup.OK;
        }
        /// <summary>
        /// Инициализация заданным значением
        /// </summary>
        /// <param name="status"></param>
        public StatusInfo(StatusBackup status)
        {
            _status = status;
        }
        /// <summary>
        /// Обновляет статус. Если новый статус хуже он становится текущим
        /// </summary>
        /// <param name="status"></param>
        public void AddStatus(StatusBackup status)
        {
            if (_status < status) _status = status;
        }

    }
}
