using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class StartPageSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private const string VolumePrefKey = "GameVolume";

    private void Start()
    {
        // �ӱ��ض�ȡ���������ֵ
        float savedVolume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);

        // ��ʼ�� slider
        volumeSlider.value = savedVolume;

        // Ӧ������
        SetVolume(savedVolume);

        // ע���¼�����
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat(VolumePrefKey, value);
    }
}
