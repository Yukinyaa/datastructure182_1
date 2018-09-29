using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Net;
using System.IO;

class OpenAPIDataReader
{
    protected static string key;
    static public void SetKey(string key)
    {
        OpenAPIDataReader.key = key;
    }

    static public void GetData(string dateTime = "20180920", string key = null)
    {
        if (key == null)
            key = OpenAPIDataReader.key;

        var cli = new WebClient
        {
            Encoding = Encoding.UTF8
        };
        string data = cli.DownloadString(@"http://openapi.seoul.go.kr:8088/" + key + @"+/json/CardSubwayStatsNew/1/1000/20180920");
    }
    

}