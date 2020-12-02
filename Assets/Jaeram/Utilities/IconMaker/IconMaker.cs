using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IconMaker : MonoBehaviour
{

    //public int height=128;
    //public int width = 128;

    public bool create;
    public RenderTexture ren;
    public Camera bakeCam;
    public string spriteName;
    // Start is called before the first frame update
    void Start()
    {
        
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

        //ren.height = height;
        //ren.width = width;
        bakeCam.targetTexture = ren;

        RenderTexture currentRT = RenderTexture.active;
        bakeCam.targetTexture.Release();
        RenderTexture.active = bakeCam.targetTexture;
        bakeCam.Render();

        Texture2D impPng = new Texture2D(bakeCam.targetTexture.width, bakeCam.targetTexture.height, TextureFormat.ARGB32, false);
        impPng.ReadPixels(new Rect(0, 0, bakeCam.targetTexture.height, bakeCam.targetTexture.width), 0, 0);
        impPng.Apply();
        RenderTexture.active = currentRT;
        byte[] bytesPng = impPng.EncodeToPNG();
        System.IO.File.WriteAllBytes(path + ".png", bytesPng);

    }

    string SaveLocation()
    {
        string basePath = "jar:file://" + Application.dataPath + "!/assets";
        string folderPath = "icons";
        string saveLocation = Path.Combine(basePath, folderPath);
        

        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }

        return saveLocation;
    }
}
