using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiPei : MonoBehaviour {
    public List<GameObject> arrSiPeinode = new List<GameObject> ();
    float offsetSize;
    public float offsetRate; //适配的比例

    void Start () {
        //设置屏幕自动旋转， 并设置支持的方向
        float device_width = Screen.width; //当前设备宽度  
        float device_height = Screen.height; //当前设备高度 
        float rat = (10f - 8.5f) / (2436f / 1125f - 1334f / 750f);
        offsetSize = rat * (device_height / device_width - 1334f / 750f);
        offsetSize = Mathf.Max (0, offsetSize);
        float newSize = 8.5f + offsetSize;
        newSize = Mathf.Max (8.5f, newSize); //摄像机距离最小为8.5
        GetComponent<Camera> ().orthographicSize = newSize;
        shiPei ();
    }
    public void shiPei () {
        arrSiPeinode.ForEach (uiNode => {
            siPeiUiNode (uiNode);
        });
    }
    //适配
    void siPeiUiNode (GameObject uiNode) {
        Vector3 v3 = uiNode.GetComponent<RectTransform> ().localPosition;
        v3.y *= 1 + offsetSize / offsetRate;
        uiNode.GetComponent<RectTransform> ().localPosition = v3;
    }
}