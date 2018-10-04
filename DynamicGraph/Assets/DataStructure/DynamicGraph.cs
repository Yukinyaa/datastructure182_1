using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;

public class DynamicGraph
{
    
    public string Title { get; private set; }
    public DateTime SaveDate { get; private set; }

    Thread fileReader;
    Dictionary<DateTime, Thread> dayUpdater = new Dictionary<DateTime, Thread>();
    Thread graphUpdater;

    Dictionary<DateTime,List<SingleChat>> dayTalk = new Dictionary<DateTime, List<SingleChat>>();
    Dictionary<DateTime, Connections> unitTimeTotalSnapShot = new Dictionary<DateTime, Connections>();
    Dictionary<DateTime, Connections> unitTimeGraphSnapShot = new Dictionary<DateTime, Connections>();

    public DynamicGraph(string dir)
    {
        fileReader = new Thread(() => ReadStart(dir));
        fileReader.Start();
    }
    bool started = false;
    void ReadStart(string dir)
    {
        if (started) return;
        started = true;



        StreamReader file =
            new StreamReader(dir);
        CultureInfo MyCultureInfo = new CultureInfo("ko-KR");


        Title = file.ReadLine();


        var dateTime = file.ReadLine().Split(":".ToArray(), 2)[1];
        SaveDate = DateTime.Parse(dateTime, MyCultureInfo);

        string line;
        DateTime? readingDate = null;
        var todayTalk = new List<SingleChat>();

        while (!file.EndOfStream)
        {
            line = file.ReadLine();
            if (line.Equals(""))
                continue;
            else if (Regex.Match(line, "[0-9]{4}년 [0-9]{1,2}월 [0-9]{1,2}일 오[전+후] [0-9]{1,2}:[0-9]{1,2}[\\s]*$").Success)
            {
                if (readingDate != null)
                {
                    dayTalk.Add((DateTime)readingDate, todayTalk);
                    Thread DayReader = new Thread(() => { ReadDay((DateTime)readingDate, todayTalk); });
                    dayUpdater.Add((DateTime)readingDate, DayReader);
                    DayReader.Start();
                }
                try
                {
                    readingDate = DateTime.Parse(line, MyCultureInfo).Date;
                }
                catch (System.FormatException) { }
            }
            else if (Regex.Match(line, "[0-9]{4}년 [0-9]{1,2}월 [0-9]{1,2}일 오[전+후] [0-9]{1,2}:[0-9]{1,2}, .* : .*").Success)
            {
                var dt_text = line.Split(",".ToArray(), 2);
                var name_text = dt_text[1].Split(":".ToArray(), 2);
                todayTalk.Add(new SingleChat()
                {
                    time = DateTime.Parse(dt_text[0], MyCultureInfo),
                    name = new HumanName(name_text[0]),
                    length = name_text[1].Length - 1
                });
            }
            else continue;

        }
        dayTalk.Add((DateTime)readingDate, todayTalk);
        Thread DayReadert = new Thread(() => { ReadDay((DateTime)readingDate, todayTalk); });
        dayUpdater.Add(readingDate ?? DateTime.Now, DayReadert);
        DayReadert.Start();
    }
    void ReadDay(DateTime readDay, List<SingleChat> chats)
    {

        Connections c = new Connections();
        List<SingleChat> prevDayChat = null;
        
        if (dayTalk.First().Key != readDay)
        {
            var lastDayKeyValuePair = dayTalk.FirstOrDefault(e => e.Key == readDay.AddDays(-1));
            prevDayChat = lastDayKeyValuePair.Value;
        }

        Connections currentConn = new Connections();
        DateTime dateTimeReading = readDay;
        {

        }
        //unitTimeTotalSnapShot.Add(dateTimeReading, currentConn);


    }
}
