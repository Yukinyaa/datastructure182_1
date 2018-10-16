using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEditor;

public class DataHandler : MonoBehaviour {

    //public static string dirstr = "/storage/emulated/0/KakaoTalk/Chats/*.txt";
    //public static string dirstr = @"C:\Users\woong\Desktop\팩린이뉴들박 33 님과 카카오톡 대화.txt";
    //public static string dirstr = @"C:\Users\woong\Desktop\킹스레이드 스타더스트 길드 31 님과 카카오톡 대화.txt";
    public static string dirstr = @"talk.txt";
    //팩린이뉴들박 33 님과 카카오톡 대화.txtKakaoTalkChats[1].txt


    public GameObject nodePF;
    public GameObject canvas;

    //public float logScale = 10;//vewing distance logarithm scale
    [Range(10.0f, 50.0f)]
    public float graphRadius = 100f;
    [Range(0.1f, 10.0f)]
    public float distanceScale = 0.1f;
    [Range(0.1f, 10.0f)]
    public float PlaybackSpeed = 0.1f;

    [Range(0.3f, 10.0f)]
    public float levelScale = 1f;

    [Range(0.1f, 100f)]
    public float gravity = 9.8f;

    [Range(0.01f, 1.0f)]
    public float attractionScale = 0.1f;

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
                    Quaternion.identity,
                    canvas.transform
                    );
                var node = obj.GetComponent<Node>();
                nodes[i] = node;
                node.no = i;
                node.text.text = node.name = HumanName.GetNameByCode(i);
                
                if (node.name.Equals(" 회원님 "))
                    node.thisColorChanger.c = Color.red;
            }
        }

        bool updateNodeStarted = false;
        foreach (var graphSnapShot in c.GetUnitTimeGraphSnapShot())
        {
            status = graphSnapShot.Key.ToString();
            currentGraph = graphSnapShot.Value;
            currentGraph.ExpandMatrix(maxCount);

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
            for (int i = 0; i < maxCount; i++)
            {
                nodes[i].velocity *= 0.80f;
                nodes[i].velocity += nodes[i].desiredVelocity * 0.05f;
                nodes[i].transform.position += nodes[i].velocity * attractionScale * Time.deltaTime;
                

                
                nodes[i].desiredVelocity = Vector3.zero;
                    nodes[i].desiredVelocity -= nodes[i].transform.position.normalized * (nodes[i].transform.position.magnitude - (graphRadius /  Mathf.Sqrt(nodes[i].level)) ) * gravity;//gravity
                nodes[i].level = 1;
            }

            //bool[] isIsolated = new bool[maxCount];
            for (int i = 0; i < maxCount; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    var vertexVector = nodes[j].transform.position - nodes[i].transform.position;
                    if (vertexVector.magnitude == 0)
                    {
                        vertexVector = new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), Random.Range(-1f, 1));
                    }
                    //if (currentGraph.Matrix[i, j] < 1)// if no connection > no force
                    //    continue;
                    var desiredDistance = 100 / (currentGraph.Matrix[i, j] + 1) * distanceScale;// 1000 ^ (log_logScale_x)
                    
                    //Debug.Log(desired);
                    if (float.IsInfinity(desiredDistance) || float.IsNaN(desiredDistance))
                        continue;
                    var nowDistance = vertexVector.magnitude;
                    var pullStrength = nowDistance- desiredDistance;

                    if (pullStrength < 0)//강하게 밀기
                        pullStrength *= 3;
                    else
                        pullStrength *= 1;
                    
                    nodes[i].desiredVelocity += vertexVector.normalized * pullStrength;
                    nodes[j].desiredVelocity -= vertexVector.normalized * pullStrength;
                    nodes[i].level += currentGraph.Matrix[i, j] / levelScale;
                    nodes[j].level += currentGraph.Matrix[i, j] / levelScale;


                }
            }
            yield return null;
        }
    }



	
	// Update is called once per frame
	void Update () {
		
	}
}
