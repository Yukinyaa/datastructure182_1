using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = @"C:\Users\woong\Desktop\KakaoTalkChats[1].txt";
            new DynamicGraph(dir);
        }
    }
}
