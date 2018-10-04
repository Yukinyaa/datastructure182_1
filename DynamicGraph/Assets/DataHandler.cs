using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour {

    public static string dirstr = "/storage/emulated/0/KakaoTalk/Chats/*.txt";

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
	
	// Update is called once per frame
	void Update () {
		
	}
}
