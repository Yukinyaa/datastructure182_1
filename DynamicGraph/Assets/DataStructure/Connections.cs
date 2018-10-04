using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Connections
{
    public List<Connection> connections = new List<Connection>();

    public void Add(Connections c)
    {
        foreach (var cc in c.connections)
            Add(cc);
    }
    public void Add(Connection c)
    {
        var find = connections.FirstOrDefault((a) => a.Match(c.a, c.b));

        if (find == null)
        {
            c.strength = (int)Mathf.Log(c.strength, DataConsts.logScale);
            connections.Add(c);
        }
        else
        {
            find.strength += c.strength;
        }
    }
    public void Decay(int durationh)
    {
        connections.RemoveAll(c => c.strength < 0.1f);
        foreach (var c in connections)
            c.strength = (int)Mathf.Pow(c.strength, -Mathf.Pow(2, (float)durationh / DataConsts.DecayRate));
        
    }
}
