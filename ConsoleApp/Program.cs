using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.Collections.Generic;

class Program
{
    struct AirInfo
    {
        public string fromTo;
        public string companyCode;

        public string flightDepart;
        public string flightArrive;
        public string flightSum;

        public string personDepart;
        public string personArrive;
        public string personSum;

        public string freightDepart;
        public string freightArrive;
        public string freightSum;
    }
    static void Main(string[] args)
    {
        var data = AirCrawler.DoCrawl();
        var doc = new HtmlDocument();
        doc.LoadHtml(data);
        var div = doc.GetElementbyId("statsForm").
            ChildNodes.Where(a => a.HasClass("section")).ToList()[3].
            ChildNodes["div"];

        var table = div.ChildNodes["table"];
        var tbody = table.ChildNodes["tbody"];

        var airVertex = new List<AirInfo>();

        int rowspan = 0;
        foreach (var tr in tbody.ChildNodes.Where(a=>a.Name.Equals("tr")).ToList())
        {
            var airInfo = new AirInfo();
            string th = "";
            var childNodes = tr.ChildNodes.Where(a => !a.Name.Equals("#text")).ToList();
            for (int i = 0; i < childNodes.Count; i++)
            {
                var td = childNodes[i];
                if (td.Name.Equals("th"))
                {
                    th = td.InnerText;//td is actually th
                    childNodes.Remove(td);
                    i--;
                }
                switch (i)
                {
                    case 0:
                        airInfo.companyCode = td.InnerText;
                        break;
                    case 1:
                        airInfo.flightDepart = td.InnerText;
                        break;
                    case 2:
                        airInfo.flightArrive = td.InnerText;
                        break;
                    case 3:
                        airInfo.flightSum = td.InnerText;
                        break;
                    case 4:
                        airInfo.personDepart = td.InnerText;
                        break;
                    case 5:
                        airInfo.personArrive = td.InnerText;
                        break;
                    case 6:
                        airInfo.personSum = td.InnerText;
                        break;
                    case 7:
                        airInfo.freightDepart = td.InnerText;
                        break;
                    case 8:
                        airInfo.freightArrive = td.InnerText;
                        break;
                    case 9:
                        airInfo.freightSum = td.InnerText;
                        break;
                }
            }
            airInfo.fromTo = th;
            airVertex.Add(airInfo);
        }

        string json = JsonConvert.SerializeObject(airVertex);
    }
}
