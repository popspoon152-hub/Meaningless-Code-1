using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDieState_Third : IBossStateThirdStage
{
    private BossThirdStateMachine _stateMachine;

    // 进入状态时调用（初始化）
    public void EnterState(BossThirdStateMachine stateMachine)
    {
        _stateMachine = stateMachine;

        _stateMachine.StartCoroutine(BossDie());
    }

    private IEnumerator BossDie()
    {
        //调用shader


        _stateMachine.TransfromAnim.SetTrigger("Start");
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene("GalAfterSecondStage");
    }

    // 每帧更新
    public void UpdateState()
    {

    }

    // 固定时间步长更新（物理相关）
    public void FixedUpdateState()
    {

    }

    // 退出状态时调用（清理）
    public void ExitState()
    {

    }

    // 处理动画事件
    public void OnAnimationEvent(string eventName)
    {

    }
}
