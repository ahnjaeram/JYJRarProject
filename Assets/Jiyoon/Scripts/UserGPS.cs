using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class UserGPS : MonoBehaviour
{
    //GPS 데이터 변수
    public float latitude = 0;
    public float longitude = 0;
    public float altitude = 0;

    //UI 텍스트 변수
    public Text gpsData;
    public Text logText;

    public float maxWait = 10.0f;
    float waitTime = 0;

    public float reSendTime = 5.0f;
    float currentTime = 0;
    Map mapAPI;

    public Vector3 myLoca;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        mapAPI = GetComponent<Map>();
        StartCoroutine(GPS_On());
    }

    void Update() // 일정한 시간마다 GPS 데이터를 다시 수신해 화면에 출력
    {
        if (currentTime < reSendTime)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            currentTime = 0;
            waitTime = 0;
            StartCoroutine(GPS_On());
        }
    }

    IEnumerator GPS_On() //사용자의 GPS 좌표를 받음
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) //위치 정보에 대한 권한 요청
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }
        #region 장치 사용 가능여부 확인
        // GPS 장치를 사용할 수 있는지 확인한다.
        while (!Input.location.isEnabledByUser)
        {
            logText.text = "GPS 장치 꺼져 있음";
            yield return null;
        }
        #endregion

        Input.location.Start(); //GPS 데이터 요청 후 수신 대기

        #region 연결 실패 시 로그 및 예외처리
        // 지정한 대기 시간을 넘어가도록 대답이 없거나, 연결이 실패했을 때에는 해당 문제 로그를 화면에 출력한다.
        while (Input.location.status == LocationServiceStatus.Initializing && waitTime < maxWait)
        {
            yield return new WaitForSeconds(1.0f);

            waitTime++;
            logText.text = string.Format("GPS 데이터 수신 중...... {0:d}초", (int)waitTime);
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            logText.text = "GPS 접속 실패";
            yield break;
        }

        if (waitTime >= maxWait)
        {
            logText.text = "데이터 응답 시간 초과";
            yield break;
        }
        #endregion

        LocationInfo li = Input.location.lastData; //수신된 GPS 데이터를 변수에 담은 후, 
        latitude = li.latitude;
        longitude = li.longitude;
        altitude = li.altitude;
        
        myLoca = new Vector3(latitude, longitude, altitude); //사용자의 위치라는 Vector3값으로 만들어줌
        logText.text = "GPS 데이터 수신 완료\r\n" + System.DateTime.Now;
    }
}

