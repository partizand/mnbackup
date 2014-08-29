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
            bool ret;
            //FileManage.DirectoryCreate(@"d:\temp\mnBackupTest");
            ret=Param.CreateTestDir();
            
            Param.CreateFileToTest("test.txt");
            
            Param.CreateFileToTest("test.log");
            
            Param.CreateFileToTest("test.zip");
            
            Param.CreateFileToTest("mtest.zip");
            
            Param.CreateFileToTest("test.7z");
        }
    }
}
