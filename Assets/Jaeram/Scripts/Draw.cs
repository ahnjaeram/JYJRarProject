using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Draw : MonoBehaviour
{
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
        //클릭 할때마다 새로운 라인 렌더러를 만든다
        //GameObject go = EventSystem.current.currentSelectedGameObject;
        //if (go != null)
        //{
        //    print(go.name);
        //}

        if (Input.touchCount > 0&& !EventSystem.current.currentSelectedGameObject)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                GameObject lineDrawer = new GameObject($"LineDrawer{lineNumb}");
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

                for(int i = 0; i < buttonColors.Length; i++)
                {
                    StartCoroutine(Hide(buttonColors[i]));
                }

                
                
            }
        //움직임에 따라 라인이 그려진다

            if (Input.GetTouch(0).phase == TouchPhase.Moved|| Input.GetTouch(0).phase == TouchPhase.Stationary)
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
                if (verticeIdx != 0)
                {
                    verticeIdx = 0;
                    lineNumb++;
                }

                for (int i = 0; i < buttonColors.Length; i++)
                {
                    StartCoroutine(Show(buttonColors[i]));
                }
            }


        }
     
    }
    public float hideShowTime=0.5f;
    
    
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

