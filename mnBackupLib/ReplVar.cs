using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mnBackupLib
{
    public class ReplVar
    {

        const string BegFrame = "${";
        const string EndFrame = "}";

        /// <summary>
        /// Раскрывает переменные ${ } в строке
        /// Options - переменные для замены, могут быть null
        /// заменяет дату
        /// Раскрывает переменные среды
        /// </summary>
        /// <param name="S"></param>
        /// <param name="replDate"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ExpandVars(string S,DateTime replDate, Dictionary<string, string> options)
        {
            //string NewS=ReplOptions(S,options);
            //NewS = ReplDate(S,replDate);

            //string param, newS;

            if (options == null)
            {
                options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // Регистр не важен
            }
            options.Add("ComputerName", System.Environment.MachineName);
            options.Add("MachineName", System.Environment.MachineName);
            options.Add("UserName", System.Environment.UserName);
            
            string[] vars=GetStrVars(S);
            string newS = newS = System.Environment.ExpandEnvironmentVariables(S);
            string strDate;
            foreach (string key in vars)
            {
                if (options.ContainsKey(key)) // Переменная
                {
                    newS = ReplaceParam(newS, key, options[key]);
                }
                else // дата время
                {
                    strDate = replDate.ToString(key);
                    //newS =  newS.Replace("%" + param + "%", strDate);
                    newS = ReplaceParam(newS, key, strDate);
                }
            }
            return newS;
        }
        /// <summary>
        /// Раскрывает переменные ${ } в строке
        /// Options - переменные для замены, могут быть null
        /// заменяет дату на текущую
        /// Раскрывает переменные среды
        /// </summary>
        /// <param name="S"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string ExpandVars(string S, Dictionary<string, string> options)
        {
            return ExpandVars(S, DateTime.Now, options);
        }
        /// <summary>
        /// Раскрывает переменные ${ } в строке
        /// заменяет дату
        /// Раскрывает переменные среды
        /// </summary>
        /// <param name="S"></param>
        /// <param name="replDate"></param>
        /// <returns></returns>
        public static string ExpandVars(string S, DateTime replDate)
        {
            return ExpandVars(S, replDate, null);
        }
        /// <summary>
        /// Раскрывает переменные ${ } в строке
        /// заменяет дату на текущую
        /// Раскрывает переменные среды
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        public static string ExpandVars(string S)
        {
            return ExpandVars(S, DateTime.Now, null);
        }

        /// <summary>
        /// Заменяет параметры даты (типа ${yyMMdd}) на текущую дату в строке S
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        /*
        static string ReplDate(string S)
        {
            return ReplDate(S, DateTime.Now);
        }
         */ 
        /// <summary>
        /// Заменяет параметры даты (типа ${yyMMdd}) на заданную дату в строке S
        /// </summary>
        /// <param name="S"></param>
        /// <param name="replDate"></param>
        /// <returns></returns>
        /*
        static string ReplDate(string S, DateTime replDate)
        {
            string param, newS, strDate;
            //DateTime dtNow = DateTime.Now;
            newS = S;
            //const string Frame = "[]";
            param = GetStrVar(newS);
            while (!String.IsNullOrEmpty(param))
            {
                strDate = replDate.ToString(param);
                //newS =  newS.Replace("%" + param + "%", strDate);
                newS = ReplaceParam(newS,  param, strDate);

                param = GetStrVar(newS);
            }
            return newS;
        }
        */
        

        /// <summary>
        /// Заменяет переменнные заданные пользователем
        /// обрамленные ${ }.
        /// Переменные задаются через Options в виде ключ - значение
        /// options may be null
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        /*
        static string ReplOptions(string S,Dictionary<string,string> Options)
        {
            string param, newS;

            if (Options == null)
            {
                Options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); // Регистр не важен
            }

            Options.Add("ComputerName", System.Environment.MachineName);
            Options.Add("MachineName", System.Environment.MachineName);
            Options.Add("UserName", System.Environment.UserName);


            newS = S;
            param = GetStrVar(newS);
            while (!String.IsNullOrEmpty(param))
            {
                if (Options.ContainsKey(param))
                {
                    newS = ReplaceParam(newS, param, Options[param]);
                }
                param = GetStrVar(newS);
            }
            newS = System.Environment.ExpandEnvironmentVariables(newS);
            return newS;
        }
        */
        /// <summary>
        /// Подстановка занчения параметра в строку. Имя параметра должно быть облачено в разделитель
        /// </summary>
        /// <param name="str"></param>
        /// <param name="ParamName"></param>
        /// <param name="ParamValue"></param>
        /// <returns></returns>
        /*
        static string ReplaceParam(string str,string Frame, string ParamName, string ParamValue)
        {
            if (String.IsNullOrEmpty(Frame)) return str;
            if (Frame.Length < 1) return str;
            char BegFrame = Frame[0];
            char EndFrame = Frame[1];
            return str.Replace(BegFrame+ ParamName+EndFrame, ParamValue);
        }
        */
        /// <summary>
        /// Находит в строке S первое вхождение параметра обрамленного в Frame (строка из 2-х символов
        /// с симоволм начала и конца  (например в "{}")
        /// и возвращает этот параметр без этих символов
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        /*
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
         */ 
        /// <summary>
        /// Находит в строке S первое вхождение параметра обрамленного в ${ }
        /// и возвращает этот параметров без этих симоволов
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        /*
        static string GetStrVar(string S)
        {
            int beg = 0, end;
            string var = "";
            //string BegFrame = "${";
            //string EndFrame = "}";
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
         */
        /// <summary>
        /// Возвращает массив переменных строки без обрамляющих символов
        /// </summary>
        /// <param name="S"></param>
        /// <returns></returns>
        static string[] GetStrVars(string S)
        {
            int beg = 0, end;
            List<string> vars = new List<string>();
            string var;
            //string BegFrame = "${";
            //string EndFrame = "}";
            beg = S.IndexOf(BegFrame);

            while (beg < S.Length && beg > -1)
            //if (beg < S.Length && beg > -1)
            {
                
                end = S.IndexOf(EndFrame, beg + 1);
                if (end == -1) break;

                var = S.Substring(beg + BegFrame.Length, end - beg - EndFrame.Length);
                if (!vars.Contains(var,StringComparer.InvariantCultureIgnoreCase))
                    vars.Add(var);
                
            }
            return vars.ToArray();
        }

        /// <summary>
        /// Подстановка занчения параметра в строку. Параметр без скобок
        /// </summary>
        /// <param name="S"></param>
        /// <param name="ParamName"></param>
        /// <param name="ParamValue"></param>
        /// <returns></returns>
        static string ReplaceParam(string S, string ParamName, string ParamValue)
        {
            return S.Replace(BegFrame + ParamName + EndFrame, ParamValue);
        }



    }
}
