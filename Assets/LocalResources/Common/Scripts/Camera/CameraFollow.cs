using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;//����Ŀ��
    public float smooth=3.0f;  //ƽ��ʱ��
    public Vector2 targetOffset;//���ٵ�ƫ��ֵ
    public Vector3 speed = Vector3.zero;//��ʼ�ٶ�
    public float PosZ = -10;
    public Camera myCamera ;

    [Header("����ƶ�����")]
    public Vector2 lrRange = new Vector2(-10, 10);
    public Vector2 udRange = new Vector2(-5, 5);


    void Start()
    {
        myCamera = GetComponent<Camera>();
        PosZ = transform.position.z;
    }
    void LateUpdate()
    {
        if (transform.position != target.position)
        {
            Vector3 targetPos = SetTargetPos();
            targetPos.z = PosZ;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, smooth);  //��㣬�յ㣬�ٶȣ�ʱ��
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SetTargetPos(), 0.1f);

        // �������ҷ�Χ��
        Gizmos.DrawLine(new Vector2(lrRange.x, 1), new Vector2(lrRange.x, -1));
        Gizmos.DrawLine(new Vector2(lrRange.y, 1), new Vector2(lrRange.y, -1));
        // �������·�Χ��
        Gizmos.DrawLine(new Vector2(1, udRange.x), new Vector2(-1, udRange.x));
        Gizmos.DrawLine(new Vector2(1, udRange.y), new Vector2(-1, udRange.y));
    }

    private Vector3 SetTargetPos()//���ø��ٵ㣨��Ҫ������д��
    {
        float higntDistance = myCamera.orthographicSize;//��������ĵ��ϱ߽�ľ���
        float weightDistance = higntDistance * myCamera.aspect;// ���ĵ���߽�ľ��루Size �� ��߱ȣ�
        Vector3 targetPos = target.position + (Vector3)targetOffset;
        targetPos.x = Mathf.Clamp(targetPos.x, lrRange.x + weightDistance, lrRange.y - weightDistance);
        targetPos.y = Mathf.Clamp(targetPos.y, udRange.x + higntDistance, udRange.y - higntDistance);
        return targetPos;
    }
}
