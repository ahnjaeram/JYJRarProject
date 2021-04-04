using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using System;

public class ViewSceneAll : MonoBehaviour
{
    //스마트폰의 GPS 장치를 이용해서 현재의 위도/경도/고도의 값을 화면에 출력
    public Text locationText;
    public float latitude = 0;
    public float longitude = 0;
    public float altitude = 0;

    public bool receivedGPS = false;

    //그래피티 위치 읽기
    public static ViewSceneAll vsa_instance;

    public Text graffitiLocation;

    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";

    public GraffitiData readData;
    public Vector3 graffitiPos;
    public Vector3 myPos;

    public bool isGraffiDataReady = false;

    public GameObject pin;
    public GameObject pinPrefab;

    private void Awake()
    {
        if (vsa_instance == null)
        {
            vsa_instance = this;
        }
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBUrl);
        graffitiLocation.text = "ViewSceneAll Awake";
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GPS_On());
        StartCoroutine(LoadData());
    }
    // Update is called once per frame
    void Update()
    {
        if (readData != null)
        {
            string mygrdata = string.Format("그래피티의 위치: 위도: {0}, 경도: {1}, 고도: {2}", readData.latitude, readData.longitude, readData.altitude);
            graffitiLocation.text = mygrdata;
            isGraffiDataReady = true;
        }
        else
        {
            print("readData가 null임");
        }
    }
    IEnumerator GPS_On()
    {
        //만일, 위치 정보 접근에 대한 허가를 받지 못했다면...
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            //사용자에게 위치 정보 접근을 허가받기 위한 팝업을 띄운다(안드로이드에 이미 있음).
            //(허가를 받아야만 GPS 자료를 받을 수 있고 무엇이든 저장할 수 있다.)
            Permission.RequestUserPermission(Permission.FineLocation);

            //사용자로부터 허가가 나올 때까지 기다린다.
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) //while문은 안이 true인 동안 반복
            {
                yield return null;
            }
            //허가가 나면 코루틴을 빠져나온다.
        }

        //위치 정보 서비스를 요청한다.
        //1. 장치가 켜져 있는지 확인한다.
        if (!Input.location.isEnabledByUser)
        {
            locationText.text = "GPS 장치 꺼져있음";
            while (!Input.location.isEnabledByUser)
            {
                yield return null;
            }
        }
        //2.위치 정보를 요청한다.
        Input.location.Start();
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            locationText.text = "수신 실패";
        }
        while (Input.location.status == LocationServiceStatus.Running)
        {
            LocationInfo receivedData = Input.location.lastData;
            latitude = receivedData.latitude;
            longitude = receivedData.longitude;
            altitude = receivedData.altitude;

            myPos = new Vector3(latitude, longitude, altitude);

            locationText.text = string.Format("나의 위치: 위도:{0}\r\n경도:{1}\r\n고도:{2}", latitude.ToString(), longitude.ToString(), altitude.ToString()); 
            receivedGPS = true;

            yield return new WaitForSeconds(2.0f);
        }
    }

    IEnumerator LoadData()
    {
        while (!receivedGPS) //GPS 못 받았으면 대기
        {
            yield return null;
        }

        while (true) //그 외의 경우에는 항상 실행
        {
            //DB를 읽기 위한 기준 디렉토리를 설정
            DatabaseReference dataRef = FirebaseDatabase.DefaultInstance.GetReference("Data0");
            dataRef.GetValueAsync().ContinueWith(task => //DB에서 요청을 받으면 task라는 변수에 데이터를 넘겨줌
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
                    print("dataRef 정상실행, 주소:" + dataRef);
                //DB로부터 결과 데이터를 모두 받음
                DataSnapshot snapshot = task.Result; 
                string myData = snapshot.GetRawJsonValue();
                //Json을GraffitiData 클래스 형태로 변환
                readData = new GraffitiData(JsonUtility.FromJson<GraffitiData>(myData).latitude, JsonUtility.FromJson<GraffitiData>(myData).longitude, JsonUtility.FromJson<GraffitiData>(myData).altitude);
                //DB에 있는 위도, 경도, 고도 값을 읽음
                graffitiPos = new Vector3(readData.latitude, readData.longitude, readData.altitude);
                    Debug.Log("그래피티의 위치: 위도:" + readData.latitude + "경도:" + readData.longitude + "고도:" + readData.altitude);
                }
            });
            yield return new WaitForSeconds(2.0f);
        }
    }
}
