using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mnBackupLib;

namespace mnBackupTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Проверка чтения периода
            Period per = new Period("2d");
            

            per = new Period("week");
            

            per = new Period("3Month");
            
        }
    }
}
