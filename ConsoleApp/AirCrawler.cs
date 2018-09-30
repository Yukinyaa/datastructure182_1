using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

class AirCrawler
{
    const string reqURL = @"https://www.airport.co.kr/www/extra/stats/domesticLineStats/layOut.do?menuId=403";
    const string method = "POST";

    
    static string MakeQuery(string strStYear = "2018", string strStMonth = "1", string strEnYear = "2018", 
        string strEnMonth = "9", string strStCity = "ALL", string strEnCity = "ALL", 
        string strRegular = "ALL", string strUse = "ALL", string strPassengerType = "ALL", string strCargoType = "ALL")
    {
        NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
        outgoingQueryString.Add("strStYear", strStYear);
        outgoingQueryString.Add("strStMonth", strStMonth);
        outgoingQueryString.Add("strEnYear", strEnYear);
        outgoingQueryString.Add("strEnMonth", strEnMonth);
        outgoingQueryString.Add("strStCity", strStCity);
        outgoingQueryString.Add("strEnCity", strEnCity);
        outgoingQueryString.Add("strRegular", strRegular);
        outgoingQueryString.Add("strUse", strUse);
        outgoingQueryString.Add("strPassengerType", strPassengerType);
        outgoingQueryString.Add("strCargoType", strCargoType);
        return outgoingQueryString.ToString();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="query">build using AirCrawler.MakeQuery, if null default</param>
    /// <returns></returns>
    public static string DoCrawl(string query = null)
    {
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(reqURL);
        byte[] postBytes = new UTF8Encoding().GetBytes(query??MakeQuery());
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = postBytes.Length;
        request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36";
        // add post data to request
        Stream postStream = request.GetRequestStream();
        postStream.Write(postBytes, 0, postBytes.Length);
        postStream.Flush();
        postStream.Close();

        string responseText = string.Empty;

        using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
        {
            HttpStatusCode status = resp.StatusCode;
            Console.WriteLine(status);  // 정상이면 "OK"

            Stream respStream = resp.GetResponseStream();
            using (StreamReader sr = new StreamReader(respStream))
            {
                responseText = sr.ReadToEnd();
            }
        }
        return responseText;
    }
}
