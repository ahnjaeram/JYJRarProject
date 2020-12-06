//#define UNITY_ENGINE
#define ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEditor;

public class JY_IconMaker : MonoBehaviour
{

    //public int height=128;
    //public int width = 128;
    //public string path;
    public bool create;
    public RenderTexture ren;
    public Camera bakeCam;
    public string spriteName;
    Text pathText;

    //지윤추가
    public GameObject emptyForLocation; 

    // Start is called before the first frame update
    void Start()
    {
        pathText = GameObject.Find("PathText").GetComponent<Text>();
        StartCoroutine(GetWritePermission());
        
        //지윤추가
        Instantiate(emptyForLocation);
        //GameObject emptyforLocation = new GameObject("EmptyforLocation");
        //emptyforLocation.AddComponent<PlaceAtLocation>(); 안됨

    }

    IEnumerator GetWritePermission()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            //사용자에게 위치 정보 접근을 허가 받기 위한 팝업을 띄운다.
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);

            //사용자로 부터허가가 나올 때까지 기다려
            while (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                yield return null;
            }

        }

    }


    // Update is called once per frame
    void Update()
    {
        if (create)
        {
            CreateIcon();
            create = false;
        }
    }
    public void CreateIcon()
    {
        
        if (string.IsNullOrEmpty(spriteName))
        {
            spriteName = "icon";
        }

        string path = SaveLocation();
        path += spriteName;
        pathText.text = path;

        //ren.height = height;
        //ren.width = width;
        bakeCam.targetTexture = ren;

        RenderTexture currentRT = RenderTexture.active;
        bakeCam.targetTexture.Release();
        RenderTexture.active = bakeCam.targetTexture;
        bakeCam.Render();

        Texture2D impPng = new Texture2D(bakeCam.targetTexture.width, bakeCam.targetTexture.height, TextureFormat.ARGB32, false);
        //impPng.ReadPixels(new Rect(0, 0, bakeCam.targetTexture.height, bakeCam.targetTexture.width), 0, 0);
        impPng.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        impPng.Apply();
        RenderTexture.active = currentRT;
        byte[] bytesPng = impPng.EncodeToPNG();
        System.IO.File.WriteAllBytes(path + ".png", bytesPng);

    }

    string SaveLocation()
    {
        //string basePath = "jar:file://" + Application.dataPath + "!/assets"*/;
        //string basePath = Application.streamingAssetsPath;
        //string folderPath = "/icons";
        //string saveLocation = Path.Combine(basePath, folderPath);
#if UNITY_ENGINE
         string saveLocation = Application.streamingAssetsPath + "/Icons/";
#elif ANDROID
        //string saveLocation = "jar:file://" + Application.dataPath + "!/assets/"/*+ "/Icons/"*/;
        string saveLocation = Application.persistentDataPath + "/Icons/";
#endif



        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }
        
        return saveLocation;
    }

    public void JY_CreateIcon2()
    {
        bakeCam.targetTexture = ren;

       
        bakeCam.targetTexture.Release();
        RenderTexture.active = bakeCam.targetTexture;
        bakeCam.Render();

        Texture2D impPng = new Texture2D(bakeCam.targetTexture.width, bakeCam.targetTexture.height, TextureFormat.ARGB32, false);
        //impPng.ReadPixels(new Rect(0, 0, bakeCam.targetTexture.height, bakeCam.targetTexture.width), 0, 0);
        impPng.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        impPng.Apply();

        //지윤추가: 이미지를 저장할 때 위치 정보를 csv 파일로 저장한다.
        float[] locationInfo = new float[3];
        //locationInfo= {LocationManager.latitude, LocationManager.longitude, LocationManager.altitude};
      
        

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(impPng, "GalleryTest", "Image.png", (success, path) => Debug.Log("Media save result: " + success + " " + path));
    }
}

