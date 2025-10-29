using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ThirdStage/EnvironmentStats")]
public class ThirdStageEnvironmentStats : ScriptableObject
{
    [Header("����")]
    public GameObject StonesPrefabs;                                                                    //ʯͷԤ����

    [Header("��ֵ�趨")]
    [Range(1f, 10f)] public float StoneMinSize = 1f;                                                           //���ɵ�ʯͷ����С�ߴ�
    [Range(1f, 10f)] public float StoneMaxSize = 3f;                                                           //���ɵ�ʯͷ�����ߴ�

    public Vector2 InstantiateLeftPosition;                                                             //ʯͷ���ɵ���߽�λ��
    public Vector2 InstantiateRightPosition;                                                            //ʯͷ���ɵ��ұ߽�λ��

    [Range(1f, 5f)] public int StoneMinInstantiateNum = 1;                                                  //ÿ������ʯͷ����С����   
    [Range(1f, 5f)] public int StoneMaxInstantiateNum = 3;                                                  //ÿ������ʯͷ���������

    [Header("Time")]
    [Range(0f, 15f)] public float StoneMinInstantiateInterval = 7f;                                           //ÿ��ʯͷ��С���ɼ��ʱ��
    [Range(0f, 15f)] public float StoneMaxInstantiateInterval = 10f;                                           //ÿ��ʯͷ������ɼ��ʱ��

    [Range(0f,1f)] public float EachStoneMinInstantiateDelay = 0.3f;                                            //ÿ��ʯͷ���ɵ���С���ʱ��
    [Range(0f,1f)] public float EachStoneMaxInstantiateDelay = 0.8f;                                            //ÿ��ʯͷ���ɵ������ʱ��
}
