using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartPageVideo : MonoBehaviour
{
    [Header("Videos")]
    public StartPageStats StartStats;
    public VideoClip OpeningAnimation;
    public VideoClip StartVideo;
    public VideoClip GameFinishedVideo;
    public VideoClip ExitVideo;

    [Header("Audios")]
    public AudioClip StartAudio;
    public AudioClip SecondStartAudio;

    public AudioSource CurrentAudio;

    public Image TitleImage;
    public Image VolumeImage;
    public float Speed = 0.2f;

    public Slider VolumeSlider;

    public MyButton StartButton;
    [SerializeField] private float fadeDuration = 2.0f;

    public VideoPlayer VideoPlayer;

    //private bool isTransitioning = false;

    void Start()
    {
        TitleImage.gameObject.SetActive(false);
        StartButton.gameObject.SetActive(false);
        VolumeSlider.gameObject.SetActive(false);
        VolumeImage.gameObject.SetActive(false);

        CurrentAudio.clip = StartAudio;
        CurrentAudio.Stop();

        StartButton.OnDoubleClick.AddListener(ExitStartPage);

        StartCoroutine(PlayOpeningAnimation());
    }

    private IEnumerator PlayOpeningAnimation()
    {
        if (OpeningAnimation != null)
        {
            VideoPlayer.clip = OpeningAnimation;
            VideoPlayer.isLooping = false;
            VideoPlayer.Play();

            // �ȴ����������������
            yield return new WaitForSeconds((float)OpeningAnimation.length);

        }
        EnterMainInterface();
    }

    private void EnterMainInterface()
    {
        if (!StartStats.IsGameFinished)
        {
            VideoPlayer.clip = StartVideo;
            CurrentAudio.clip = StartAudio;      
        }
        else
        {
            VideoPlayer.clip = GameFinishedVideo;
            CurrentAudio.clip = SecondStartAudio;
        }
            
        VideoPlayer.isLooping = true;
        VideoPlayer.Play();

        CurrentAudio.loop = true;
        CurrentAudio.Play();

        StartCoroutine(FadeIn());
        StartButton.gameObject.SetActive(true);
        VolumeSlider.gameObject.SetActive(true);
        VolumeImage.gameObject.SetActive(true);
    }

    private IEnumerator FadeIn()
    {
        TitleImage.gameObject.SetActive(true);

        float elapsedTime = 0f;
        UnityEngine.Color originalColor = TitleImage.color;
        UnityEngine.Color transparentColor = new UnityEngine.Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        UnityEngine.Color targetColor = new UnityEngine.Color(originalColor.r, originalColor.g, originalColor.b, 1f);

        // ��ʼ����Ϊ͸��
        TitleImage.color = transparentColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            TitleImage.color = new UnityEngine.Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // ȷ��������ȫ��͸��
        TitleImage.color = targetColor;
    }

    private void ExitStartPage()
    {
        StartButton.gameObject.SetActive(false);
        VolumeSlider.gameObject.SetActive(false);
        VolumeImage.gameObject.SetActive(false);
        CurrentAudio.Stop();

        StartCoroutine(PlayExitVideoAndLoadScene());
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        UnityEngine.Color originalColor = TitleImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            TitleImage.color = new UnityEngine.Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // ȷ��������ȫ͸��
        TitleImage.color = new UnityEngine.Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    private IEnumerator PlayExitVideoAndLoadScene()
    {
        // �����˳���Ƶ
        VideoPlayer.clip = ExitVideo;
        VideoPlayer.isLooping = false;
        VideoPlayer.Play();

        // �ȴ���Ƶ���ȵ�ʱ��
        yield return new WaitForSeconds((float)ExitVideo.length);

        // ���س���
        CurrentAudio.Stop();
        SceneManager.LoadScene("GalBeforeFirstStage");
    }









    //void Start()
    //{
    //    VideoPlayer.clip = StartMoveVideo;
    //    VideoPlayer.isLooping = true;
    //    VideoPlayer.Play();

    //    StartButton.OnDoubleClick.AddListener(ExitStartPage);
    //    StartButton.gameObject.SetActive(true);
    //}

    //private void OnDestroy()
    //{
    //    StartButton.OnDoubleClick.RemoveListener(ExitStartPage);
    //    VideoPlayer.loopPointReached -= OnStartMoveVideoFinished;
    //    //VideoPlayer.loopPointReached -= OnExitVideoFinished;
    //}

    //private void ExitStartPage()
    //{
    //    if (isTransitioning) return;

    //    isTransitioning = true;
    //    StartButton.gameObject.SetActive(false);

    //    VideoPlayer.loopPointReached -= OnStartMoveVideoFinished;
    //    VideoPlayer.loopPointReached += OnStartMoveVideoFinished;

    //    VideoPlayer.clip = ExitVideo;
    //    VideoPlayer.isLooping = false;
    //    VideoPlayer.Play();

    //    Debug.Log("��ʼ�����˳���Ƶ");
    //}

    //private void OnStartMoveVideoFinished(VideoPlayer source)
    //{
    //    Debug.Log("��ʼ�ƶ���Ƶ�������");
    //    VideoPlayer.loopPointReached -= OnStartMoveVideoFinished;

    //    SceneManager.LoadScene("FirstStagePage");
    //}

    ////private void OnExitVideoFinished(VideoPlayer source)
    ////{
    ////    VideoPlayer.loopPointReached -= OnExitVideoFinished;
    ////    SceneManager.LoadScene("FirstStagePage");
    ////}
}
