using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using mnBackupLib;


namespace mnBackupTest
{
    [TestFixture]
    public class PeriodTest
    {
        [Test]
        public void TestPeriod()
        {
            // Проверка чтения периода
            TimePeriod per = new TimePeriod(" 2d ");
            Assert.AreEqual(2,per.Range.Days,"2 Дня");

            per = new TimePeriod(" ");
            Assert.AreEqual(true, per.isEmpty(), "пустой период");

            per = new TimePeriod("vasya");
            Assert.AreEqual(true, per.isEmpty(), "Неправильная строка");

            per = new TimePeriod("15");
            Assert.AreEqual(15, per.Range.Days, "15 дней");

            per = new TimePeriod("week");
            Assert.AreEqual(1, per.Range.Weeks, "1 неделя");
            

            per = new TimePeriod("3Month");
            Assert.AreEqual(3, per.Range.Months, "3 months");
            

            

            // Прибавить отнять даты
            per = new TimePeriod("3Day");
            DateTime dTest=new DateTime(2014,08,20); // Исходная дата
            DateTime dRes=new DateTime(2014,08,23); // Дата должна получится
            DateTime dt = per.AddToDate(dTest); // Вычисляемая дата
            Assert.AreEqual(dRes, dt, "Прибавление интервала к дате");

            
            dRes = new DateTime(2014, 08, 17); // Дата должна получится
            dt = per.SubFromDate(dTest); // Вычисляемая дата
            Assert.AreEqual(dRes, dt, "Вычитание интервала из даты");
            

        }
    }
}
