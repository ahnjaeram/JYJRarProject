using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Opening_logo : MonoBehaviour
{
    public float speed1 = 10;
    public float speed2 = 10;
    public float switchTime;
    float currentTime;
    Vector3 originPos;
    float floatingY;

    // Start is called before the first frame update
    void Start()
    {
        originPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime < switchTime)
        {
            floatingY = Mathf.Sin(currentTime * speed1) * speed2;
            transform.position = new Vector3(originPos.x, originPos.y + floatingY, originPos.z);
        }
        else
        {
            SceneManager.LoadScene("Login_JYCopy");
        }
    }

}
