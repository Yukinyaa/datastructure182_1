using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class HumanName
{
    static List<string> names = new List<string>();
    static void Reset() { names = new List<string>(); }
    public int code;
    public int Code { get { return code; } }
    public override string ToString() { return names[code]; }

    static public string GetNameByCode(int i)
    {
        try
        {
            lock (((ICollection)names).SyncRoot)
                return names[i];
        }
        catch (ArgumentOutOfRangeException) { return "unknown"; }
    }
    public HumanName(string nameStr)
    {
        lock (((ICollection)names).SyncRoot)
        {
            var find = names.FirstOrDefault(name => name.Equals(nameStr));

            if (find == null)
                names.Add(nameStr);

            this.code = names.IndexOf(nameStr);
        }
    }
    
    static public bool operator==(HumanName a, HumanName b)
    {
        return a.Code == b.Code;
    }
    static public bool operator!=(HumanName a, HumanName b)
    {
        return a.Code != b.Code;
    }
}