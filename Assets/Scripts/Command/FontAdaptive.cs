using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontAdaptive : MonoBehaviour {
    public int fontWidth = 50; //艺术字大小
    public float maxWidth = 0; //最大宽度
    float initSize;
    private void Awake () {
        if (maxWidth == 0) {
            maxWidth = GetComponent<RectTransform> ().rect.width;
        }
        initSize = GetComponent<RectTransform> ().localScale.x;
    }
    public void setString (string str) {
        GetComponent<Text> ().text = str;
        float width = str.Length * fontWidth * initSize;
        if (width > maxWidth) {
            GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, width);
            float newSize = maxWidth / width;
            GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1) * newSize * initSize;
        } else {
            GetComponent<RectTransform> ().localScale = new Vector3 (1, 1, 1) * initSize;
        }
    }
}