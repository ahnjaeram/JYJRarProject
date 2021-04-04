using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class LocationManagerJR : MonoBehaviour
{
    public static LocationManagerJR instance;
    //내 스마트 폰의 GPS 장치를 이용해서 현재의 위도/경도/고도의 값을 화면에 출력하고 싶다.
    public Text locationText;

    public float latitude = 0;
    public float longitude = 0;
    public float altitude = 0;
    public bool locationAccepted = false;
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        locationText.text = "888";
        StartCoroutine(GPS_ON());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator GPS_ON()
    {
        //만일 위치 정보 접근에 대한 허가를 받지 못했다면, 
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            //사용자에게 위치 정보 접근을 허가 받기 위한 팝업을 띄운다.
            Permission.RequestUserPermission(Permission.FineLocation);

            //사용자로 부터허가가 나올 때까지 기다려
            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }

        }

        //위치 정보 서비스를 요쳥한다.
        //1. 장치가 켜져 있는지 확인한다.
        if (!Input.location.isEnabledByUser)
        {
            locationText.text = "장치가 꺼져 있습니다";

            while (!Input.location.isEnabledByUser)
            {
                yield return null;
            }
        }
        //위치 정보를 요청을 한다.
        Input.location.Start();

        //만일, 위치 정보의 수신 상태가 초기화 상태(수신을 받을때 까지)일때
        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }
        //만일 위치 정보 수신에 실패했다면 "수신에 실패했습니다"라고 띄운다.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            locationText.text = "수신에 실패했습니다";
        }
        //만일 위치 정보 수신에 성공 했다면, 그 정보를 변수에 받아서 화면에 출력한다.
        while (Input.location.status == LocationServiceStatus.Running)
        {
            //정보 수신하기
            LocationInfo receiveData = Input.location.lastData;
            latitude = receiveData.latitude;
            longitude = receiveData.longitude;
            altitude = receiveData.altitude;

            //화면에 출력하기
            //   \r은 커서를 그 줄 맨 앞으로(carrage return)  \n은 커서를 밑으로 그래서 둘 합치면 한 줄 띄우기지
            locationText.text = string.Format("위도:{0}\r\n경도:{1}\r\n고도:{2}", latitude.ToString(), longitude.ToString(), altitude.ToString());
            locationAccepted = true;
            yield return new WaitForSeconds(2f);
        }



        //Running(받았을때)
        //Fail(실패했을 때) 
    }

}
