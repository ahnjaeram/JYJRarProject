using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class SetNickName : MonoBehaviour
{
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    DatabaseReference reference;
    string id;
    string NickName=null;
    string intro = null;
    string tagContents = null;
    public Text nickNameText;
    public Text introText;
    public Text tagText;
    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";
    public static SetNickName instance; 

    // Start is called before the first frame update
    private void Awake()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBUrl);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        id = GetUserID(user.Email);
        if (instance == null)
        {
            instance = this;
        }

        
        

        }

    private void UserDataTest()
    {
        if (user != null)
        {
            foreach (var profile in user.ProviderData)
            {
                // Id of the provider (ex: google.com)
                string providerId = profile.ProviderId;

                // UID specific to the provider
                string uid = profile.UserId;

                // Name, email address, and profile photo Url
                string name = profile.DisplayName;
                string email = profile.Email;
                System.Uri photoUrl = profile.PhotoUrl;

                print("providerID:" + providerId);
                print("UserID:" + uid);
                print("DisplayName:" + name);
                print("email:" + email);
                print("photoURL:" + photoUrl);
            }
        }
    }

    void Start()
    {
        FirebaseDatabase.DefaultInstance
      .GetReference(id)
      .GetValueAsync().ContinueWith(task => {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              NickName = snapshot.Child("nickName").Value.ToString();
              intro= snapshot.Child("intro").Value.ToString();
              tagContents = snapshot.Child("tags").Value.ToString();

              nickNameText.text = NickName;
              introText.text = intro;
              tagText.text = tagContents;
              

              // Do something with snapshot...
          }

      });

        StartCoroutine("FuncSetNickName");
        UserDataTest();
    }
    float currentTime = 0;
    public float setTime=1;
    IEnumerator FuncSetNickName()
    {
        while (currentTime / setTime < 1)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }
        nickNameText.text = NickName;
        introText.text = intro;
        tagText.text = tagContents;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetUserID(string id)
    {
        string userIdBeforeAt="";
        foreach (char a in id)
        {
            if (a.Equals('@'))
            {
                break;
            }
            userIdBeforeAt += a;
        }
        return userIdBeforeAt;
    }
}
