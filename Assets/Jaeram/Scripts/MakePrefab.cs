using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MakePrefab : MonoBehaviour
{
    public GameObject Graffities;
    int graffitiNum = 0;
    public bool makePrefab=false;
    
    [MenuItem("Examples/Create Prefab")]
    // Start is called before the first frame update
   
    void Update()
    {
        if (makePrefab)
        {
            CreatePrefab();
            makePrefab = false;
        }
    }
    void CreatePrefab()
    {
        string localPath = "Assets/"+ "GraffitiMade/" + $"graffiti{graffitiNum}"+ ".prefab";
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
        PrefabUtility.SaveAsPrefabAssetAndConnect(Graffities, localPath, InteractionMode.UserAction);
        graffitiNum++;
    }
}
