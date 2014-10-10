using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mnBackupLib
{
    public class ReplVar
    {
        
        
        /// <summary>
        /// Заменяет параметры даты (типа [yymmdd]) на текущую дату в строке S
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public static string ReplDate(string S)
        {
            return ReplDate(S, DateTime.Now);
        }
        /// <summary>
        /// Заменяет параметры даты (типа [yymmdd]) на заданную дату в строке S
        /// </summary>
        /// <param name="S"></param>
        /// <param name="replDate"></param>
        /// <returns></returns>
        public static string ReplDate(string S, DateTime replDate)
        {
            string param, newS, strDate;
            //DateTime dtNow = DateTime.Now;
            newS = S;
            const string Frame = "{}";
            param = GetStrVar(newS, Frame);
            while (!String.IsNullOrEmpty(param))
            {
                strDate = replDate.ToString(param);
                //newS =  newS.Replace("%" + param + "%", strDate);
                newS = ReplaceParam(newS, Frame, param, strDate);

                param = GetStrVar(newS, Frame);
            }
            return newS;
        }

        /// <summary>
        /// Заменяет переменные среды и имя пользователя/машины в строке
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplENV(string S)
        {
            string NewS=System.Environment.ExpandEnvironmentVariables(S);
            NewS=NewS.Replace("%ComputerName%", System.Environment.MachineName);
            NewS = NewS.Replace("%MachineName%", System.Environment.MachineName);
            NewS = NewS.Replace("%UserName%", System.Environment.UserName);
            return NewS;
        }

        /// <summary>
        /// Заменяет переменнные заданные пользователем
        /// обрамленные [].
        /// Переменные задаются через Options в виде ключ - значение
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public string ReplOptions(string S,Dictionary<string,string> Options)
        {
            string param, newS;
            string Frame="[]";
            newS = S;
            param = GetStrVar(newS, Frame);
            while (!String.IsNullOrEmpty(param))
            {

                newS = ReplaceParam(newS,Frame, param, Options[param]);
                param = GetStrVar(newS, Frame);
            }
            newS = System.Environment.ExpandEnvironmentVariables(newS);
            return newS;
        }

        /// <summary>
        /// Подстановка занчения параметра в строку. Имя параметра должно быть облачено в разделитель
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ParamName"></param>
        /// <param name="ParamValue"></param>
        /// <returns></returns>
        static string ReplaceParam(string str,string Frame, string ParamName, string ParamValue)
        {
            if (String.IsNullOrEmpty(Frame)) return str;
            if (Frame.Length < 1) return str;
            char BegFrame = Frame[0];
            char EndFrame = Frame[1];
            return str.Replace(BegFrame+ ParamName+EndFrame, ParamValue);
        }

        /// <summary>
        /// Находит в строке S первое вхождение параметра обрамленного в Frame (строка из 2-х символов
        /// с симоволм начала и конца  (например в "{}")
        /// и возвращает этот параметр без этих символов
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        static string GetStrVar(string S, string Frame)
        {
            int beg = 0, end;
            string var = "";
            if (String.IsNullOrEmpty(Frame)) return String.Empty;
            if (Frame.Length < 1) return String.Empty;
            char BegFrame = Frame[0];
            char EndFrame = Frame[1];
            beg = S.IndexOf(BegFrame);

            if (beg < S.Length && beg > -1)
            {

                if (beg == -1) return "";
                end = S.IndexOf(EndFrame, beg + 1);
                if (end == -1) return "";

                var = S.Substring(beg + 1, end - beg - 1);
                return var;
            }
            return String.Empty;


        }
    }
}
