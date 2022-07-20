using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;

    [Header("판정 별 소리")]
    [SerializeField] private AudioClip[] JudgementSounds;
    [Header("스테이지 노래")]
    [SerializeField] private AudioClip[] stageMusics;
    [Header("BGM")]
    [SerializeField] private AudioClip[] BGMS;

    [SerializeField] private AudioSource gameSFXSorce;
    [SerializeField] private AudioSource StageMusicSorce;
    [SerializeField] private AudioSource BGMSorce;


    public static SoundManager Instance
    {
        get
        {
            return getInstance();
        }
    }

    private static SoundManager getInstance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }


    private void Awake()
    {
        if (this != instance || instance == null)
        {
            instance = this;
        }
    }
    
    public void PlayStageMusic()
    {
        // index 처리 필요
        if (GameData.Instance.selecttedStage == 3)
        {
            StageMusicSorce.volume = 0.5f;
        }
        else 
        {
            StageMusicSorce.volume = 1f;
        }
        StageMusicSorce.PlayOneShot(stageMusics[GameData.Instance.selecttedStage]);
    }

    public void PlayBGM(int selectedBGM)
    {
        BGMSorce.clip = BGMS[selectedBGM];
        BGMSorce.Play();
    }

    public void StopStageMusic()
    {
        StageMusicSorce.Stop();
    }
    
    public void PlaygameSFX(int judgement)
    {
        gameSFXSorce.PlayOneShot(JudgementSounds[judgement]);
    }
}
