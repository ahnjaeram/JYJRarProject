using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draw : MonoBehaviour
{

    LineRenderer lr;
    Vector3[] lineVertices;
    Vector3[] lineVerticestoAdd = new Vector3[2];
    //public float lineDist=5;
    public int verticeIdx = 0;
    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 verticePos = new Vector3(touch.position.x, touch.position.y, 0) ;
            verticePos = Camera.main.ScreenToWorldPoint(verticePos)+ Camera.main.transform.forward * 2;
            lineVertices[verticeIdx] = verticePos;
            lr.SetPosition(verticeIdx, verticePos);
            verticeIdx++;
            
            
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
        }
        else
        {
            if (verticeIdx != 0)
            {
                verticeIdx = 0;
            }
        }
    }
}

