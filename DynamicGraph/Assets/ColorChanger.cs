using UnityEngine;

public class ColorChanger : MonoBehaviour {
    public Color c;
    public Color prevColor;
    public Renderer renderer;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (c == prevColor)
            return;
        prevColor = c;
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetColor("_Color", c);
        var child = transform.Find("Cylinder");
        if(child == null)
            GetComponent<Renderer>().SetPropertyBlock(props);
        else 
            child.GetComponent<Renderer>().SetPropertyBlock(props);

    }
}
