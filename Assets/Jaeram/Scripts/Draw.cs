using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System.IO;
using System;


using Firebase;
using Firebase.Database;
//using Firebase.Storage;

public class Draw : MonoBehaviour
{
    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";
    EventTrigger eTrigger;
    LineRenderer lr;
    Vector3[] lineVertices = new Vector3[10000000];
   // Vector3[] lineVerticestoAdd = new Vector3[2];
    Vector3 verticePos;
    //public float lineDist=5;
    public int verticeIdx = 0;
    int lineNumb = 0;
    GameObject graffiti;
    public Color lineColor=Color.white;
    GameObject colorSelection;
    public Image[] buttonColors=new Image[8];
    public bool isDrawingButtonTouched = false;
    public string dateName;

    public static Draw instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        dateName = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBUrl);
    }
    // Start is called before the first frame update
    void Start()
    {
        //lr =GameObject.Find("Line").GetComponent<LineRenderer>();
        //lr.enabled = false;
        graffiti = GameObject.Find("Graffiti");
        colorSelection= GameObject.Find("ColorSelection");

        
        
    }

    // Update is called once per frame
    void Update()
    {
        
        DrawLine();
       
     
    }
    LineRendererData linedata;
    GameObject lineDrawer;
    private void DrawLine()
    {
        if (isDrawingButtonTouched && Input.touchCount > 0 && !EventSystem.current.currentSelectedGameObject)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                lineDrawer = new GameObject($"LineDrawer{lineNumb}");
                lineDrawer.transform.SetParent(graffiti.transform);
                lineDrawer.AddComponent<LineRenderer>();
                //라인 렌더러를 추가하면 기본 매터리얼이 없으니 추가한다.
                Material whiteDiffuseMat = new Material(Shader.Find("Sprites/Default"));
                lr = lineDrawer.GetComponent<LineRenderer>();
                lr.material = whiteDiffuseMat;
                //lr.material.color = Color.white;
                lr.material.color = lineColor;
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
                lr.numCornerVertices = 5;
                lr.numCapVertices = 5;

                linedata = new LineRendererData($"LineDrawer{lineNumb}");

                for (int i = 0; i < buttonColors.Length; i++)
                {
                    StartCoroutine(Hide(buttonColors[i]));
                }



            }
            //움직임에 따라 라인이 그려진다

            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                Touch touch = Input.GetTouch(0);
                verticePos = new Vector3(touch.position.x, touch.position.y, 0);
                verticePos = Camera.main.ScreenToWorldPoint(new Vector3(verticePos.x, verticePos.y, Camera.main.nearClipPlane)) + Camera.main.transform.forward * 2;
                lineVertices[verticeIdx] = verticePos;
                lr.positionCount = verticeIdx + 1;
                lr.SetPosition(verticeIdx, verticePos);
                verticeIdx++;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                SaveLineRendererData();
                if (verticeIdx != 0)
                {
                    verticeIdx = 0;
                    lineNumb++;
                }

                for (int i = 0; i < buttonColors.Length; i++)
                {
                    StartCoroutine(Show(buttonColors[i]));
                }

                isDrawingButtonTouched = false;

            }


        }
    }

    void SaveLineRendererData()
    {
        
        //라인렌더러 색 설정
        linedata.lineRendColorR=lineColor.r.ToString();
        linedata.lineRendColorG = lineColor.g.ToString();
        linedata.lineRendColorB = lineColor.b.ToString();
        //라인렌더러 부모 오브젝트의 위치 설정
        linedata.objectPosX=lineDrawer.transform.position.x.ToString();
        linedata.objectPosY = lineDrawer.transform.position.y.ToString();
        linedata.objectPosZ = lineDrawer.transform.position.z.ToString();
        
        //라인렌더러 정점 갯수 저장
        linedata.verticiesNum = lr.positionCount;


        for (int i = 0; i < linedata.verticiesNum; i++)
        {
            linedata.VerticiesPosX+= lineVertices[i].x.ToString() + ",";
            linedata.VerticiesPosY+= lineVertices[i].y.ToString() + ",";
            linedata.VerticiesPosZ+= lineVertices[i].z.ToString() + ",";
            
        }

        string jsonData = JsonUtility.ToJson(linedata);
        //DB의 최상단 디렉토리를 참조한다.  //DefaultInstace는 싱글톤 같은거 ㅇㅇ//RootReference는 최상단 노드, 루트 노드
        DatabaseReference dataRef = FirebaseDatabase.DefaultInstance.RootReference;

        //없는 디렉토리는 만들어서 넣고, 있는 디렉토리는 덮어 쓴다.
        //dataRef.Child("LineData").Child($"LineData{dataIdx}").SetRawJsonValueAsync(jsonData);
        dataRef.Child(dateName).Child($"LineData{dataIdx}").SetRawJsonValueAsync(jsonData);
       
        
        

        dataIdx++;
    }

    public float hideShowTime=0.5f;
    private int dataIdx=0;

    IEnumerator Hide(Image image)
    {
        float currentTime = 0;
        while (currentTime / hideShowTime < 1)
        {
            currentTime += Time.deltaTime;
            float a = Mathf.Lerp(1, 0, currentTime / hideShowTime);
            image.color=new Color(image.color.r,image.color.g,image.color.b,a);
            yield return null;
        }
        
    } 
    IEnumerator Show(Image image)
    {
        float currentTime = 0;
        while (currentTime / hideShowTime < 1)
        {
            currentTime += Time.deltaTime;
            float a=Mathf.Lerp(0, 1, currentTime / hideShowTime);
            image.color = new Color(image.color.r, image.color.g, image.color.b, a);
            yield return null;
        }
        
    }
}

class LineRendererData
{
    public string lineRendName;
    public string lineRendColorR;
    public string lineRendColorG;
    public string lineRendColorB;
    public string objectPosX;
    public string objectPosY;
    public string objectPosZ;
    public int verticiesNum;
    public string VerticiesPosX;
    public string VerticiesPosY;
    public string VerticiesPosZ;


    List<int> emptyList = new List<int>();
    public LineRendererData(string lineName)
    {
        lineRendName = lineName;

    }

}

