using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDieState_Third : IBossStateThirdStage
{
    private BossThirdStateMachine _stateMachine;

    // ����״̬ʱ���ã���ʼ����
    public void EnterState(BossThirdStateMachine stateMachine)
    {
        _stateMachine = stateMachine;

        _stateMachine.StartCoroutine(BossDie());
    }

    private IEnumerator BossDie()
    {
        //����shader


        _stateMachine.TransfromAnim.SetTrigger("Start");
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("GalAfterSecondStage");
    }

    // ÿ֡����
    public void UpdateState()
    {

    }

    // �̶�ʱ�䲽�����£�������أ�
    public void FixedUpdateState()
    {

    }

    // �˳�״̬ʱ���ã�����
    public void ExitState()
    {

    }

    // �������¼�
    public void OnAnimationEvent(string eventName)
    {

    }
}
