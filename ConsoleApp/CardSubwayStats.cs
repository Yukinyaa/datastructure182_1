using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace ConsoleApp
{
    class CardSubwayStats
    {
        class CardSubwayStatsNew_Raw
        {
            public class CardSubwayStatsNewData
            {
                public class CardSubwayStatsNewDataRow
                {
                    public string USE_DT { get; set; }
                    public string LINE_NUM { get; set; }
                    public string SUB_STA_NM { get; set; }
                    public float RIDE_PASGR_NUM { get; set; }
                    public float ALIGHT_PASGR_NUM { get; set; }
                    public string WORK_DT { get; set; }
                }

                public int list_total_count { get; set; }
                public class Result
                {
                    public string CODE { get; set; }
                    public string MESSAGE { get; set; }
                }
                public Result RESULT;
                public List<CardSubwayStatsNewDataRow> row;
            }
            public CardSubwayStatsNewData CardSubwayStatsNew;
        }
        
        public class CardSubwayStatsRow
        {
            public DateTime dt;
            public int lineNum;
            public string stationName;
            public int onCnt;
            public int offCnt;
        }
        public List<CardSubwayStatsRow> onOffData;

        public CardSubwayStats(string ss)
        {
            var rawPharse = JsonConvert.DeserializeObject<CardSubwayStatsNew_Raw>(ss);
            if (!rawPharse.CardSubwayStatsNew.RESULT.CODE.Equals("INFO-000"))
                throw new ArgumentException();

            foreach (var row in rawPharse.CardSubwayStatsNew.row)
            {
                onOffData.Add(new CardSubwayStatsRow())
            }
        }
    }
}
