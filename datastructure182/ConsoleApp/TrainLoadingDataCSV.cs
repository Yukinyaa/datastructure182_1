using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TrainLoadingDataCSV
{

    public class Row
    {
        readonly string[] fieldNames;
        protected string from_station;
        #region rowField
        public string From_Station
        {
            get { return from_station; }
            protected set { from_station = value; }
        }
        protected string to_station;
        public string To_Station
        {
            get { return to_station; }
            protected set { to_station = value; }
        }
        protected string line;
        public string Line
        {
            get { return line; }
            protected set { line = value; }
        }
        protected string line_direction;
        public string Line_Direction
        {
            get { return line_direction; }
            protected set { line_direction = value; }
        }
        protected string naptan_from;
        public string Naptan_From
        {
            get { return naptan_from; }
            protected set { naptan_from = value; }
        }
        protected string naptan_to;
        public string Naptan_To
        {
            get { return naptan_to; }
            protected set { naptan_to = value; }
        }
        #endregion
        protected Dictionary<int, int?> crowdingdata = new Dictionary<int, int?>();
        int? this[int n]
        {
            get { return crowdingdata[n]; }
        }
        
        public Row(string[] fieldNames, string[] csvRow)
        {
            //[0-9][0-9][0-9]-[0-9][0-9][0-9]$
            
            for (int i = 0; i < fieldNames.Length; i++) 
            {
                if (Regex.Match(fieldNames[i], @"[0-9][0-9][0-9][0-9]-[0-9][0-9][0-9][0-9]$").Success)
                {
                    bool nameSucess = Int32.TryParse(fieldNames[i].Split('-')[0], out int nameresult);
                    if (!nameSucess)
                        throw new Exception("Data Corrupted");
                    int? element = null;
                    if (!csvRow[i].Equals(""))
                    {
                        bool elementSucess = Int32.TryParse(csvRow[i], out int elemresult);
                        if (elementSucess)
                            element = elemresult;
                    }
                    crowdingdata.Add(nameresult, element);
                }
                else
                {
                    string value = csvRow[i].ToString();
                    switch (fieldNames[i].ToLower())
                    {
                        case "from_station":
                            from_station = value;
                            break;
                        case "to_station":
                            to_station = value;
                            break;
                        case "line":
                            line = value;
                            break;
                        case "line_direction":
                            line_direction = value;
                            break;
                        case "naptan_from":
                            naptan_from = value;
                            break;
                        case "naptan_to":
                            naptan_to = value;
                            break;
                    }
                }
            }
        }
    }
    readonly string[] fieldNames;
    List<Row> rows = new List<Row>();
    public TrainLoadingDataCSV(string[] fieldNames)
    {
        this.fieldNames = fieldNames;
    }
    public TrainLoadingDataCSV(string filename = @"./Data/LUTrainLoadingData.csv")
    {
        //read csv
        bool isFirstRow = true;
        using (TextFieldParser parser = new TextFieldParser(filename))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            while (!parser.EndOfData)
            {
                //Read Row
                string[] fields = parser.ReadFields();
                
                //Process row
                if (isFirstRow)
                    fieldNames = fields;
                else
                    rows.Add(new Row(fieldNames, fields));
                isFirstRow = false;
            }
        }
    }
    

}
