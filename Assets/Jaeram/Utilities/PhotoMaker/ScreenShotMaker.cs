#define ANDROID
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class ScreenShotMaker : MonoBehaviour
{
    //public int height=128;
    //public int width = 128;
    //public string path;
    public bool create;
    public RenderTexture ren;
    public Camera bakeCam;
    public string spriteName;
    Text pathText;
    public GameObject canvas;
    int nameNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        pathText = GameObject.Find("ScreenShotPathText").GetComponent<Text>();
        
        //canvas = GameObject.Find("Canvas");
        
    }

    


    // Update is called once per frame
    void Update()
    {
        if (create)
        {
            CreateScreenShot();
            create = false;
        }
    }
    public void CreateScreenShot()
    {
        //if (canvas.GetComponent<Canvas>().enabled)
        //{
        //    canvas.GetComponent<Canvas>().enabled = false;
        //}

        if (string.IsNullOrEmpty(spriteName))
        {
            spriteName = "screenShot";
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

        Texture2D impJpg = new Texture2D(bakeCam.targetTexture.width, bakeCam.targetTexture.height, TextureFormat.ARGB32, false);
        //impJpg.ReadPixels(new Rect(0, 0, bakeCam.targetTexture.height, bakeCam.targetTexture.width), 0, 0);
        impJpg.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        impJpg.Apply();
        RenderTexture.active = currentRT;
        byte[] bytesJPG = impJpg.EncodeToJPG();
        System.IO.File.WriteAllBytes(path + ".jpg", bytesJPG);

        //if (!canvas.GetComponent<Canvas>().enabled)
        //{
        //    canvas.GetComponent<Canvas>().enabled = true;
        //}

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
        string saveLocation = Application.persistentDataPath + "/Screenshots/";
#endif



        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }

        return saveLocation;
    }

    public void ScreenShot()
    {
        if (canvas.activeSelf)
        {
            print("11");
            canvas.SetActive(false);
            print("22");
        }
        ScreenCapture.CaptureScreenshot($"Screenshots/ScreenShotForAR{nameNum}.png");

        StartCoroutine(WaitToCapture());
        
    }


    public void ScreenShot2()
    {
        if (canvas.activeSelf)
        {
            print("11");
            canvas.SetActive(false);
            print("22");
        }
        bakeCam.targetTexture = ren;

       // RenderTexture currentRT = RenderTexture.active;
        bakeCam.targetTexture.Release();
        RenderTexture.active = bakeCam.targetTexture;
        bakeCam.Render();
        
        Texture2D impJpg = new Texture2D(bakeCam.targetTexture.width, bakeCam.targetTexture.height, TextureFormat.ARGB32, false);
        //impJpg.ReadPixels(new Rect(0, 0, bakeCam.targetTexture.height, bakeCam.targetTexture.width), 0, 0);
        impJpg.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        impJpg.Apply();
        byte[] imageBytes = impJpg.EncodeToJPG();

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(imageBytes, "GalleryTest", "Image.jpg", (success, path) => Debug.Log("Media save result: " + success + " " + path));


        StartCoroutine(WaitToCapture());
    }
    float currentTime=0;
    float setTime = 1;
    IEnumerator WaitToCapture()
    {
        while (currentTime < setTime)
        {
        currentTime += Time.deltaTime;
        yield return null;

        }

        if (!canvas.activeSelf)
        {
            canvas.SetActive(true);
            currentTime = 0;
            bakeCam.targetTexture = null;
        }
    }
}
