using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;

public class SignIn : MonoBehaviour
{
    public InputField idInputField;
    public InputField pwInputField;
    public string id;
    public string pw;
    //public GameObject signInObject;
    public GameObject successObject;
    Firebase.Auth.FirebaseAuth auth;
    // Start is called before the first frame update
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

    }

    void SignInforFireBase(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });

        if (!successObject.activeSelf)
        {
            successObject.SetActive(true);

        }
        StartCoroutine(nextScene());
    }
    float currentTime = 0;
    float setTime = 2;
    IEnumerator nextScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3);
        //AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }
    public void LogIn()
    {
        SignInforFireBase(this.id, this.pw);
    }

    public void ChangeID()
    {

        id = idInputField.text;
    }
    public void ChangePW()
    {
        pw = pwInputField.text;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
