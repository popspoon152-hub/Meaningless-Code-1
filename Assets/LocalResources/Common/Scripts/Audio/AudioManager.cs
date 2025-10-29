using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class Audio
{
    public AudioClip Clip;
    public bool Loop;
    public bool PlayOnAwake;
    public AudioTag Tag;
}

public enum AudioTag
{
    BGM,
    FX,
}

public enum EAudioControl
{
    Play,
    Pause,
    Stop,
}


public class AudioManager : MonoBehaviour
{
    public List<Audio> audioClips = new List<Audio>();

    // �洢������Ͷ�Ӧ��AudioSource������ֵ�
    private static Dictionary<string, AudioSource> _audioSources;

    private void Awake()
    {
        PlayerSettings.Ins.LoadPlayerSettings();
        // ��ʼ���ֵ�
        _audioSources = new Dictionary<string, AudioSource>();
        var bgmVolume = PlayerSettings.Ins.BGMVolume / 100f;
        var fxVolume = PlayerSettings.Ins.FXVolume / 100f;
        // Ϊÿ����Ƶ�ļ�����һ���������AudioSource���
        foreach (Audio clip in audioClips)
        {
            GameObject audioObject = new GameObject(clip.Clip.name);
            audioObject.transform.parent = transform; // ���ø�����Ϊ��Ƶ������
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();

            audioSource.clip = clip.Clip;               // ������Ƶ����
            audioSource.playOnAwake = clip.PlayOnAwake; // ��ֹ�Զ�����
            audioSource.volume = clip.Tag == AudioTag.BGM ? bgmVolume : fxVolume;
            audioSource.loop = clip.Loop;

            // ��AudioSource��ӵ��ֵ���
            _audioSources.Add(clip.Clip.name, audioSource);
            if (audioSource.playOnAwake)
            {
                audioSource.Play();
            }
        }
        EventCenter.Ins.AddListener(EAudioControl.Play, PlayAudio);
        EventCenter.Ins.AddListener(EAudioControl.Pause, PauseAudio);
        EventCenter.Ins.AddListener(EAudioControl.Stop, StopAudio);
    }

    // һ���򵥵Ĳ�����Ƶ�ķ���������ͨ����Ƶ����������
    public void PlayAudio(List<object> args)
    {
        PlayAudio((string)args[0]);
    }

    public void PlayAudio(string audioName)
    {
        if (_audioSources.ContainsKey(audioName))
        {
            _audioSources[audioName].Play();
        }
    }

    public void StopAudio(List<object> args)
    {
        StopAudio((string)args[0]);
    }

    // һ���򵥵�ֹͣ��Ƶ�ķ���������ͨ����Ƶ������ֹͣ
    public void StopAudio(string audioName)
    {
        if (_audioSources.ContainsKey(audioName))
        {
            _audioSources[audioName].Stop();
        }
    }

    public void PauseAudio(List<object> args)
    {
        PauseAudio((string)args[0]);
    }

    // һ���򵥵�ֹͣ��Ƶ�ķ���������ͨ����Ƶ������ֹͣ
    public void PauseAudio(string audioName)
    {
        if (_audioSources.ContainsKey(audioName))
        {
            _audioSources[audioName].Pause();
        }
    }
}
