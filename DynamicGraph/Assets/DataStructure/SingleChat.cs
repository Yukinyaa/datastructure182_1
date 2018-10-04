using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class SingleChat
{
    public override string ToString()
    {
        return time + ", " + name + ": " + length;
    }
    public DateTime time;
    public HumanName name;
    public int length;
}
