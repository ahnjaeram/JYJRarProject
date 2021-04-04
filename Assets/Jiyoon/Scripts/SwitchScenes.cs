using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//각 버튼을 누를 때 메뉴별 씬을 로드
public class SwitchScenes : MonoBehaviour
{
    public void LoadViewScene()
    {
        SceneManager.LoadScene("JY_View");
    }
    public void LoadDrawScene()
    {
        SceneManager.LoadScene("Draw_JYCopy");
    }
    public void LoadMapScene()
    {
        SceneManager.LoadScene("JY_Map");
    }
    public void LoadProfileScene()
    {
        SceneManager.LoadScene("Profile_JYCopy");
    }
}
