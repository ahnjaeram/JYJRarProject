using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
public class ProfilePic : MonoBehaviour
{
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;
    DatabaseReference reference;
    public Sprite nullPic;
    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";
    System.Uri photoUrl;
    private void Awake()
    {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBUrl);
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
    }
    // Start is called before the first frame update
    void Start()
    {
        CheckProfilePic();
    }

    private void CheckProfilePic()
    {
        if (user != null)
        {
            foreach (var profile in user.ProviderData)
            {
                
                photoUrl = profile.PhotoUrl;

            }

            if (photoUrl == null)
            {
                this.GetComponent<Image>().sprite = nullPic;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SetProfileImage()
    {
        while (true)
        {
            if (user != null)
            {
                foreach (var profile in user.ProviderData)
                {

                    photoUrl = profile.PhotoUrl;

                }

                if (photoUrl == null)
                {
                    this.GetComponent<Image>().sprite = nullPic;
                }
                else
                {

                }
            }
            yield return new WaitForSeconds(2f);
        }
    }
    public void SetPic(int maxSize)
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
                //Rect rect = new Rect(Vector2.zero, new Vector2(1000, 1000));
                //Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);
                texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                texture.Apply();
                byte[] bytesPng = texture.EncodeToPNG();

                DBManager.dbManager.SaveImage(bytesPng, SetNickName.instance.GetUserID(user.Email));
                



            }
        }, "Select a PNG image", "image/png");



    }

    
}
