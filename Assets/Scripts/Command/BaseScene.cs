using System.Collections;
using System.Collections.Generic;
using komal.puremvc;
using UnityEngine;
//放在canvas因为这是最先调用的
public class BaseScene : SceneBase {
    AdProxy AdProxy;
    void Start(){
        komal.sdk.SDKManager.Instance.Login();
    }
    private void OnEnable () {
        AdProxy = this.facade.RetrieveProxy (ProxyNameEnum.AdProxy) as AdProxy;
    }
    public override void Update () {
        base.Update();
        AdProxy.Update (Time.deltaTime);
    }
}