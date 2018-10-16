using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        //public static string dirstr = "/storage/emulated/0/KakaoTalk/Chats/*.txt";
        //public static string dirstr = @"C:\Users\woong\Desktop\팩린이뉴들박 33 님과 카카오톡 대화.txt";
        public static string dirstr = @"C:\Users\woong\Desktop\킹스레이드 스타더스트 길드 31 님과 카카오톡 대화.txt";
        //public static string dirstr = @"C:\Users\woong\Desktop\KakaoTalkChats[1].txt";
        //팩린이뉴들박 33 님과 카카오톡 대화.txtKakaoTalkChats[1].txt

        static void Main(string[] args)
        {
            var c = new DynamicGraphFactory(dirstr);
            Thread.Sleep(1000);
            while (true)
            {
                Console.WriteLine(c.Lv0Progression + ", " + c.Lv1Progression + ", " + c.Lv2Progression);
                if (c.GraphFinished)
                    break;
                Thread.Sleep(100);
            }


            DateTime maxDay = c.GetUnitTimeCommsSnapShot().Max(k => k.Key);


            var data = c.GetUnitTimeCommsSnapShot();
            int maxNode = data.Last().Value.MatrixSize;

            int edgeid = 0;//edgeid++;
            //c.GetUnitTimeCommsSnapShot()[]
            var nodes = new StringBuilder().Append("Id,").Append("Label,").Append("Timestamp,").Append("Score\n");
            var edges = new StringBuilder().Append("Source,").Append("Target,").Append("Type,").Append("id,").Append("Timestamp,").Append("weightdynamic\n");

            for (int nodeid = 0; nodeid < maxNode; nodeid++)
            {
                {
                    nodes.Append(nodeid).Append(',').Append(HumanName.GetNameByCode(nodeid)).Append(',').Append("\"<[");
                    bool isremove = false;
                    foreach (var timeset in data)
                    {
                        isremove = true;
                        if (timeset.Value.MatrixSize > nodeid)
                            nodes.Append(timeset.Key.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")).Append(",");
                    }
                    if(isremove)
                        nodes.Remove(nodes.Length - 1, 1);
                    nodes.Append("]>\",");
                    nodes.Append("\"<");
                    foreach (var timeset in data)
                    {
                        if (timeset.Value.MatrixSize > nodeid)
                        {
                            nodes.Append("[");
                            nodes.Append(timeset.Key.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")).Append(",");
                            nodes.Append(timeset.Value.GetScore(nodeid));
                            nodes.Append("];");
                        }

                    }
                    nodes.Append(">\"");
                    nodes.Append("\n");
                }
                {
                    for (int targetnode = 0; targetnode < nodeid; targetnode++)
                    {
                        edges.Append(nodeid).Append(',').Append(targetnode).Append(',').Append("Undirected").Append(',').Append(edgeid++).Append(" ,").Append("\"<[");
                        bool isremove = false;
                        foreach (var timeset in data)
                        {
                            isremove = true;
                            if (timeset.Value.MatrixSize > nodeid)
                                edges.Append(timeset.Key.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")).Append(",");
                        }
                        if (isremove)
                            edges.Length--;
                        edges.Append("]>\",");



                        edges.Append("\"<");
                        foreach (var timeset in data)
                        {
                            if (timeset.Value.GetConnectionBetween(targetnode, nodeid) > 0)
                            {
                                edges.Append("[");
                                edges.Append(timeset.Key.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz")).Append(",");
                                edges.Append(timeset.Value.GetConnectionBetween(targetnode, nodeid));
                                edges.Append("];");
                            }

                        }
                        edges.Append(">\"");
                        edges.Append("\n");
                    }

                }
            }


            File.WriteAllText(@"D:\Data\edges.csv", edges.ToString());
            File.WriteAllText(@"D:\Data\nodes.csv", nodes.ToString());




        }
    }
}
