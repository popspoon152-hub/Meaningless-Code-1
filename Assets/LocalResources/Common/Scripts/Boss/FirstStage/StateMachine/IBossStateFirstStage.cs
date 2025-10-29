using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBossStateFirstStage
{
    // ����״̬ʱ���ã���ʼ����
    void EnterState(BossFirstStateMachine stateMachine);

    // ÿ֡����
    void UpdateState();

    // �̶�ʱ�䲽�����£�������أ�
    void FixedUpdateState();

    // �˳�״̬ʱ���ã�����
    void ExitState();

    // �������¼�
    void OnAnimationEvent(string eventName);
}
