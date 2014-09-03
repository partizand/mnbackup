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
            Period per = new Period(" 2d ");
            Assert.AreEqual(Period.PeriodName.Day, per.IntervalName, "Имя периода День");
            Assert.AreEqual(2, per.IntervalValue, "Значение 2");

            per = new Period("week");
            Assert.AreEqual(Period.PeriodName.Week, per.IntervalName, "Имя периода");
            Assert.AreEqual(1, per.IntervalValue, "Значение периода");

            per = new Period("3Month");
            Assert.AreEqual(Period.PeriodName.Month, per.IntervalName, "Имя периода");
            Assert.AreEqual(3, per.IntervalValue, "Значение периода");

            per = new Period(Period.PeriodName.Month);
            Assert.AreEqual(Period.PeriodName.Month, per.IntervalName, "Имя периода");
            Assert.AreEqual(1, per.IntervalValue, "Значение периода");

            // Прибавить отнять даты
            per = new Period("3Day");
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
