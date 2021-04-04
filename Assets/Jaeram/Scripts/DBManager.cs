using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


using Firebase;
using Firebase.Database;
using Firebase.Storage;


using System;
using System.Threading.Tasks;


public class DBManager : MonoBehaviour
{

    public string DBUrl = "https://jyjrarproject-default-rtdb.firebaseio.com/";
    public string DBStorageURL;
    //싱글톤만들어놓자
    public static DBManager dbManager;
    public Text debugText;
    Firebase.Storage.FirebaseStorage storage;
    Firebase.Storage.StorageReference storage_ref;


    int dataIdx = 0;

    private void Awake()
    {
        if (dbManager == null)
        {
            dbManager = this;
        }
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = new Uri(DBUrl);
    }


    // Start is called before the first frame update
    void Start()
    {
        DBStorageURL = "gs://jyjrarproject.appspot.com";
        
        storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        storage_ref = storage.GetReferenceFromUrl(DBStorageURL);
        print(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveGraffitiData()
    {
        //버튼을 누르면 위도, 경도, 고도 위치를 저장한다.
        GraffitiData data = new GraffitiData(LocationManagerJR.instance.latitude, LocationManagerJR.instance.longitude, LocationManagerJR.instance.altitude);

        string jsonData = JsonUtility.ToJson(data);

        //DB의 최상단 디렉토리를 참조한다.  //DefaultInstace는 싱글톤 같은거 ㅇㅇ//RootReference는 최상단 노드, 루트 노드
        DatabaseReference dataRef = FirebaseDatabase.DefaultInstance.RootReference;

        //없는 디렉토리는 만들어서 넣고, 있는 디렉토리는 덮어 쓴다.
        dataRef.Child(Draw.instance.dateName).Child("GPSData").SetRawJsonValueAsync(jsonData);

       // dataIdx++;

    }

    public void SaveImage(byte[] imageBytes, string imageName)
    {
        
        StorageReference images_ref = storage_ref.Child($"JPGImages/{imageName}");
        object p = images_ref.PutBytesAsync(imageBytes);/*.ContinueWith((Task<StorageMetadata> task) =>
        {
            if (task is null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (task.IsFaulted || task.IsCanceled)
            {
                debugText.text = task.Exception.ToString();
                Uh - oh, an error occurred!
            }
            else
            {
                Metadata contains file metadata such as size, content - type, and download URL.
                 Firebase.Storage.StorageMetadata metadata = task.Result;
                debugText.text = metadata.Path;


            }
        });*/
    }

}



public class GraffitiData
{
    public float latitude;
    public float longitude;
    public float altitude;
    

    public GraffitiData(float lat, float lon,float alt)
    {
        latitude = lat;
        longitude = lon;
        altitude = alt;
        
    }

}
