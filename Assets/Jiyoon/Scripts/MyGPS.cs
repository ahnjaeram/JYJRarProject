using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class MyGPS : MonoBehaviour
{
    // GPS 데이터 변수
    public float latitude = 0;
    public float longitude = 0;
    public float altitude = 0;

    // UI 텍스트 변수
    public Text gpsData;
    public Text logText;

    public float maxWait = 10.0f;
    float waitTime = 0;

    public float reSendTime = 10.0f;
    float currentTime = 0;
    MyMapAPI mapAPI;

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        mapAPI = GetComponent<MyMapAPI>();

        StartCoroutine(GPS_On());
    }

    void Update()
    {
        // 일정한 시간마다 GPS 데이터를 다시 수신해서 그 값을 화면에 출력한다.
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

    // GPS 처리 함수
    IEnumerator GPS_On()
    {
        // 만일, 위치 정보에 대한 권한이 없다면...
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // 위치 정보에 대한 권한을 사용자에게 요청한다.
            Permission.RequestUserPermission(Permission.FineLocation);
            Permission.RequestUserPermission(Permission.CoarseLocation);
        }

        // GPS 장치를 사용할 수 있는지 확인한다.
        while (!Input.location.isEnabledByUser)
        {
            logText.text = "너 GPS 안 켜냐?";
            yield return null;
        }

        // GPS 데이터를 요청한다. -> 수신 대기
        Input.location.Start();

        // 지정한 대기 시간을 넘어가도록 대답이 없거나, 연결이 실패했을 때에는 해당 문제 로그를 화면에 출력한다.
        while (Input.location.status == LocationServiceStatus.Initializing && waitTime < maxWait)
        {
            yield return new WaitForSeconds(1.0f);

            waitTime++;
            logText.text = string.Format("GPS 데이터 수신 중...... {0:d}초", (int)waitTime);
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            logText.text = "GPS 접속 실패!";
            yield break;
        }

        if (waitTime >= maxWait)
        {
            logText.text = "데이터 응답 시간이 넘어갔어요~";
            yield break;
        }

        // 수신된 GPS 데이터를 변수에 담는다.
        LocationInfo li = Input.location.lastData;
        latitude = li.latitude;
        longitude = li.longitude;
        altitude = li.altitude;
        logText.text = "GPS 수신 완료!\r\n" + System.DateTime.Now;
        mapAPI.ShowMap();

        // 각 변수에 있는 값을 화면에 출력한다.
        string GpsText = string.Format("위도: {0:f4}\r\n경도: {1:f4}\r\n고도: {2:f4}", latitude, longitude, altitude);
        gpsData.text = GpsText;
    }
}

