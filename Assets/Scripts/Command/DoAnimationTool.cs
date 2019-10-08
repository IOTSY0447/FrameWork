using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DoAnimationTool {
    static public void btnEffect (GameObject obj, System.Action cb) {
        Sequence mySequence = DOTween.Sequence (); //创建空序列 
        Tweener anction0 = obj.GetComponent<RectTransform>().DOScale(new Vector3(1.3f,1.3f,0),0.2f);
        Tweener anction1 = obj.GetComponent<RectTransform>().DOScale(new Vector3(0.9f,0.9f,0),0.15f);
        Tweener anction2 = obj.GetComponent<RectTransform>().DOScale(new Vector3(1.2f,1.2f,0),0.1f);
        // Tweener anction20 = obj.transform.do//透明度
        Tweener anction3 = obj.GetComponent<RectTransform>().DOScale(new Vector3(1f,1f,0),2f);
        mySequence.Append(anction0);
        mySequence.Append(anction1);
        mySequence.Append(anction2);
        mySequence.Append(anction3);
        mySequence.AppendCallback (() => {
            // cb ();
        });
    }
}