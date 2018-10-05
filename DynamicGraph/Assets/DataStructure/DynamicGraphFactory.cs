using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Collections;

public class DynamicGraphFactory
{
    
    public string Title { get; private set; }
    public DateTime SaveDate { get; private set; }
    Thread allTalksUpdater;
    Dictionary<DateTime, Thread> unitTimeCommsSnapShotUpdater = new Dictionary<DateTime, Thread>();
    Thread dynamicGraphAccumulator;
    object progLock = new object();
    Progression prog = new Progression();
    class Progression
    {
        public int daysFoundFromFile = 0;
        public int lv1days = 0;
        public int lv2days = 0;
    }
    public bool Lv0Progression { get { return !allTalksUpdater.IsAlive; } }
    public float? Lv1Progression {
        get
        {
            return prog.lv1days;
        }
    }
    public float? Lv2Progression
    {
        get
        {
            return prog.lv2days;
        }
    }

    public bool GraphFinished { get; private set; }

    Dictionary<DateTime,List<SingleChat>> allTalks = new Dictionary<DateTime, List<SingleChat>>();
    Dictionary<DateTime, Connections> unitTimeCommsSnapShot = new Dictionary<DateTime, Connections>();
    Dictionary<DateTime, Connections> unitTimeGraphSnapShot = new Dictionary<DateTime, Connections>();

    public Dictionary<DateTime, Connections> GetUnitTimeGraphSnpSot()
    {
        return GraphFinished ? unitTimeCommsSnapShot : null;
    }

    public DynamicGraphFactory(string dir)
    {
        allTalksUpdater = new Thread(() => FileReaderTStart(dir));
        allTalksUpdater.Start();
        dynamicGraphAccumulator = new Thread(() => { DynamicGraphAccumulatorTstart(); });
        dynamicGraphAccumulator.Start();
    }
    
    
    bool started = false;
    void FileReaderTStart(string dir)
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
            else if (Regex.Match(line, "[0-9]{4}년 [0-9]{1,2}월 [0-9]{1,2}일 오[전+후] [0-9]{1,2}:[0-9]{1,2}$").Success)
            {
                if (readingDate != null)
                {
                    Thread DayReader = new Thread(new ParameterizedThreadStart(ReadDayTStart));
                    unitTimeCommsSnapShotUpdater.Add((DateTime)readingDate, DayReader);
                    lock (((ICollection)allTalks).SyncRoot)
                        allTalks.Add((DateTime)readingDate, todayTalk);
                    DayReader.Start(readingDate);
                    todayTalk = new List<SingleChat>();
                }
                readingDate = DateTime.Parse(line, MyCultureInfo).Date;
                lock(progLock)
                    prog.daysFoundFromFile++;
            }
            else if (Regex.Match(line, "[0-9]{4}년 [0-9]{1,2}월 [0-9]{1,2}일 오[전+후] [0-9]{1,2}:[0-9]{1,2}, .* : .*").Success)
            {
                var dt_text = line.Split(",".ToArray(), 2);
                var name_text = dt_text[1].Split(":".ToArray(), 2);
                var lastTalk = todayTalk.LastOrDefault();
                var currentName = new HumanName(name_text[0]);
                if (lastTalk != null && lastTalk.name == currentName)
                {
                    lastTalk.length += name_text[1].Length - 1;
                }
                    else 
                    todayTalk.Add(new SingleChat()
                    {
                        time = DateTime.Parse(dt_text[0], MyCultureInfo),
                        name = new HumanName(name_text[0]),
                        length = name_text[1].Length - 1
                    });
            }
            else continue;

        }
        { 
            Thread DayReader = new Thread(new ParameterizedThreadStart(ReadDayTStart));
            unitTimeCommsSnapShotUpdater.Add((DateTime)readingDate, DayReader);
            lock (((ICollection)allTalks).SyncRoot)
                allTalks.Add((DateTime)readingDate, todayTalk);
            DayReader.Start(readingDate);
            lock (progLock)
                prog.daysFoundFromFile++;
        }
    }

    //chats of today, yesterday
    class TalkBank
    {
        public List<SingleChat> prevDayChat = new List<SingleChat>();
        public List<SingleChat> todayChat = null;
        public SingleChat GetChat(int index, int offset = 0)
        {
            int target = index - offset;
            if (target < 0)
                return prevDayChat.Count - 1 < -target ? null : prevDayChat[-target];
            else
                return todayChat.Count - 1 < target ? null : todayChat[target];
        }
    }
    void ReadDayTStart(object readDayObj)
    {
        DateTime readingTimeSpan = (DateTime)readDayObj;
        Connections c = new Connections();
        TalkBank talkBank = new TalkBank();

        //initalize TalkBank
        lock (((ICollection)allTalks).SyncRoot)
        {
            if (allTalks.First().Key != readingTimeSpan)
            {
                var lastDayKeyValuePair = allTalks.FirstOrDefault(e => e.Key == readingTimeSpan.AddDays(-1));
                talkBank.prevDayChat = new List<SingleChat>(lastDayKeyValuePair.Value);
            }
            talkBank.todayChat = new List<SingleChat>(allTalks[readingTimeSpan]);
        }

        Connections currentConn = new Connections();
        
        for (int i = 0; i < talkBank.todayChat.Count; i++)
        {
            var thisChat = talkBank.GetChat(i);
            DateTime scanChatTime;
            int scanOffset = 1;
            var scanChat = talkBank.GetChat(i);

            if ((thisChat.time - readingTimeSpan).TotalHours > DataConsts.UnitTime)
            {
                lock (((ICollection)unitTimeCommsSnapShot).SyncRoot)
                {
                    unitTimeCommsSnapShot.Add(readingTimeSpan, currentConn);
                }
                currentConn = new Connections();
                readingTimeSpan = readingTimeSpan.AddHours(DataConsts.UnitTime);
            }

            do
            {
                scanChat = talkBank.GetChat(i, scanOffset);
                if (scanChat == null) break;
                int tstrength = (int)(100 / ((thisChat.time - scanChat.time).TotalMinutes + scanOffset));
                if (thisChat.name != scanChat.name)
                    currentConn.Add(new Connection()
                    {
                        a = thisChat.name,
                        b = scanChat.name,
                        strength = tstrength
                    });
                scanOffset++;
                if (tstrength <= 1)
                    break;
            } while(true);

        }
        lock (((ICollection)unitTimeCommsSnapShot).SyncRoot)
        {
            unitTimeCommsSnapShot.Add(readingTimeSpan, currentConn);
        }

        lock (progLock)
            prog.lv1days++;
    }
    void DynamicGraphAccumulatorTstart()
    {

        DateTime readFrom = default(DateTime);
        DateTime readTo = default(DateTime);
        int allTalkIndex=0;
        var CurrentConn = new Connections();
        while (true)//accumulate one day
        {
            while (true)//wait for dayTalk insert, if there is no dayTalk to read and wait, exit.
            {
                lock (((ICollection)allTalks).SyncRoot)
                {
                    if (allTalks.Count > allTalkIndex + 1)
                        break;
                    else if (allTalksUpdater.IsAlive == false && allTalks.Count > allTalkIndex)
                    {
                        GraphFinished = true;
                        return;
                    }
                }
                Thread.Sleep(100);
            }
            allTalkIndex++;
            if (default(DateTime) == readFrom)
                lock (((ICollection)allTalks).SyncRoot)
                    readFrom = allTalks.First().Key; //first day.
            lock (((ICollection)allTalks).SyncRoot)
                readTo = allTalks.ElementAt(allTalkIndex).Key.AddDays(1);
            while ((readFrom - readTo).Hours <= 0)
            {
                try
                {
                    while (true)
                    {
                        lock (((ICollection)unitTimeCommsSnapShotUpdater).SyncRoot)
                            if (!unitTimeCommsSnapShotUpdater[readFrom].IsAlive)
                                break;
                    }
                    CurrentConn.Add(unitTimeCommsSnapShot[readFrom]);

                    lock (progLock)
                        prog.lv2days++;
                }
                catch (KeyNotFoundException) { }
                CurrentConn.Decay(DataConsts.UnitTime);
                unitTimeGraphSnapShot.Add(readFrom = readFrom.AddHours(DataConsts.UnitTime), CurrentConn.Copy());
                prog.lv2days++;
            }
        }
        
    }
    
    
}
