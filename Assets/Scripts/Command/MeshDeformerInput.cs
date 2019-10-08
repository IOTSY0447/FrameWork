// using komal.puremvc;
// using UnityEngine;
// using UnityEngine.EventSystems;

// public class MeshDeformerInput : ComponentEx, INotificationHandler {

// 	public float force = 10f;
// 	public float forceOffset = 0.1f;
// 	GameContrProxy GameContrProxy;
// 	private void Start () {
// 		GameContrProxy = this.facade.RetrieveProxy (ProxyNameEnum.GameContrProxy) as GameContrProxy;
// 		Input.multiTouchEnabled = false; //关闭多点触控
// 	}
// 	void Update () {
// 		GameContrProxy.tipLinkTimeVal += Time.deltaTime;
// 		if (GameContrProxy.tipLinkTimeVal > GameContrProxy.tipLinkDisTime) {
// 			GameContrProxy.tipCanLinkByTime ();
// 			GameContrProxy.tipLinkTimeVal = 0;
// 		}
// 		GameContrProxy.adTimeVal += Time.deltaTime;
// 		if (GameContrProxy.interval > 0) {
// 			GameContrProxy.interval -= Time.deltaTime;
// 		}
// 		// if (EventSystem.current.IsPointerOverGameObject ()) { //如果是ui
// 		// 	return;
// 		// }
// 		if (Input.GetMouseButton (0) && GameContrProxy.interval <= 0) {
// 			HandleInput ();
// 		}
// 	}

// 	void HandleInput () {
// 		GameContrProxy.tipLinkTimeVal = 0;
// 		Ray inputRay = Camera.main.ScreenPointToRay (Input.mousePosition);
// 		RaycastHit hit;
// 		if (Physics.Raycast (inputRay, out hit)) {
// 			switch (GameContrProxy.touchType) {
// 				case touchType.none:
// 					MeshDeformer deformer = hit.collider.GetComponent<MeshDeformer> ();
// 					if (deformer) {
// 						Vector3 point = hit.point;
// 						point += hit.normal * forceOffset;
// 						deformer.AddDeformingForce (point, force);
// 						GameContrProxy.onTouchCub (hit.collider.transform.position, hit.collider.transform);
// 						// hit.collider.GetComponent<MeshDeformer> ().AnalogTouch (anctionType.xiaoDouDong);
// 					}
// 					if (hit.collider.transform.name == "RayPlan") {
// 						Vector3 point = hit.point;
// 						GameObject linePrefab = GameContrProxy.linePrefabNow;
// 						if (linePrefab != null) {
// 							linePrefab.transform.GetComponent<DrawLineTool> ().drag (point);
// 						}
// 					}
// 					if (hit.collider.name == "btnDouble" || hit.collider.name == "btnCuiZi") {
// 						GameContrProxy.UiBox.onClick (hit.collider.gameObject);
// 					}
// 					break;
// 				case touchType._double:
// 					if (hit.collider.GetComponent<Cub> () && hit.collider.GetComponent<Cub> ().canDouble ()) {
// 						GameContrProxy.doDouble (hit.collider.gameObject);
// 					}
// 					break;
// 				case touchType.chuizi:
// 					if (hit.collider.GetComponent<Cub> ()) {
// 						GameContrProxy.doCuiZi (hit.collider.gameObject);
// 					}
// 					break;
// 				case touchType.cross:
// 					if (hit.collider.GetComponent<Cub> ()) {
// 						GameContrProxy.doCross (hit.collider.gameObject);
// 					}
// 					break;
// 				case touchType.uiMaskOpen:
// 					GameContrProxy.interval = 0.5f;
// 					break;
// 			}
// 		} else {
// 			GameContrProxy.onTouchuUp ();
// 		}
// 	}

// }