using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MyMapAPI : MonoBehaviour
{
    public GameObject mapObject;
    public string API_id;
    public string API_pass;
    public Slider levelSlider;

    RawImage mapImage;
    RectTransform rt;
    MyGPS mGPS;

    void Start()
    {
        mGPS = GetComponent<MyGPS>();
        mapImage = mapObject.GetComponent<RawImage>();
        rt = mapObject.GetComponent<RectTransform>();
        levelSlider.onValueChanged.AddListener((num) => { ShowMap(); });
    }

    public void ShowMap()
    {
        StopAllCoroutines();
        StartCoroutine(MapOn());
    }

    IEnumerator MapOn()
    {
        // 원하는 지도 데이터를 요청하기 위한 URL을 생성한다.
        string url = "https://naveropenapi.apigw.ntruss.com/map-static/v2/raster?"
                    + "w=" + rt.rect.width + "&h=" + rt.rect.height
                    + "&center=" + mGPS.longitude + "," + mGPS.latitude
                    + "&markers=size:mid|pos:" + mGPS.longitude + "%20" + mGPS.latitude + "|viewSizeRatio:2.0"
                    //+ "&center=" + 126.8823f + "," + 37.4808f
                    //+ "&markers=size:tiny|pos:" + 126.8823f + "%20" + 37.4808f
                    + "&level=" + (int)levelSlider.value
                    + "&X-NCP-APIGW-API-KEY-ID=" + API_id
                    + "&X-NCP-APIGW-API-KEY=" + API_pass;

        // URL 주소로부터 이미지 데이터를 요청(대기)한다.
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        // 수신 실패, 수신 에러 등을 확인하고 로그를 남긴다.
        if (www.isNetworkError || www.isHttpError)
        {
            mGPS.logText.text = "지도 데이터 수신 실패\r\n" + www.error;
        }

        // 받은 이미지를 UI에 출력한다.
        mapObject.SetActive(true);
        mapImage.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
    }

}

