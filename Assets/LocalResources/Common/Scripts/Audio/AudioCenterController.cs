using CutyRoom.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class AudioCenterController : MonoBehaviour
{
    /// <summary>
    /// ����AudioManager�趨����Ƶͨ��Ѱ�����Ƶķ�ʽ������
    /// </summary>
    private void Awake()
    {
        #region Player
        EventCenter.Ins.AddListener(EPlayerFirstAttack.on, HandlePlayerFirstAttackOn);
        EventCenter.Ins.AddListener(EPlayerFirstAttack.off, HandlePlayerFirstAttackOff);

        EventCenter.Ins.AddListener(EPlayerSecondAttack.on, HandlePlayerSecondAttackOn);
        EventCenter.Ins.AddListener(EPlayerSecondAttack.off, HandlePlayerSecondAttackOff);

        EventCenter.Ins.AddListener(EPlayerThirdAttack.on, HandlePlayerThirdAttackOn);
        EventCenter.Ins.AddListener(EPlayerThirdAttack.off, HandlePlayerThirdAttackOff);

        EventCenter.Ins.AddListener(EPlayerDash.on, HandlePlayerDashOn);
        EventCenter.Ins.AddListener(EPlayerDash.off, HandlePlayerDashOff);

        EventCenter.Ins.AddListener(EPlayerBack.on, HandlePlayerBackOn);

        EventCenter.Ins.AddListener(EPlayerLaser.on, HandlePlayerLaserOn);

        EventCenter.Ins.AddListener(EPlayerLaser.on, HandlePlayerLaserOn);

        EventCenter.Ins.AddListener(EPlayerBirth.on, HandlePlayerBirthOn);

        EventCenter.Ins.AddListener(EPlayerHurt.on, HandlePlayerHurtOn);

        EventCenter.Ins.AddListener(EPlayerDeath.on, HandlePlayerDeathOn);
        #endregion

        #region Boss
        EventCenter.Ins.AddListener(EBossDash.on, HandleBossDashOn);

        EventCenter.Ins.AddListener(EBossEatBeansMove.on, HandleBossEatBeansMoveOn);

        EventCenter.Ins.AddListener(EBossEatBean.on, HandleBossEatBeanOn);

        EventCenter.Ins.AddListener(EBossRangedAttack.on, HandleBossRangedAttackOn);
        #endregion
    }

    #region PlayerAudios
    /// <summary>
    /// һ��ƽa
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerFirstAttackOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "һ����ƽA" });
    }

    private void HandlePlayerFirstAttackOff(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Stop, new List<object> { "һ����ƽA" });
    }

    /// <summary>
    /// ����ƽa
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerSecondAttackOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "һ����ƽA" });
    }

    private void HandlePlayerSecondAttackOff(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Stop, new List<object> { "һ����ƽA" });
    }

    /// <summary>
    /// ����ƽa
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerThirdAttackOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "������ƽA" });
    }

    private void HandlePlayerThirdAttackOff(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Stop, new List<object> { "������ƽA" });
    }

    /// <summary>
    /// ���
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerDashOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "�����Ч" });
    }

    private void HandlePlayerDashOff(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Stop, new List<object> { "�����Ч" });
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerBackOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "������Ч" });
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerLaserOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "������Ч" });
    }

    /// <summary>
    /// ��ɫ����
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerBirthOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "��ɫ������Ч" });
    }

    /// <summary>
    /// ��ɫ����
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerHurtOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "��ɫ����" });
    }

    /// <summary>
    /// ��ɫ����
    /// </summary>
    /// <param name="list"></param>
    private void HandlePlayerDeathOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "��ɫ����" });
    }
    #endregion

    #region BossAudio
    /// <summary>
    /// Boss���
    /// </summary>
    /// <param name="list"></param>
    private void HandleBossDashOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "boss�����Ч+�ӽ����Ӽ�����Ч" });
    }

    /// <summary>
    /// Boss�Զ��ƶ�
    /// </summary>
    /// <param name="list"></param>
    private void HandleBossEatBeansMoveOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "boss�����Ч+�ӽ����Ӽ�����Ч" });
    }

    /// <summary>
    /// Boss�Զ�
    /// </summary>
    /// <param name="list"></param>
    private void HandleBossEatBeanOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "boss�Զ���Ч" });
    }

    /// <summary>
    /// boss��Ļ����
    /// </summary>
    /// <param name="list"></param>
    private void HandleBossRangedAttackOn(List<object> list)
    {
        EventCenter.Ins.Dispatch(EAudioControl.Play, new List<object> { "boss��Ļ����" });
    }
    #endregion
}
