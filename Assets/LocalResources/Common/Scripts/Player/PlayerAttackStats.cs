using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Attack")]
public class PlayerAttackStats : ScriptableObject
{
    [Header("Attack Action")]
    public float AttackNumberCount = 3;                                                 //��������
    [Range(1f, 100f)] public float[] ComboDamage = { 10f, 10f, 20f };                          //ÿ�ι������˺�

    [Header("Attack Time")]
    [Range(0.1f, 1f)] public float AttackComboWindow = 0.4f;                            //����ʱ�䴰��
    [Range(0.1f, 1f)] public float[] AttackDuration = { 0.3f, 0.4f, 0.5f };             //ÿ�ι����ĳ���ʱ��
    [Range(0.1f, 1f)] public float AttackBuffer = 0.1f;                                 //��������ʱ��

    [Header("Attack Postion")]
    [Range(0.1f, 30f)] public float[] AttackRange = { 3f, 4f, 5f };                       //������Χ
    [Range(0f, 3f)] public float[] AttackLittleDash = { 0.1f, 0.1f, 0.1f };             //ÿ�ι�����С��̾���

    [Header("Attack Layer")]
    public LayerMask EnemyLayer;                                                        //���˲�
    public LayerMask BeanLayer;                                                     //�ϰ���
    public LayerMask GroundLayer;                                                   //�����
    public LayerMask RewardLayer;                                                   //������

    [Header("AttackVisualization Tool")]
    public bool ShowAttackRangeArc = false;

    [Header("������Ѫ��ֵ")]
    [Range(1f, 50f)] public float AttackHurtPlayerNum = 10f;                            //�Լ���Ѫ 
    public float PlayerAttackUpPerHealth;
}
