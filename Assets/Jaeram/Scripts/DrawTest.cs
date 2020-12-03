#define UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using System.IO;
using System;
public class DrawTest : MonoBehaviour
{

    LineRenderer lr;
    Vector3[] lineVertices = new Vector3[1000000];
    // Vector3[] lineVerticestoAdd = new Vector3[2];
    Vector3 verticePos;
    //public float lineDist=5;
    public int verticeIdx = 0;
    int lineNumb = 0;
    GameObject graffiti;
    bool forTheFirstTime=true;
    



    //저장용 데이터
    string timeName;
    GameObject lineDrawer;
    // Start is called before the first frame update
    void Start()
    {
        //lr =GameObject.Find("Line").GetComponent<LineRenderer>();
        //lr.enabled = false;
        graffiti = GameObject.Find("Graffiti");
    }

    // Update is called once per frame
    void Update()
    {

        DrawLine();

           




    }

    private void DrawLine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lineDrawer = new GameObject($"LineDrawer{lineNumb}");
            lineDrawer.transform.SetParent(graffiti.transform);
            lineDrawer.AddComponent<LineRenderer>();
            //라인 렌더러를 추가하면 기본 매터리얼이 없으니 추가한다.
            Material whiteDiffuseMat = new Material(Shader.Find("Sprites/Default"));
            lr = lineDrawer.GetComponent<LineRenderer>();
            lr.material = whiteDiffuseMat;
            lr.material.color = Color.white;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.numCornerVertices = 5;
            lr.numCapVertices = 5;

            timeName = DateTime.Now.ToString("yyyyMMddTHHmmss");

        }
        //움직임에 따라 라인이 그려진다

        else if (Input.GetMouseButton(0))
        {

            verticePos = (Vector3)Input.mousePosition;
            Vector3 dir = (Camera.main.ScreenToWorldPoint(new Vector3(verticePos.x, verticePos.y, Camera.main.nearClipPlane)) - Camera.main.transform.position).normalized;
            verticePos = Camera.main.ScreenToWorldPoint(new Vector3(verticePos.x, verticePos.y, Camera.main.nearClipPlane)) + dir* 10;
            lineVertices[verticeIdx] = verticePos;
            lr.positionCount = verticeIdx + 1;
            lr.SetPosition(verticeIdx, verticePos);
            verticeIdx++;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            print(lineNumb);
            Save();
            if (verticeIdx != 0)
            {
                verticeIdx = 0;
                lineNumb++;
            }
        }
    }

    private List<string[]> rowData = new List<string[]>();
    void Save()
    {
        //여기에 부울 하나 붙이자
        //처음 쓸때에는 이름,색깔 그 리스트 만들게 ㅇㅇ
        if (forTheFirstTime)
        {

        // Creating First row of titles manually..
        string[] rowDataTemp = new string[5];
        rowDataTemp[0] = "Name";
        rowDataTemp[1] = "Color";
        rowDataTemp[2] = "ObjectPos";

        rowDataTemp[3] = "VerticiesNum";
        rowDataTemp[4] = "VerticiesPosition";
        rowData.Add(rowDataTemp);
        }

        // You can add up the values in as many cells as you want.

        //일단 인트 아이 바꾸기
        for (int i = lineNumb; i < lineNumb+1; i++)
        {
            string[] rowDataTemp = new string[5];
            rowDataTemp[0] = "Line" + i; // 이름
            rowDataTemp[1] = lr.material.color.r.ToString() + lr.material.color.g.ToString() + lr.material.color.b.ToString(); // 색깔
            rowDataTemp[2] = lineDrawer.transform.position.x.ToString() + lineDrawer.transform.position.y.ToString() + lineDrawer.transform.position.z.ToString();//라인 드로워 위치
            rowDataTemp[3] = (verticeIdx + 1).ToString();//정점 갯수
            for(int j = 0; j < verticeIdx + 1; j++)
            {
                rowDataTemp[4] += lineVertices[verticeIdx].x.ToString() + lineVertices[verticeIdx].y.ToString()+ lineVertices[verticeIdx].z.ToString();
            }
            rowData.Add(rowDataTemp);
        }

        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));


        string filePath = getPath();

        StreamWriter outStream = System.IO.File.CreateText(filePath);
        outStream.WriteLine(sb);
        outStream.Close();

        if (forTheFirstTime)
        {
            forTheFirstTime = false;
        }
    }

    // Following method is used to retrive the relative path as device platform
    private string getPath()
    {
#if UNITY_EDITOR
        return Application.dataPath + "/CSV/" + "Saved_data.csv";
#elif UNITY_ANDROID
        return Application.persistentDataPath+"Saved_data.csv";
#elif UNITY_IPHONE
        return Application.persistentDataPath+"/"+"Saved_data.csv";
#else
        return Application.dataPath +"/"+"Saved_data.csv";
#endif
    }

}

