using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

//스마트폰의 GPS 장치를 이용해서 현재의 위도/경도/고도의 값을 화면에 출력함
public class LocationManager : MonoBehaviour
{
    public static LocationManager instance;

    public Text locationText;

    public float latitude = 0;
    public float longitude = 0;
    public float altitude = 0;

    public bool receivedGPS = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GPS_On());
    }

    IEnumerator GPS_On()
    {
        
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) //위치 정보 접근에 대한 허가를 받지 못했다면
        {
            Permission.RequestUserPermission(Permission.FineLocation); //사용자에게 위치 정보 접근을 허가받기 위한 팝업을 띄움(안드로이드에 이미 있음)
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) ///사용자로부터 허가가 나올 때까지 기다림
            {
                yield return null;
            }
        }

        //위치 정보 서비스를 요청
        if (!Input.location.isEnabledByUser) //1.장치가 켜져 있는지 확인
        {
            locationText.text = "GPS 장치 꺼져있음";
            while (!Input.location.isEnabledByUser)
            {
                yield return null;
            }
        }
        Input.location.Start(); //2.위치 정보를 요청

        while (Input.location.status == LocationServiceStatus.Initializing) //위치 정보의 수신 상태가 초기화 상태일 때 대기
        {
            yield return null;
        }
        if (Input.location.status == LocationServiceStatus.Failed) //위치 정보 수신에 실패했다면 "수신 실패" 문구를 띄움
            locationText.text = "수신 실패";
        while (Input.location.status == LocationServiceStatus.Running) //위치 정보 수신 성공
        {
            
            LocationInfo receivedData = Input.location.lastData; //정보 수신
            latitude = receivedData.latitude;
            longitude = receivedData.longitude;
            altitude = receivedData.altitude;
            
            locationText.text = string.Format("위도:{0}\r\n경도:{1}\r\n고도:{2}", latitude.ToString(), longitude.ToString(), altitude.ToString()); //화면에 출력 
            receivedGPS = true;

            yield return new WaitForSeconds(2.0f); //yield return null은 너무 자주 부름
        }
    }

}
