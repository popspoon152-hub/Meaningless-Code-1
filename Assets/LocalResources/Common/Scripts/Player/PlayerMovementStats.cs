using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player Movement")] 
public class PlayerMovementStats : ScriptableObject
{
    /// <summary>
    /// ���������ο�
    /// </summary> 

    [Header("Walk")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 12.5f;                        //����ٶ�
    [Range(0.25f, 50f)] public float GroundAccleration = 5f;                    //������ٶ�
    [Range(0.25f, 50f)] public float GroundDeceleration = 20f;                  //������ٶ�               
    [Range(0.25f, 50f)] public float AirAccleration = 5f;                      //�������ٶ�
    [Range(0.25f, 50f)] public float AirDeceleration = 5f;                     //�������ٶ�

    [Header("Run")]
    [Range(1f, 100f)] public float MaxRunSpeed = 20f;                           //����ٶ�

    [Header("������")]
    public LayerMask GroundLayer;                                               //�����
    public float GroundDetectionRayLength = 0.02f;                              //���������߳���
    public float HeadDetectionRayLength = 0.02f;                                //ͷ��������߳���
    [Range(0f, 1f)] public float HeadWidth = 0.75f;                             //ͷ��

    [Header("Jump")]
    public float JumpHeight = 6.5f;                                             //��Ծ�߶�
    [Range(1f, 2f)] public float JumpHeightCompensationFactor = 1.054f;       //��Ծ������ϵ��(����ڽ��ж̰���Ծʱ����С�߶�)
    public float TimeTillJumpApex = 0.35f;                                      //������ߵ������ʱ��
    [Range(0.01f, 5f)] public float GravityOnReleaseMultiplier = 2f;            //�ɿ�����ʱ����������(������ڿ����ɿ���Ծ��ʱ��ʩ���ڽ�ɫ���ϵ�������������ϵ����Ϊ���������ø��߻����)
    public float MaxFallSpeed = 26f;                                            //��������ٶ�
    [Range(1, 5)] public int NumberOfJumpsAllowed = 2;                          //���������Ծ����

    [Header("JumpCut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardCancel = 0.027f;             //����ȡ�������ٶȵ�ʱ��(��������ͨ���ɿ�������ֱ��ȡ�������ٶ�)

    [Header("JumpApex")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;                       //��ߵ���ֵ
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;                      //��ߵ���ͣʱ��

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;                         //������Ծ��,����ɫ��ʱ����������������ʱ,�Ὣ�����Ծָ����ʱ��������.��������ʱ�䴰���ڽ�ɫ��ÿ�������,��ôϵͳ���Զ�ִ�������Ծ

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;                         //����ɫ��ƽ̨��Ե������һ��ʱ����,ϵͳ��Ȼ��Ϊ��ɫ���ڵ�����,��������Ծ

    

    [Header("Debug")]
    public bool DebugShowIsGroundedBox;
    public bool DebugShowHeadBumpBox;

    [Header("JumpVisualization Tool")]
    public bool ShowWalkJumpArc = false;
    public bool ShowRunJumpArc = false;
    public bool ShowDashJumpArc = false;
    public bool StopOnCollision = true;
    public bool DrawRight = true;
    [Range(5, 100)] public int ArcSolution = 20;
    [Range(0, 500)] public int VisualizationSteps = 90;

    public float AdjustedJumpHeight { get; private set; }
    public float Gravity { get; private set; }
    public float IntialJumpVelocity { get; private set; }

    private void OnValidate()
    {
        CalculateValues();
    }

    private void OnEnable()
    {
        CalculateValues();
    }

    public void CalculateValues()
    {
        AdjustedJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        IntialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }

    [Header("���")]
    [Range(1f, 20f)] public float MaxDashLength = 5f;                           // ��̾���
    [Range(0f, 10f)] public float DashCooldown = 1.0f;                          // �����ȴʱ��
    [Range(0f, 1f)] public float DashDuration = 0.2f;                           // ��̳���ʱ��

    //[Header("��")]

}
