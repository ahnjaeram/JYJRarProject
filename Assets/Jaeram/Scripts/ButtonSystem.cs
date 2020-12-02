using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSystem : MonoBehaviour
{
    IconMaker iconMaker;
    PhotoMaker photoMaker;
    Draw draw;
    GameObject drawingButton;
    void Start()
    {
        draw = GameObject.Find("Draw").GetComponent<Draw>();
        drawingButton = GameObject.Find("DrawingButton");
        iconMaker = GameObject.Find("Holder").GetComponent<IconMaker>();
        photoMaker = GameObject.Find("Holder").GetComponent<PhotoMaker>();
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
        iconMaker.create = true;
    }
    public void MakePhoto()
    {
        photoMaker.TakeScreenShot();
    }
}
