using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;

public class SignUp : MonoBehaviour
{
    public InputField idInputField;
    public InputField pwInputField;
    public InputField nickNameInputField;
    public InputField introducingInputField;
    public InputField tagInputField;

    private string userIdCode;
    public string id;
    public string pw;
    public string nickName;
    public string introducing;
    public string userTag;
    bool isSuccess=false;
    public GameObject signOnObject;
    public GameObject successObject;
    Firebase.Auth.FirebaseAuth auth;
    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";


    // Start is called before the first frame update
    Firebase.Auth.FirebaseUser user;
    private void Awake()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBUrl);
    }
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        

    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetIDPW()
    {
        SignUpforFireBase(this.id, this.pw,this.nickName,this.introducing);
    }

    public void ChangeID()
    {

        id = idInputField.text;
    }
    public void ChangePW()
    {
        pw = pwInputField.text;
    }
    public void ChangeNickName()
    {
        nickName = nickNameInputField.text;
    }
    public void ChangeIntroducing()
    {
         introducing= introducingInputField.text;
    }
    public void ChangeTag()
    {
        userTag = tagInputField.text;
    }
    void SignUpforFireBase(string email, string password, string nName, string introduce)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
            userIdCode = newUser.UserId;

            

            /////


        });

            if (!successObject.activeSelf)
            {
                successObject.SetActive(true);
            }

        SetUserData();
    }

    private void SetUserData()
    {
        UserInfo newUserData = new UserInfo(nickName, introducing, userTag);
        string jsonData = JsonUtility.ToJson(newUserData);
        DatabaseReference dataRef = FirebaseDatabase.DefaultInstance.RootReference;

        string userIdBeforeAt = "";
        foreach(char a in id)
        {
            if (a.Equals('@'))
            {
                break;
            }
            userIdBeforeAt += a;
        }

        dataRef.Child($"{userIdBeforeAt}").SetRawJsonValueAsync(jsonData);
    }

    public void SignUpPageOn()
    {
        if (!signOnObject.activeSelf)
        {
            signOnObject.SetActive(true);
        }
    }

    public void ExitSignIn()
    {
        if (signOnObject.activeSelf)
        {
            signOnObject.SetActive(false);
        }
    }

    public void ExitSignInandSuccess()
    {
        if (signOnObject.activeSelf)
        {
            signOnObject.SetActive(false);
        }
        if (successObject.activeSelf)
        {
            successObject.SetActive(false);
        }
    }
}

public class UserInfo
{
    public string nickName;
    public string intro;
    public string tags;

    public UserInfo(string name, string intr, string tags)
    {
        nickName = name;
        intro = intr;
        this.tags = tags;
    }
}
