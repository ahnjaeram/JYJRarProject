using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Database;
using System;
using System.Threading.Tasks;

public class LoadPin : MonoBehaviour
{
    public Text graffitiLocation;
    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";

    public static LoadPin lp_instance;

    public GraffitiData readData;

    public Vector2 locationPos;

    public bool isGPSReady = false;
    UserGPS mGps;


    private void Awake()
    {
        if (lp_instance == null)
        {
            lp_instance = this;
        }
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBUrl);
        
        graffitiLocation.text = "LoadPin Awake";
    }

    void Start()
    {
        mGps = GetComponent<UserGPS>();
        LoadData();
    }

   
    public float lineLaFl;
    public float lineLoFl;
    public float lineAlFl;
    public DataSnapshot lineAltitude;
    public bool isLineReadyToMap = false;
    public List<Vector3> loadedLineLocas = new List<Vector3>();

    public void LoadData()
    {
        //DB를 읽기 위한 기준 디렉토리를 설정
        DatabaseReference lineDataRef = FirebaseDatabase.DefaultInstance.RootReference;
        lineDataRef.GetValueAsync().ContinueWith(task => //DB에서 요청을 받으면 task라는 변수에 데이터를 넘김
        {
            if (task.IsFaulted)
            {
                Debug.LogError("DB에서 데이터를 가져오는 것에 실패");
                graffitiLocation.text = "DB에서 데이터를 가져오는 것에 실패";
            }
            else if (task.IsCanceled)
            {
                Debug.Log("DB에서 데이터를 가져오는 것을 취소");
                graffitiLocation.text = "DB에서 데이터를 가져오는 것을 취소";
            }
            else if (task.IsCompleted)
            {
                print("lineDataRef 정상실행, 주소:" + lineDataRef);
                //DB로부터 결과 데이터를 모두 받음
                DataSnapshot snapshot = task.Result; //DataSnapshot: 하위 노드들을 가지고 있음
                                                     //RootReference 하위에 있는 모든 데이터를 순회
                foreach (DataSnapshot data in snapshot.Children)
                {
                    if (data.HasChild("GPSData") == true)
                    {

                        #region 데이터 이름
                        string dataKey = data.Key;
                        print("데이터 이름:" + dataKey);
                        #endregion

                        #region GPS 데이터
                        DataSnapshot lineGPS = data.Child("GPSData");
                        string lineGPSName = lineGPS.Key;
                        print(lineGPSName);

                        DataSnapshot lineLatitude = lineGPS.Child("latitude");
                        string lineLa = lineLatitude.GetRawJsonValue().Replace('\\', ' ').Replace('"', ' ');
                        lineLaFl = float.Parse(lineLa);

                        DataSnapshot lineLongitude = lineGPS.Child("longitude");
                        string lineLo = lineLongitude.GetRawJsonValue().Replace('\\', ' ').Replace('"', ' ');
                        lineLoFl = float.Parse(lineLo);

                        lineAltitude = lineGPS.Child("altitude");
                        string lineAl = lineAltitude.GetRawJsonValue().Replace('\\', ' ').Replace('"', ' ');
                        lineAlFl = float.Parse(lineAl);

                        print("GPS 값 in 플롯:" + lineLaFl + ",경도:" + lineLoFl + ",고도:" + lineAlFl);
                        #endregion

                        Vector3 loadedLineLoca = new Vector3(lineLaFl, lineLoFl, lineAlFl);
                        if (Vector3.Distance(loadedLineLoca, mGps.myLoca)< 2000)
                        {
                        loadedLineLocas.Add(loadedLineLoca);
                        }

                        isLineReadyToMap = true;
                    }
                    else
                    {

                    }
                }
            }
            else
            {
                print("로그인 정보");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if(readData != null)
        {
            string mydata = string.Format("그래피티의 위치: 위도: {0}, 경도: {1}", readData.latitude, readData.longitude);
            graffitiLocation.text = mydata;
        }
    }
}
