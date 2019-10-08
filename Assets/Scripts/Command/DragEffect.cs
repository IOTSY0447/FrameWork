using System.Collections;
using System.Collections.Generic;
using komal.puremvc;
using UnityEngine;
public class DragEffect : ComponentEx, INotificationHandler {
    public float num1 = 0.008f;
    public float num2 = 5f;

    Mesh deformingMesh; //网格
    Vector3[] originalVertices, displacedVertices; //原始，位移
    public bool isDrag = false;//是否真被拉
    void Start () {
        deformingMesh = GetComponent<MeshFilter> ().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++) {
            displacedVertices[i] = originalVertices[i];
        }
    }
    void Update () {

    }
    //拖拽变形
    //如果果冻效果也开着，会有收缩力相互影响
    public void drag1 (Vector3 point) {
        point = transform.InverseTransformPoint (point);
        float maxFDis = 3f; //最大力矩
        float maxDis = 1.42f; //该模型内任意亮点的最大距离 ，根号2 
        float num = 1.5f; //修正力，使受力更均匀；//这个值越大受力越均匀
        float guiOne = 1 / num - 1 / (maxDis + num); //归一化参数
        Vector3 zero = Vector3.zero; //与连线平行的点//本地坐标 //通过打印发现point的z值是固定的//这坐标我都晕了
        zero.z += 0.7f;
        Vector3 pointJiao = zero + (point - zero).normalized * 0.5f; //交点坐标 //半径都是0.5，以为是在z值固定的情况下拉伸的
        for (int i = 0; i < originalVertices.Length; i++) {
            Vector3 pointToVertex = point - originalVertices[i];
            float F = num1 * Mathf.Min (maxFDis, pointToVertex.sqrMagnitude); //力跟距离是成正方向比的  
            float ratDis = Vector3.Distance (originalVertices[i], pointJiao);
            float Rat = 1 / ((ratDis + num) * guiOne); //受力百分比 底数加num，使受力更加均匀
            float _rat = Mathf.Pow (Rat, 4); // 归一化后算其4次方，使力呈现抛物线，主要为了让靠边的几乎为0//事实证明归一化好像有问题
            displacedVertices[i] = originalVertices[i] + pointToVertex.normalized * _rat * F;
        }
        deformingMesh.vertices = displacedVertices;
    }
    //拖拽变形
    //如果果冻效果也开着，会有收缩力相互影响
    public void drag (Vector3 point) {
        isDrag = true;
        point = transform.InverseTransformPoint (point);
        float maxFDis = 3f; //最大力矩
        Vector3 zero = Vector3.zero; //与连线平行的点//本地坐标 //通过打印发现point的z值是固定的//这坐标我都晕了
        zero.z += 0.7f;
        Vector3 pointJiao = zero + (point - zero).normalized * 0.5f; //交点坐标 //半径都是0.5，以为是在z值固定的情况下拉伸的
        pointJiao.z = 1;
        for (int i = 0; i < originalVertices.Length; i++) {
            Vector3 pointToVertex = point - originalVertices[i];
            float F = num2 * Mathf.Min (maxFDis, pointToVertex.sqrMagnitude); //力跟距离是成正方向比的  
            displacedVertices[i] = originalVertices[i] + pointToVertex.normalized * originalVertices[i].z * F;
        }
        deformingMesh.vertices = displacedVertices;
    }
}