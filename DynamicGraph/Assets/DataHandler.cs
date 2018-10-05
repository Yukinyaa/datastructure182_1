using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour {

    //public static string dirstr = "/storage/emulated/0/KakaoTalk/Chats/*.txt";
    public static string dirstr = string dir = @"C:\Users\woong\Desktop\KakaoTalkChats[1].txt";
    // Use this for initialization
    void Start () {
        if (System.IO.File.Exists(dirstr))
        {
            Debug.Log("File loaded");
        }
        else
        {
            Debug.Log("File Not Found");
        }
    }

    
    public bool dataLoaded = false;
    [HideInInspector]
    public Dictionary<System.DateTime, Connections> dynamicGraph;

    IEnumerator DataLoadingCorourine()
    {
        var c = new DynamicGraphFactory(dirstr);
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (c.GraphFinished)
                break;
            Debug.Log(c.Lv0Progression + ", " + c.Lv1Progression + ", " + c.Lv2Progression);
            yield return new WaitForSeconds(0.1f);
        }
        dataLoaded = true;
        dynamicGraph = c.GetUnitTimeGraphSnpSot();
    }


	
	// Update is called once per frame
	void Update () {
		
	}
}
