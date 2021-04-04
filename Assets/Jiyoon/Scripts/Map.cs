using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Database;
using System;
using System.Threading.Tasks;

public class Map: MonoBehaviour
{
    public GameObject mapObject;

    RawImage mapImage;
    RectTransform rt;
    UserGPS mGPS;

    public float zoom = 12;
    public string googleAPI;

    public float readLati;
    public float readLongi;
    public Vector2 myPos;

    public Text myMapText;

    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";

    void Start()
    {
        StartCoroutine(PrintlpGPS());

        mGPS = GetComponent<UserGPS>();
        mapImage = mapObject.GetComponent<RawImage>();
        rt = mapObject.GetComponent<RectTransform>();
        GetLocfromDB();
    }

    private IEnumerator PrintlpGPS()
    {
        while(LoadPin.lp_instance.lineAltitude ==null)
        {
            yield return null;
        }
        print(LoadPin.lp_instance.lineLaFl + "," + LoadPin.lp_instance.lineLoFl);
        StartCoroutine(MapOn());
    }

    IEnumerator MapOn()
    {
        while(LoadPin.lp_instance.isLineReadyToMap==false) //DB에서 데이터가 불려올 때까지 대기
        {
            yield return null;
        }

        string url = "https://maps.googleapis.com/maps/api/staticmap?"
            + "center=" + mGPS.latitude.ToString() + "," + mGPS.longitude.ToString() + "&zoom=" + zoom + "&size=" + rt.rect.width + "x" + rt.rect.height
            + "&scale=2" + "&markers=color:purple%7Clabel:C%7C" + mGPS.latitude.ToString() + "," + mGPS.longitude.ToString()
            + bluePin(LoadPin.lp_instance.loadedLineLocas); //구글 맵과 핀을 그리기 위한 URL
       
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);  // URL 주소로부터 이미지 데이터를 요청 후 대기
        yield return www.SendWebRequest();
        myMapText.text = url;
        
        if (www.isNetworkError || www.isHttpError) //수신 에러 확인
        {
            mGPS.logText.text = "지도 데이터 수신 실패\r\n" + www.error;
        }
        mapImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture; // 받은 이미지를 UI에 출력
    }

    string bluePin(List<Vector3> list)
    {
        string temp = null;
        string locaToUrl = null;
        for (int i = 0; i < LoadPin.lp_instance.loadedLineLocas.Count; i++)
        {
            temp = "&markers=color:blue%7Clabel:S%7C" + LoadPin.lp_instance.loadedLineLocas[i].x.ToString() + "," + LoadPin.lp_instance.loadedLineLocas[i].y.ToString();
            if (locaToUrl == null)
                locaToUrl = temp;
            else
                locaToUrl = locaToUrl + temp;
        }
        return locaToUrl + "&key=" + googleAPI;
    }

    void GetLocfromDB()
    {
        myPos = new Vector2(mGPS.latitude, mGPS.longitude); //나의 현재 위치를 기준으로 DB에서 데이터를 검색하도록
    }
}

