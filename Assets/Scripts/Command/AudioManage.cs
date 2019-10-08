using System.Collections;
using System.Collections.Generic;
using komal;
using komal.puremvc;
using UnityEngine;
public class EffectRes {
    static public string lajitongusic = "lajitong"; //垃圾桶
    static public string shengji = "shengji"; //升级
    static public string shuangbei = "shuangbei"; //双倍道具
    static public string anniu = "anniu"; //按钮
    static public string duankai = "duankai"; //断开
    static public string pojilu = "pojilu"; //破纪录?
    static public string hecheng = "hecheng"; //合成
    static public string k = "k"; //连出k
    static public string fuhuo = "fuhuo";//复活
}
public class switchData {
    public int SoundEnabled;
    public int TapEnabled;
    public switchData (int SoundEnabled, int TapEnabled) {
        this.SoundEnabled = SoundEnabled;
        this.TapEnabled = TapEnabled;
    }
    public switchData () {
        this.SoundEnabled = 1;
        this.TapEnabled = 1;
    }

}
public class BGMRes {
    static public string beijing = "beijing"; //背景
    static public string bgMusic = "bgm";//背景新
}
public class AudioManage : ComponentEx, INotificationHandler {

    AudioSource audiosource;
    // GameContrProxy GameContrProxy;
    public bool SoundEnabled;
    public bool TapEnabled;

    protected override void Awake () {
        base.Awake ();
        audiosource = gameObject.AddComponent<AudioSource> ();
        audiosource.loop = true;
        switchData switchData = KomalUtil.Instance.GetItem ("switch", new switchData ());
        SoundEnabled = switchData.SoundEnabled == 1;
        TapEnabled = switchData.TapEnabled == 1;
    }
    private void OnEnable () {
        // GameContrProxy = this.facade.RetrieveProxy (ProxyNameEnum.GameContrProxy) as GameContrProxy;
        // GameContrProxy.AudioManage = this;
    }
    private void Start () {
    }

    //在指定位置播放音频 PlayClipAtPoint()
    public void playEffect (string name) {
        if (!SoundEnabled) {
            return;
        }
        AudioClip clip = Resources.Load<AudioClip> ("Sounds/" + name);
        if (clip == null) {
            Debug.LogError ("未找到音频" + name);
            return;
        }
        AudioSource.PlayClipAtPoint (clip, Camera.main.transform.position);
    }

    //如果当前有其他音频正在播放，停止当前音频，播放下一个
    public void playBGM (string name) {
        // return;//暂时不要背景音
        if (audiosource.isPlaying) {
            audiosource.Stop ();
        }
        AudioClip clip = Resources.Load<AudioClip> ("Sounds/" + name);
        if (clip == null) {
            Debug.LogError ("未找到音频" + name);
            return;
        }
        audiosource.clip = clip;
        if (!SoundEnabled) {
            return;
        }
        audiosource.Play ();
    }
    //开关音效
    public void switchEffect () {
        // SoundEnabled = !SoundEnabled;
    }
    //开关BGM
    public void switchBgm () {
        SoundEnabled = !SoundEnabled;
        if (!SoundEnabled && audiosource.isPlaying) {
            audiosource.Stop ();
        } else {
            audiosource.Play ();
        }
        KomalUtil.Instance.SetItem ("switch", new switchData (SoundEnabled?1 : 0, TapEnabled?1 : 0));
    }
    //开关震动
    public void switchTap () {
        TapEnabled = !TapEnabled;
        TapMIDIUM ();
        KomalUtil.Instance.SetItem ("switch", new switchData (SoundEnabled?1 : 0, TapEnabled?1 : 0));
    }
    //震动
    public void TapMIDIUM () {
        if (TapEnabled) {
            KomalUtil.Instance.TapEngineImpact (TapEngineImpactType.MIDIUM);
        }
    }
}