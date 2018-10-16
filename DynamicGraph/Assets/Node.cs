using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour {
    public int no;
    public float level = 1;
    public Text text;
    public ColorChanger thisColorChanger;
    public Vector3 desiredVelocity = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    bool isMouseOver = false;
    public void Update()
    {
        var pos = this.transform.position;
        pos.z = 0;
        this.transform.position = pos;
        if (this.velocity.magnitude > 10.0f || isMouseOver)
            text.gameObject.SetActive(true);
        else
            text.gameObject.SetActive(false);
    }
    public void OnMouseEnter()
    {
        isMouseOver = true;
    }
    public void OnMouseExit()
    {
        isMouseOver = false;
    }

}
