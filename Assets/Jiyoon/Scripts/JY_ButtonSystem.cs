using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JY_ButtonSystem : MonoBehaviour
{
    JY_IconMaker iconMaker;
    ScreenShotMaker photoMaker;
    Draw draw;
    GameObject drawingButton;
    public Material drawMat;
    //Text pathText;
    void Start()
    {
        draw = GameObject.Find("Draw").GetComponent<Draw>();
        drawingButton = GameObject.Find("DrawingButton");
        iconMaker = GameObject.Find("Holder").GetComponent<JY_IconMaker>();
        photoMaker = GameObject.Find("Holder").GetComponent<ScreenShotMaker>();
        //pathText = GameObject.Find("PathText").GetComponent<Text>();
    }

    // Update is called once per frame
    public void SetColorWhite()
    {
        draw.lineColor = Color.white;
    }
    public void SetColorRed()
    {
        draw.lineColor = Color.red;
    }
    public void SetColorOrange()
    {
        draw.lineColor = new Color(1,0.5f,0);
    }
    public void SetColorYellow()
    {
        draw.lineColor = Color.yellow;
    }
    public void SetColorGreen()
    {
        draw.lineColor = Color.green;
    }
    public void SetColorBlue()
    {
        draw.lineColor = new Color(0, 0.4f, 1);
    }
    public void SetColorDarkBlue()
    {
        draw.lineColor = new Color(0, 0, 1);
    }
    public void SetColorPurple()
    {
        draw.lineColor = new Color(0.5f, 0, 1);
    }

    public void SetButtonBoolTrue()
    {
        draw.isDrawingButtonTouched = true;
    }
    public void SetDrawButtonColor()
    {
        drawingButton.GetComponent<Image>().color = draw.lineColor;
    }
    public void MakeIcon()
    {
        // iconMaker.CreateIcon();
        iconMaker.JY_CreateIcon2();
        //pathText.text = iconMaker.path;
    }
    public void MakePhoto()
    {
        photoMaker.CreateScreenShot();
    }

    public void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }

                //Sprite sprite = LoadNewSprite(texture);

                // Assign texture to a temporary quad and destroy it after 5 seconds
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                quad.transform.forward = Camera.main.transform.forward;
                //quad.transform.localScale = new Vector3(1f, (float)texture.height / (float)texture.width, 1f);
                quad.transform.localScale = new Vector3(3f, (float)texture.height / (float)texture.width, 1f);

                //Material newMat = new Material(Shader.Find("Standard"));
                

                //Material material = quad.GetComponent<Renderer>().material;


                //if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                //    material.shader = Shader.Find("Diffuse");

                quad.GetComponent<Renderer>().material = drawMat;

                //quad.GetComponent<Renderer>().material.mainTexture = texture;
                quad.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
                Destroy(quad, 5f);

                // If a procedural texture is not destroyed manually, 
                // it will only be freed after a scene change
                Destroy(texture, 5f);
            }
        }, "Select a PNG image", "image/png");

        Debug.Log("Permission result: " + permission);

        
    }

    private Sprite LoadNewSprite(Texture2D texture, float PixelsPerUnit = 100.0f)
    {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference

        Sprite NewSprite;// = new Sprite();
        

        NewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), PixelsPerUnit);

        return NewSprite;
    }
}
