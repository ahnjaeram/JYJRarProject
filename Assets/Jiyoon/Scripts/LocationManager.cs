using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class LocationManager : MonoBehaviour
{
    //내 스마트폰의 GPS 장치를 이용해서 현재의 위도/경도/고도의 값을 화면에 출력하고 싶다.

    public Text locationText;

    public float latitude = 0;
    public  float longitude = 0;
    public float altitude = 0;

    // Start is called before the first frame update
    void Start()
    {
        //외부와 통신이 필요할 땐 항상 지연시간을 고려해야 한다. 요청하고 기다리는 것은 사람 입장에서는 긴 시간이 걸리지 않지만 컴퓨터 입장에서는 어쨌든 한 프레임에서 해결할 수 없는 작업-> Coroutine 사용!
        //코루틴 7종: 일정시간, 한 프레임, FixedUpdate끝날 때까지, 다른 코루틴이 끝날 때까지, 유니티웹리퀘스트, 
        StartCoroutine(GPS_On());
    }

    IEnumerator GPS_On()
    {
        //만일, 위치 정보 접근에 대한 허가를 받지 못햇다면...
        if(! Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            //사용자에게 위치 정보 접근을 허가받기 위한 팝업을 띄운다(안드로이드에 이미 있음).
            //(허가를 받아야만 GPS 자료를 받을 수 있고 무엇이든 저장할 수 있다.)
            Permission.RequestUserPermission(Permission.FineLocation);

            //사용자로부터 허가가 나올 때까지 기다린다.
            while(! Permission.HasUserAuthorizedPermission(Permission.FineLocation)) //while문은 안이 true인 동안 반복
            {
                yield return null;
            } 
            //허가가 나면 코루틴을 빠져나온다.
        }

        //위치 정보 서비스를 요청한다.
        //1. 장치가 켜져 있는지 확인한다.
        if(! Input.location.isEnabledByUser)
        {
            locationText.text = "GPS 장치 꺼져있음";
            while(! Input.location.isEnabledByUser)
            {
                yield return null;
            }
        }
        //2.위치 정보를 요청한다.
        Input.location.Start();

        //만일, 위치 정보의 수신 상태가 초기화 상태일 때
        while(Input.location.status==LocationServiceStatus.Initializing)
        {
            //바뀔 때까지 기다린다.
            yield return null;
        }
        //만일, 위치 정보 수신에 실패했다면 "수신 실패" 문구를 띄운다.
        if (Input.location.status == LocationServiceStatus.Failed)
            locationText.text = "수신 실패";
        //일정 시간(20초정도) 수신이 되지 않으면 꺼버리는 기능도 있어야

        //만일, 위치 정보 수신에 성공했다면 그 정보를 변수에 받아서 화면에 출력한다.
        while (Input.location.status == LocationServiceStatus.Running)
        {
            //정보 수신하기
            LocationInfo receivedData = Input.location.lastData;
            latitude = receivedData.latitude;
            longitude = receivedData.longitude;
            altitude = receivedData.altitude;

            //화면에 출력하기 
            locationText.text = string.Format("위도:{0}\r\n경도:{1}\r\n고도:{2}",
                latitude.ToString(), longitude.ToString(), altitude.ToString()); //문장에 빈칸 넣어놓는 것. 요소가 많이 들어가면 +보다 format으로 쓰는 게 가독성이 더 좋음 //\r은 커서를 맨 앞으로 옮기기, \n은 한 줄 내리기, \t은 한 탭(스페이스4) 띄우기

            //이렇게도 가능함//locationText.text = string.Format("위도:{0:F}\r\n경도:{1:F}\r\n고도:{2}", latitude, longitude, altitude);

            // yield return null; //이렇게 하면 너무 자주 부른다. 봇 체크에 걸림
            yield return new WaitForSeconds(2.0f);
        }
    }
  
}
