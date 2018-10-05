public class Connection
{
    public bool Match(HumanName aa, HumanName bb)
    {
        if ((aa == a && bb == b) || (bb == a && aa == b))
            return true;
        else
            return false;
    }
    public Connection() { }
    public Connection(Connection c) {
        this.strength = c.strength;
        this.a = c.a;
        this.b = c.b;
    }
    public float strength;
    public HumanName a, b;
}