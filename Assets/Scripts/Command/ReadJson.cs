using System.Collections;
using System.Collections.Generic;
using System.IO;
using komal;
using komal.puremvc;
using LitJson;
using UnityEngine;
public class ReadJson : ComponentEx {
    public static JsonData myJson;
    public static JsonData levelBiao;
    public override void Init () {
        StreamReader sr = new StreamReader (Application.streamingAssetsPath + "/configData.json");
        string str = sr.ReadToEnd ();
        JsonData jd = JsonMapper.ToObject (str);
        myJson = jd;
        levelBiao = myJson["configData"]["level"];
    }
    public void Start () {
        StreamReader sr = new StreamReader (Application.streamingAssetsPath + "/configData.json");
        string str = sr.ReadToEnd ();
        JsonData jd = JsonMapper.ToObject (str);
        myJson = jd;
        levelBiao = myJson["configData"]["level"];
    }
    public static JsonData getLevelData (int level, levelKey key) {
        if (levelBiao.Count > level) {
            return levelBiao[level - 1][(int) key];
        }
        return null;
    }
}