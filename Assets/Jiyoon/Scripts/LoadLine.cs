using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;
using System.IO;


using Firebase;
using Firebase.Database;
using ARLocation;

//DB에서 데이터를 받아와 그래피티를 그림
public class LoadLine : MonoBehaviour
{
    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";
    EventTrigger eTrigger;
    LineRenderer lr;
    Vector3[] lineVertices = new Vector3[10000000];
    Vector3 verticePos;
    public int verticeIdx = 0;
    int lineNumb = 0;
    public Color lineColor = Color.white;
    GameObject colorSelection;
    public bool isDrawingButtonTouched = false;
    public static LoadLine _instance;
    public Text lineLog;
    public bool isLineDataReady = false;
    Color loadCol;
    public GameObject placeAtLocation; //라인의 부모오브젝트
    int verticesNum;
    GameObject lineLoader;
    public bool isLineReDrawn = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBUrl);
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadLineData());
    }

    // Update is called once per frame
    void Update()
    {
        if (isLineDataReady == true)
        {
            if (placeAtLocation == null)
            {
                print("placeAtLocation 없음");
            }
            else
            {
                placeAtLocation.GetComponent<PlaceAtLocation>().Location.Altitude = 20;
            }
            lineLog.text = "라인 그릴 수 있음";
            ReDrawLine();
        }
        else
        {
            print("라인 데이터 준비 안 됨");
            lineLog.text = "라인 데이터 준비 안 됨";
        }
    }

    //정점 정보를 float으로 전환하기 위한 리스트
    List<float> verticesPosXFloat = new List<float>();
    float testArrayValue;
    List<float> verticesPosYFloat = new List<float>();
    List<float> verticesPosZFloat = new List<float>();
    Vector3 loadPos;

    string verticesPosXVal;
    string verticesPosYVal;
    string verticesPosZVal;

    int verticesNumInt;
    string[] verticesPosXSplit;
    string[] verticesPosYSplit;
    string[] verticesPosZSplit;

    public float lineLaFl;
    public float lineLoFl;
    public float lineAlFl;

    List<Graffiti> graffitiList = new List<Graffiti>();

    public class Graffiti
    {
        public string dataKey;
        public Vector3 lineGps;
        public List<Line> lineList;
    }

    public class Line
    {
        public string lineDataKey;
        public int verticesNum;
        public Color loadCol;
        public List<Vector3> loadedVerticesPos;
    }

    public IEnumerator LoadLineData()
    {
        while (ViewSceneAll.vsa_instance.isGraffiDataReady == false) //DB에서 그림의 좌표를 받지 못했으면 대기
        {
            yield return null;
        }

        while (true) //DB에서 데이터가 수신된 경우에는 항상 실행
        {
            //DB를 읽기 위한 기준 디렉토리를 설정
            DatabaseReference lineDataRef = FirebaseDatabase.DefaultInstance.RootReference; //DB를 읽기 위한 기준 디렉토리를 설정
            lineDataRef.GetValueAsync().ContinueWith(task => //DB에서 요청을 받으면 task라는 변수에 데이터를 넘겨줌
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("DB에서 데이터를 가져오는 것에 실패");
                    lineLog.text = "DB에서 데이터를 가져오는 것에 실패";
                }
                else if (task.IsCanceled)
                {
                    Debug.Log("DB에서 데이터를 가져오는 것을 취소");
                    lineLog.text = "DB에서 데이터를 가져오는 것을 취소";
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result; //DB로부터 결과 데이터를 모두 받음
                    foreach (DataSnapshot data in snapshot.Children)
                    {
                        if (data.HasChild("GPSData") == true) //하위 노드명으로 데이터 유형 판단
                        {
                            Graffiti gc = new Graffiti();
                            gc.lineList = new List<Line>();
                            #region 데이터 이름
                            string dataKey = data.Key;
                            #endregion
                            gc.dataKey = dataKey;
                            
                            #region GPS 데이터
                            DataSnapshot lineGPS = data.Child("GPSData");
                            string lineGPSName = lineGPS.Key;

                            DataSnapshot lineLatitude = lineGPS.Child("latitude");
                            string lineLa = lineLatitude.GetRawJsonValue().Replace('\\', ' ').Replace('"', ' ');
                            lineLaFl = float.Parse(lineLa);

                            DataSnapshot lineLongitude = lineGPS.Child("longitude");
                            string lineLo = lineLongitude.GetRawJsonValue().Replace('\\', ' ').Replace('"', ' ');
                            lineLoFl = float.Parse(lineLo);

                            DataSnapshot lineAltitude = lineGPS.Child("altitude");
                            string lineAl = lineAltitude.GetRawJsonValue().Replace('\\', ' ').Replace('"', ' ');
                            lineAlFl = float.Parse(lineAl);

                            Vector3 lineGps = new Vector3(lineLaFl, lineLoFl, lineAlFl);
                            #endregion
                            gc.lineGps = lineGps;

                            //각 그래피티마다
                            for (int l = 0; l < 20; l++)
                            {
                                //각 라인마다
                                if (data.HasChild("LineData" + l.ToString()) == true)
                                {
                                    Line lc = new Line();
                                    lc.loadedVerticesPos = new List<Vector3>();

                                    #region 라인데이터 이름
                                    DataSnapshot lineDatum = data.Child("LineData" + l.ToString());
                                    string lineDataKey = lineDatum.Key;
                                    #endregion
                                    lc.lineDataKey = lineDataKey;

                                    #region 라인 정점 개수
                                    DataSnapshot verticesNumData = lineDatum.Child("verticiesNum");
                                    string verticesNumSt = verticesNumData.GetRawJsonValue();
                                    verticesNumInt = int.Parse(verticesNumSt);
                                    #endregion
                                    lc.verticesNum = verticesNumInt;

                                    #region 컬러값
                                    DataSnapshot lrColR = lineDatum.Child("lineRendColorR");
                                    string lrColRSt = lrColR.GetRawJsonValue();
                                    float lrColRFl = float.Parse(lrColRSt.Replace('\\', ' ').Replace('"', ' '));

                                    DataSnapshot lrColG = lineDatum.Child("lineRendColorG");
                                    string lrColGSt = lrColG.GetRawJsonValue();
                                    float lrColGFl = float.Parse(lrColGSt.Replace('\\', ' ').Replace('"', ' '));

                                    DataSnapshot lrColB = lineDatum.Child("lineRendColorB");
                                    string lrColBSt = lrColB.GetRawJsonValue();
                                    float lrColBFl = float.Parse(lrColBSt.Replace('\\', ' ').Replace('"', ' '));
                                    loadCol = new Color(lrColRFl, lrColGFl, lrColBFl);
                                    #endregion
                                    lc.loadCol = loadCol;

                                    #region 정점 좌표들
                                    DataSnapshot verticesPosX = lineDatum.Child("VerticiesPosX");
                                    verticesPosXVal = verticesPosX.GetRawJsonValue();
                                    verticesPosXVal = verticesPosXVal.Replace('\\', ' ');
                                    verticesPosXVal = verticesPosXVal.Replace('"', ' ');
                                    verticesPosXSplit = verticesPosXVal.Split(',');

                                    DataSnapshot verticesPosY = lineDatum.Child("VerticiesPosY");
                                    verticesPosYVal = verticesPosY.GetRawJsonValue();
                                    verticesPosYVal = verticesPosYVal.Replace('\\', ' ');
                                    verticesPosYVal = verticesPosYVal.Replace('"', ' ');
                                    verticesPosYSplit = verticesPosYVal.Split(',');

                                    DataSnapshot verticesPosZ = lineDatum.Child("VerticiesPosZ");
                                    verticesPosZVal = verticesPosZ.GetRawJsonValue();
                                    verticesPosZVal = verticesPosZVal.Replace('\\', ' ');
                                    verticesPosZVal = verticesPosZVal.Replace('"', ' ');
                                    verticesPosZSplit = verticesPosZVal.Split(',');
                                    #endregion

                                    for (int i = 0; i < lc.verticesNum; i++)
                                    {
                                        float parseEachX = float.Parse(verticesPosXSplit[i]);
                                        float parseEachY = float.Parse(verticesPosYSplit[i]);
                                        float parseEachZ = float.Parse(verticesPosZSplit[i]);
                                        loadPos = new Vector3(parseEachX, parseEachY, parseEachZ);

                                        lc.loadedVerticesPos.Add(loadPos);

                                        isLineDataReady = true;
                                    }
                                    gc.lineList.Add(lc);
                                }
                                else
                                { }
                            }
                            graffitiList.Add(gc);
                        }
                    }
                }
                else
                {
                    print("로그인 정보");
                }
            });
            yield return new WaitForSeconds(2.0f);
        }
    }

    bool isDrawEnd = false;

    //DB에서 데이터가 불러와지면, 불러온 데이터에 따라 라인을 그림
    public void ReDrawLine()
    {
        if (isLineDataReady == true && isDrawEnd == false) //데이터 로드가 완료되었고 모두 그려지지 않은 상태
        {
            foreach (Graffiti gc in graffitiList)
            {
                placeAtLocation = new GameObject("GPS Stage Object"); //그림 데이터마다 AR상에 나타내기 위한 오브젝트를 생성
                PlaceAtLocation pal= placeAtLocation.AddComponent<PlaceAtLocation>(); //GPS 좌표대로 AR상에 나타내기 위한 컴포넌트
                Location loca = pal.LocationOptions.LocationInput.Location; //좌표를 결정하는 클래스에 데이터베이스에서 받은 GPS좌표를 넣어줌
                loca.Latitude = gc.lineGps.x;
                loca.Longitude = gc.lineGps.y;
                loca.Altitude = gc.lineGps.z;
                
                lineLoader = new GameObject(gc.dataKey); //그림의 라인들을 로드하기 위한 오브젝트를 생성해
                lineLoader.transform.parent = placeAtLocation.transform; //앞서 생성한 오브젝트의 자식으로 넣어줌 
                LineRenderer lr = lineLoader.AddComponent<LineRenderer>();

                foreach (Line lc in gc.lineList) //데이터가 가지고 있는 라인 개수만큼 반복
                {
                    Material whiteDiffuseMat = new Material(Shader.Find("Sprites/Default"));
                    lr.material = whiteDiffuseMat;
                    lr.material.color = lc.loadCol;
                    lr.startWidth = 0.1f;
                    lr.endWidth = 0.1f;
                    lr.numCornerVertices = 5;
                    lr.numCapVertices = 5;
                    lr.positionCount = lc.verticesNum;
                    for (int i = 0; i < lc.verticesNum; i++) //각 라인의 정점 개수만큼 반복
                    {
                        lr.SetPosition(i, lc.loadedVerticesPos[i]); //불러온 Vector3 값들을 정점의 위치값으로 지정
                    }
                    isLineReDrawn = true;
                }
            }
            isDrawEnd = true;
        }
    }
}
