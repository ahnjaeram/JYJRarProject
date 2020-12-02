using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PhotoMaker : MonoBehaviour
{

    public bool isProcessing=false;
    // Start is called before the first frame update
    //Set your screenshot resolutions 
    public int captureWidth = 1080; public int captureHeight = 1920; 
    // configure with raw, jpg, png, or ppm (simple raw format) 
    public enum Format { RAW, JPG, PNG, PPM }; 
    public Format format = Format.JPG; 
    // folder to write output (defaults to data path)
    private string ScreenshotFolder;
    private Rect rect;
    private RenderTexture renderTexture;
    private Texture2D screenShot;

    private string CreateFileName(int width, int height)
    { //날짜로 이름 짓기
        string timeName = DateTime.Now.ToString("yyyyMMddTHHmmss"); 
        // use width, height, and timestamp for unique file 
        var filename = string.Format("{0}/screen_{1}x{2}_{3}.{4}", ScreenshotFolder, width, height, timeName, format.ToString().ToLower()); 
        // return filename 
        return filename; }
    private void CaptureScreenshot()
    {
        isProcessing = true; 
        // create screenshot objects 
        if (renderTexture == null) { 
            // creates off-screen render texture to be rendered into
            rect = new Rect(0, 0, captureWidth, captureHeight); 
            renderTexture = new RenderTexture(captureWidth, captureHeight, 24);
            screenShot = new Texture2D(captureWidth, captureHeight, TextureFormat.RGB24, false); }
        // get main camera and render its output into the off-screen render texture created above 
        Camera camera = Camera.main;
        camera.targetTexture = renderTexture; 
        camera.Render(); 
        // mark the render texture as active and read the current pixel data into the 
        RenderTexture.active = renderTexture; screenShot.ReadPixels(rect, 0, 0); 
        // reset the textures and remove the render texture from the Camera since were done reading the screen data 
        camera.targetTexture = null; RenderTexture.active = null; 
        // get our filename 
        string filename = CreateFileName((int)rect.width, (int)rect.height); 
        // get file header/data bytes for the specified image format 
        byte[] fileHeader = null; 
        byte[] fileData = null; 
        //Set the format and encode based on it 
        if (format == Format.RAW) 
        { fileData = screenShot.GetRawTextureData(); }
        else if (format == Format.PNG)
        { fileData = screenShot.EncodeToPNG(); } 
        else if (format == Format.JPG) { fileData = screenShot.EncodeToJPG(); }
        else //For ppm files 
        { // create a file header - ppm files 
            string headerStr = string.Format("P6\n{0} {1}\n255\n", rect.width, rect.height); fileHeader = 
                System.Text.Encoding.ASCII.GetBytes(headerStr); fileData = screenShot.GetRawTextureData(); } 
        // create new thread to offload the saving from the main thread 
        new System.Threading.Thread(() => { var file = System.IO.File.Create(filename); 
            if (fileHeader != null) { file.Write(fileHeader, 0, fileHeader.Length); } 
            file.Write(fileData, 0, fileData.Length); 
            file.Close(); 
            Debug.Log(string.Format("Screenshot Saved {0}, size {1}", filename, fileData.Length)); 
            isProcessing = false; }).Start(); 
        //Cleanup 
        Destroy(renderTexture);
        renderTexture = null; screenShot = null; }
    // private variables needed for screenshot 


    public void TakeScreenShot() 
    { if (!isProcessing) { CaptureScreenshot(); } 
        else { Debug.Log("Currently Processing"); } }
    
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
