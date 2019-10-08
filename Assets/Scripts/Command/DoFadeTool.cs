using System.Collections;
using UnityEngine;

[RequireComponent (typeof (CanvasGroup))]

public class DoFadeTool : MonoBehaviour {
    private CanvasGroup canvasGroup;

    void Awake () {
        canvasGroup = this.gameObject.GetComponent<CanvasGroup> ();
    }
    public void FadeTo (float alpha, float time, float delayTime = 0, System.Action cb = null) {
        if (time == 0) {
            canvasGroup.alpha = alpha;
            if (cb != null) {
                cb ();
            }
            return;
        }
        float alphaNow = canvasGroup.alpha;
        int count = (int) Mathf.Floor (time / Time.fixedDeltaTime);
        float d = (alpha - alphaNow) / count;
        System.Action<int> callback = (int i) => {
            canvasGroup.alpha += i * d;
            if (i == count) {
                canvasGroup.alpha = alpha;
                if (cb != null) {
                    cb ();
                }
            }
        };
        StartScheduler (count, callback, Time.fixedDeltaTime, delayTime);
    }
    protected Coroutine StartScheduler (int count, System.Action<int> callback, float duration = 0.0f, float delay = 0.0f) {
        return StartCoroutine (Scheduler (count, callback, duration, delay));
    }
    private IEnumerator Scheduler (int count, System.Action<int> callback, float duration = 0.0f, float delay = 0.0f) {
        if (delay != 0.0f) {
            yield return new WaitForSeconds (delay);
        }
        if (duration == 0.0f) {
            for (int i = 0; i < count; i++) {
                callback (i + 1);
                yield return null;
            }
        } else {
            for (int i = 0; i < count; i++) {
                callback (i + 1);
                yield return new WaitForSeconds (duration);
            }
        }
    }
}