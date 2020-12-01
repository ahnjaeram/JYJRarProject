using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTest : MonoBehaviour
{

    LineRenderer lr;
    Vector3[] lineVertices = new Vector3[10000000];
    // Vector3[] lineVerticestoAdd = new Vector3[2];
    Vector3 verticePos;
    //public float lineDist=5;
    public int verticeIdx = 0;
    int lineNumb = 0;
    GameObject graffiti;
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
        //클릭 할때마다 새로운 라인 렌더러를 만든다
        //if (Input.GetMouseButton(0))
        //{
            
            if (Input.GetMouseButtonDown(0))
            {
                GameObject lineDrawer = new GameObject($"LineDrawer{lineNumb}");
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

            }
            //움직임에 따라 라인이 그려진다

            else if (Input.GetMouseButton(0))
            {

            verticePos = (Vector3)Input.mousePosition;
            verticePos = Camera.main.ScreenToWorldPoint(new Vector3(verticePos.x, verticePos.y, Camera.main.nearClipPlane)) + Camera.main.transform.forward * 2;
                lineVertices[verticeIdx] = verticePos;
                lr.positionCount = verticeIdx + 1;
                lr.SetPosition(verticeIdx, verticePos);
                verticeIdx++;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (verticeIdx != 0)
                {
                    verticeIdx = 0;
                    lineNumb++;
                }
            }




            //if (Input.GetTouch(0).phase == TouchPhase.Began)
            //{
            //    verticeIdx = 0;
            //    lineVertices[verticeIdx] = verticePos;
            //}
            ////플레이어가 화면을 터치하면
            //if (Input.GetTouch(0).phase == TouchPhase.Moved)
            //{
            //    lineVerticestoAdd[0] = lineVertices[verticeIdx];
            //    verticeIdx += 1;
            //    lineVertices[verticeIdx] = verticePos;
            //    lineVerticestoAdd[1] = lineVertices[verticeIdx];
            //    lr.SetPositions(lineVerticestoAdd);

            //}
            //else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            //{
            //    print("선긋기 끝");
            //}
            //그 부분부터 손을 대고 있는 동안 선을 그리기 시작한다
            //손을 때면 끝난다.
        //}
        //else
        //{
        //    if (verticeIdx != 0)
        //    {
        //        verticeIdx = 0;
        //    }
        //}
    }


    //LineRenderer lr;
    //Vector3[] lineVertices=new Vector3[10000000];
    //Vector3[] lineVerticestoAdd = new Vector3[2];
    //Vector3 verticePos;
    ////public float lineDist=5;
    //public int verticeIdx = 0;
    //// Start is called before the first frame update
    //void Start()
    //{
    //    lr =GameObject.Find("Line").GetComponent<LineRenderer>();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //    if (Input.GetMouseButton(0))
    //    {

    //        verticePos = (Vector3)Input.mousePosition;
    //        //verticePos.z = 5;
    //       verticePos = Camera.main.ScreenToWorldPoint(new Vector3(verticePos.x,verticePos.y,Camera.main.nearClipPlane))+Camera.main.transform.forward;
    //        print("    " + verticePos);
    //        lineVertices[verticeIdx] = verticePos;
    //        lr.positionCount = verticeIdx + 1;
    //        lr.SetPosition(verticeIdx, lineVertices[verticeIdx]);
    //        verticeIdx++;


    //        //if (Input.GetTouch(0).phase == TouchPhase.Began)
    //        //{
    //        //    verticeIdx = 0;
    //        //    lineVertices[verticeIdx] = verticePos;
    //        //}
    //        ////플레이어가 화면을 터치하면
    //        //if (Input.GetTouch(0).phase == TouchPhase.Moved)
    //        //{
    //        //    lineVerticestoAdd[0] = lineVertices[verticeIdx];
    //        //    verticeIdx += 1;
    //        //    lineVertices[verticeIdx] = verticePos;
    //        //    lineVerticestoAdd[1] = lineVertices[verticeIdx];
    //        //    lr.SetPositions(lineVerticestoAdd);

    //        //}
    //        //else if (Input.GetTouch(0).phase == TouchPhase.Ended)
    //        //{
    //        //    print("선긋기 끝");
    //        //}
    //        //그 부분부터 손을 대고 있는 동안 선을 그리기 시작한다
    //        //손을 때면 끝난다.
    //    }
    //    else
    //    {
    //        if (verticeIdx != 0)
    //        {
    //            verticeIdx = 0;
    //        }
    //    }
}

