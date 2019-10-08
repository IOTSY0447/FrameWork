// using komal.puremvc;
// using UnityEngine;

// public enum anctionType {
// 	down,
// 	_double,
// 	chuiZi,
// 	swallow, //吞噬
// 	cross, //十字消
// 	ya, //往下压的感觉
// 	xiaoDouDong, //小抖动
// }

// [RequireComponent (typeof (MeshFilter))]
// public class MeshDeformer : ComponentEx, INotificationHandler {

// 	public float springForce = 20f;
// 	public float damping = 5f;

// 	Mesh deformingMesh;
// 	Vector3[] originalVertices, displacedVertices; //原始顶点列表， 当前顶点列表
// 	Vector3[] vertexVelocities; // 存力的大小

// 	float uniformScale = 1f; //使力的计算跟缩放有关
// 	//自己写的代码用来调试
// 	float touchOnTimeVal = 0; //记录调试间隔
// 	float touchDisTimeDisVal = 0; //记录调试间歇的间隔
// 	private bool isAnalogTouch = false; //是否处于模拟触摸
// 	public float touchTime = 0.1f; //模拟触摸时间
// 	float touchDisTimeDis = 0; //记录调试间歇的间隔
// 	Vector3 analogTouchPos; //模拟点击的位置
// 	bool isLoop = false; //是否循环
// 	float? autoStopLoop = null; //自动关闭循环
// 	float autoStopLoopTimeVal = 0f; //自动关闭循环计时器
// 	float foce = 0; //力相关
// 	float tongYongIndex = 0;
// 	anctionType type; //记录触摸类型
// 	public Transform startCub; //起始节点
// 	//-------------------//拖拽效果//--------------------//
// 	public float dragFoce = 5f;
// 	public bool isLoopDrag = false; //是否开启循环拖拽
// 	public Vector3 dragPoint = new Vector3 (); //循环的模拟拖拽
// 	Vector3 localZero = new Vector3 (0, 0, 0.7f); //圆心。0.7表示与传进来的point同平面
// 	GameContrProxy GameContrProxy; //游戏管理起
// 	void Start () {
// 		GameContrProxy = this.facade.RetrieveProxy (ProxyNameEnum.GameContrProxy) as GameContrProxy;
// 		init ();
// 	}
// 	public void init () {
// 		deformingMesh = GetComponent<MeshFilter> ().mesh;
// 		originalVertices = deformingMesh.vertices;
// 		displacedVertices = new Vector3[originalVertices.Length];
// 		for (int i = 0; i < originalVertices.Length; i++) {
// 			displacedVertices[i] = originalVertices[i];
// 		}
// 		vertexVelocities = new Vector3[originalVertices.Length];
// 	}

// 	void Update () {
// 		uniformScale = transform.localScale.x;
// 		if (isLoopDrag) {
// 			drag (dragPoint);
// 		}
// 		for (int i = 0; i < displacedVertices.Length; i++) {
// 			UpdateVertex (i);
// 		}
// 		deformingMesh.vertices = displacedVertices;
// 		deformingMesh.RecalculateNormals (); //跟新法线，没光照应该不用

// 		AnalogTouchAnction (); //模拟点击效果
// 	}

// 	void UpdateVertex (int i) {
// 		Vector3 velocity = vertexVelocities[i];
// 		Vector3 displacement = displacedVertices[i] - originalVertices[i];
// 		displacement *= uniformScale;
// 		velocity -= displacement * springForce * Time.deltaTime;
// 		velocity *= 1f - damping * Time.deltaTime; //力
// 		vertexVelocities[i] = velocity;
// 		displacedVertices[i] += velocity * (Time.deltaTime / uniformScale);
// 	}

// 	public void AddDeformingForce (Vector3 point, float force) {
// 		point = transform.InverseTransformPoint (point);
// 		for (int i = 0; i < displacedVertices.Length; i++) {
// 			AddForceToVertex (i, point, force);
// 		}
// 	}

// 	void AddForceToVertex (int i, Vector3 point, float force) {
// 		Vector3 pointToVertex = displacedVertices[i] - point;
// 		pointToVertex *= uniformScale;
// 		float attenuatedForce = force / (1f + pointToVertex.sqrMagnitude); //这保证当距离为零时，力处于全强度,而不是无限大 Fv=Fd2;
// 		float velocity = attenuatedForce * Time.deltaTime;
// 		vertexVelocities[i] += pointToVertex.normalized * velocity;
// 	}

// 	//模拟触摸 不支持多个动画叠加
// 	public void AnalogTouch (anctionType type) {
// 		this.type = type;
// 		isAnalogTouch = true;
// 		switch (type) {
// 			case anctionType.down: //下落
// 				touchTime = 0.1f;
// 				analogTouchPos = new Vector3 (0.5f, -0.6f, 0.7f);
// 				foce = 100;
// 				isLoop = false;
// 				autoStopLoop = null;
// 				break;
// 			case anctionType._double: //乘2
// 			case anctionType.cross: //十字消
// 				touchTime = 0.2f;
// 				foce = 30;
// 				isLoop = true;
// 				touchDisTimeDis = 0.9f;
// 				autoStopLoop = null;
// 				break;
// 			case anctionType.chuiZi: //锤子
// 				touchTime = 0.1f;
// 				foce = 40;
// 				isLoop = true;
// 				touchDisTimeDis = 1;
// 				autoStopLoop = null;
// 				break;
// 			case anctionType.swallow:
// 				touchTime = 0.2f;
// 				foce = 130;
// 				analogTouchPos = new Vector3 (-0.5f, -0.4f, 0.4f);
// 				analogTouchPos = new Vector3 (-0.5f, 0.4f, -0.4f);
// 				autoStopLoop = null;
// 				break;
// 			case anctionType.ya: //下压
// 				touchTime = 0.4f;
// 				analogTouchPos = new Vector3 (0, 0, 1f);
// 				foce = 150;
// 				isLoop = false;
// 				autoStopLoop = null;
// 				break;
// 			case anctionType.xiaoDouDong: //小抖动
// 				// xiaoDouDong ();
// 				// isAnalogTouch = false;
// 				touchTime = 0f;
// 				foce = 100;
// 				isLoop = true;
// 				touchDisTimeDis = 0f;
// 				autoStopLoop = 2f;
// 				break;
// 		}
// 	}
// 	public void stopLoop () {
// 		isLoop = false;
// 	}
// 	//模拟触摸动画
// 	void AnalogTouchAnction () {
// 		if (isAnalogTouch) {
// 			touchOnTimeVal += Time.deltaTime;
// 			Vector3 v3 = Vector3.zero;
// 			switch (type) {
// 				case anctionType.ya:
// 				case anctionType.down:
// 					v3 = transform.TransformPoint (analogTouchPos);
// 					break;
// 				case anctionType._double:
// 				case anctionType.cross:
// 				case anctionType.chuiZi:
// 					v3 = transform.TransformPoint (getRandomPoint ());
// 					break;
// 				case anctionType.xiaoDouDong:
// 					v3 = transform.TransformPoint (xiaoDouDong2 ());
// 					break;
// 			}
// 			AddDeformingForce (v3, foce);
// 			if (touchOnTimeVal > touchTime) {
// 				touchOnTimeVal = 0;
// 				isAnalogTouch = false;
// 			}
// 		}
// 		if (isLoop) {
// 			touchDisTimeDisVal += Time.deltaTime;
// 			if (touchDisTimeDisVal > touchDisTimeDis) {
// 				touchDisTimeDisVal = 0;
// 				isAnalogTouch = true;
// 			}
// 			if (autoStopLoop != null) {
// 				autoStopLoopTimeVal += Time.deltaTime;
// 				if (autoStopLoopTimeVal > autoStopLoop) {
// 					autoStopLoopTimeVal = 0;
// 					isLoop = false;
// 				}
// 			}
// 		}
// 	}
// 	Vector3 getRandomPoint () {
// 		return new Vector3 (Random.Range (-0.5f, 0.5f), Random.Range (-0.5f, 0.5f), Random.Range (0, 1f));
// 	}

// 	//拖拽变形
// 	public void drag (Vector3 point) {
// 		point = transform.InverseTransformPoint (point);
// 		float maxFDis = 3f; //最大力矩
// 		// Vector3 localzero = Vector3.zero; //与连线平行的点//本地坐标 //通过打印发现point的z值是固定的//这坐标我都晕了
// 		// localzero.z += 0.7f;
// 		Vector3 pointJiao = localZero + (point - localZero).normalized * 0.5f; //交点坐标 //半径都是0.5，以为是在z值固定的情况下拉伸的
// 		pointJiao.z = 1; //本模型高度为1，1表示吧受力点改为最表面
// 		for (int i = 0; i < originalVertices.Length; i++) {
// 			Vector3 pointToVertex = point - displacedVertices[i];
// 			pointToVertex *= uniformScale;
// 			float F = dragFoce * Mathf.Min (maxFDis, pointToVertex.sqrMagnitude); //力跟距离是成正方向比的  
// 			float rat = Mathf.Pow (originalVertices[i].z, 2); //z轴距离已经是归一化的 //平方使得关系呈曲线
// 			vertexVelocities[i] += pointToVertex.normalized * rat * F * Time.deltaTime;
// 		}
// 	}
// 	//设置拖拽状态
// 	public void setDragStatu (bool isDrag, Vector3 dragP = new Vector3 ()) {
// 		isLoopDrag = isDrag;
// 		dragPoint = dragP;
// 	}
// 	//小抖动
// 	public void swallow () {
// 		Vector3 point = new Vector3 (0, 0, 0.5f);
// 		for (int i = 0; i < originalVertices.Length; i++) {
// 			Vector3 pointToVertex = displacedVertices[i] - point;
// 			pointToVertex *= uniformScale;
// 			vertexVelocities[i] += pointToVertex.normalized * 200 * Time.deltaTime;
// 		}
// 	}
// 	//小抖动
// 	public Vector3 xiaoDouDong2 () {
// 		Vector3 zero = new Vector3 (0, 0, 1);
// 		Vector3 _fromV3 = transform.InverseTransformPoint (startCub.position);
// 		Vector3 _toV3 = transform.InverseTransformPoint (transform.position);
// 		Vector3 VV3 = _toV3 - _fromV3;
// 		Vector3 fromV3 = zero - VV3 * 1 * 0.5f / (1 + 1); // 模型长度*0.5/（模型长度 + padding）
// 		Vector3 toV3 = zero + VV3 * 1 * 0.5f / (1 + 1);;
// 		float speed = 0.1f;
// 		Vector3 touchV3 = VV3 * speed * tongYongIndex + fromV3;
// 		tongYongIndex += 1f;
// 		if (tongYongIndex > 10) {
// 			tongYongIndex = 0;
// 			isLoop = false;
// 		}
// 		return touchV3;
// 	}
// }