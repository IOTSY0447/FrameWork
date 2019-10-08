using System.Collections.Generic;
using UnityEngine;
public class ObjectPool {
    private List<GameObject> objectList = new List<GameObject> ();
    //如果没有会返回空，空的话需要调用的地方自己实例化
    public GameObject GetObject () {
        GameObject obj = null;
        if (objectList.Count > 0) {
            obj = objectList[objectList.Count - 1];
            objectList.RemoveAt (objectList.Count - 1);
            obj.SetActive (true);
        }
        return obj;
    }
    public void PushObject (GameObject obj) {
        obj.SetActive (false);
        objectList.Add (obj);
    }
}