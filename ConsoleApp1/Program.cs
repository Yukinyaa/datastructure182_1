using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = @"C:\Users\woong\Desktop\KakaoTalkChats[1].txt";
            var c = new DynamicGraphFactory(dir);
            Thread.Sleep(1000);
            while (true)
            {
                if (c.GraphFinished)
                    break;
                Console.WriteLine(c.Lv0Progression + ", " + c.Lv1Progression + ", " + c.Lv2Progression);
                Thread.Sleep(100);
            }
            
        }
    }
}
