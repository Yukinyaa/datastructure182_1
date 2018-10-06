using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class DataHandler : MonoBehaviour {

    //public static string dirstr = "/storage/emulated/0/KakaoTalk/Chats/*.txt";
    //public static string dirstr = @"C:\Users\woong\Desktop\팩린이뉴들박 33 님과 카카오톡 대화.txt";
    //public static string dirstr = @"C:\Users\woong\Desktop\킹스레이드 스타더스트 길드 31 님과 카카오톡 대화.txt";
    public static string dirstr = @"C:\Users\woong\Desktop\KakaoTalkChats[1].txt";
    //팩린이뉴들박 33 님과 카카오톡 대화.txtKakaoTalkChats[1].txt


    public GameObject nodePF;

    public float logScale = 10;//vewing distance logarithm scale
    public float graphScale = 10;
    [Range(0.3f, 10.0f)]
    public float PlaybackSpeed = 3;

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
        StartCoroutine(DataLoadingCorourine());
    }

    
    public bool dataLoaded = false;
    [HideInInspector]
    public Dictionary<System.DateTime, Connections> dynamicGraph;
    public string status;
    Node[ ] nodes;
    int maxCount;
    
    Connections currentGraph;

    IEnumerator DataLoadingCorourine()
    {
        var c = new DynamicGraphFactory(dirstr);
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (c.GraphFinished)
                break;
            status = c.Lv0Progression + ", " + c.Lv1Progression + ", " + c.Lv2Progression;
            yield return null;
        }
        dataLoaded = true;
        dynamicGraph = c.GetUnitTimeGraphSnapShot();

        maxCount = dynamicGraph.Values.Last().MatrixSize + 1;
        nodes = new Node[maxCount];
        status = "Creating Nodes";
        for (int i = 0; i < maxCount; i++)
        {
            { 
                var obj = Instantiate(nodePF,
                    new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), Random.Range(-1f, 1)),
                    Quaternion.identity
                    );
                var node = obj.GetComponent<Node>();
                nodes[i] = node;
                node.no = i;
                node.name = HumanName.GetNameByCode(i);
            }
        }

        bool updateNodeStarted = false;
        foreach (var graphSnapShot in dynamicGraph)
        {
            status = graphSnapShot.Key.ToString();
            currentGraph = graphSnapShot.Value;
            
            if (updateNodeStarted == false)
                StartCoroutine(UpdateNode());
            updateNodeStarted = true;
            yield return new WaitForSeconds(PlaybackSpeed);
        }
    }
    IEnumerator UpdateNode()
    {
        while (true)
        {
            currentGraph.ExpandMatrix(maxCount);
            for (int i = 0; i < maxCount; i++)
            {
                nodes[i].velocity *= 0.80f;
                nodes[i].velocity += nodes[i].desiredVelocity * 0.05f;
                nodes[i].transform.position += nodes[i].velocity * Time.deltaTime;
                nodes[i].desiredVelocity = Vector3.zero;
                
            }
            
            for (int i = 0; i < maxCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    var vertexVector = nodes[j].transform.position - nodes[i].transform.position;
                    if (vertexVector.magnitude == 0)
                    {
                        vertexVector = new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), Random.Range(-1f, 1));
                    }
                    //Debug.Log(Mathf.Log(currentGraph.Matrix[i, j], logScale));
                    var desired = (1000/i) * graphScale;// 1000 ^ (log_logScale_x)
                    //Debug.Log(desired);
                    if (float.IsInfinity(desired) || float.IsNaN(desired))
                        desired = 1000;
                    var real = vertexVector.magnitude;
                    var pullStrength = real- desired;
                    if (pullStrength < 0)
                    {
                        //pullStrength /= 10;
                        pullStrength *= -pullStrength;
                    }
                    
                    nodes[i].desiredVelocity += vertexVector.normalized * pullStrength;
                    nodes[j].desiredVelocity -= vertexVector.normalized * pullStrength;
                    
                }
            }
            yield return null;
        }
    }



	
	// Update is called once per frame
	void Update () {
		
	}
}
