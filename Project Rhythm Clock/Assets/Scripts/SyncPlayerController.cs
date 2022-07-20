using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 리듬게임 판정
// 모바일인 경우면 40~60ms 언저리가 표준임니다
// 참고로 투덱은 16ms, 팝픈은 20ms, 유비트는 좀 특이한데 프레임 기준이라 마커 속도따라 다르지만 35ms정도에요

public class SyncPlayerController : MonoBehaviour
{

	public GameObject panel;

	[SerializeField] private HourHandMoving hourHandMoving;
	[SerializeField] private StageSettings stageSettings;
	[SerializeField] private float SyncTime = 0.0f;

	private List<float> SyncTimings = new List<float>();
	private float StartTime = 0.0f;
	private float PlayTime = 0.0f;
	private float MusicPlayTime = 0.0f;
	private bool isStart = false;
	private bool isPlaying = false;
	private bool isMusicPlaying = false;
	private float halfSecondPerBit = 0.0f;

	private void Start()
	{
		SyncTime = 0.0f;
		halfSecondPerBit = (float)stageSettings.CPS / (float)stageSettings.CPB / 2.0f;
		SyncTimings.Clear();
		PlayTime = 0.0f;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S) && !isStart)
		{
			isStart = true;
			StartCoroutine(StartGame());
			StartCoroutine(StartBit());
			StartCoroutine(StartMusic(SyncTime));
		}

		// click, double, hold?
		if (Input.GetKeyDown(KeyCode.Space))
		{
			float minSync = -0.2f;
			float maxSync = 0.2f;
			print(PlayTime);
			while (minSync < 100f)
			{
				if (minSync < PlayTime && PlayTime < maxSync)
				{
					SyncTimings.Add(PlayTime - ((minSync + maxSync) / 2));
					break;
				}
				else
				{
					minSync += 0.5f;
					maxSync += 0.5f;
				}
			}
			
			if (SyncTimings.Count >= 10)
			{
				float sum = 0.0f;
				foreach (var timing in SyncTimings)
				{
					sum += timing;
				}
				print(sum / SyncTimings.Count);
			}

			if (SyncTimings.Count >= 20)
            {
				isStart = false;
				SoundManager.Instance.StopStageMusic();
				// 시계 바늘도 멈춰야 함

				float sum = 0.0f;
				foreach (var timing in SyncTimings)
				{
					sum += timing;
				}

				// 싱크 저장
				SyncTime = 0.25f - (sum / SyncTimings.Count);
				GameData.Instance.saveSyncTime(SyncTime);

				// 싱크 저장 정보 알림
				panel.GetComponentInChildren<Text>().text = "저장되었습니다!";
				hourHandMoving.HourHandStop();

				GameData.Instance.Clear_Stage();

				StartCoroutine(gotoScene());
				
			}
		}

		if (isStart)
		{
			StartTime += Time.deltaTime;
		}

		if (isPlaying)
		{
			PlayTime += Time.deltaTime;
		}
		
		if (isMusicPlaying)
		{
			MusicPlayTime += Time.deltaTime;
		}
	}

	IEnumerator StartGame()
	{
		yield return new WaitForSeconds(0.5f);
		hourHandMoving.HourHandStart();
		print("Play Game : " + StartTime);
	}
	
	IEnumerator StartBit()
	{
		yield return new WaitForSeconds(0.5f + halfSecondPerBit);
		isPlaying = true;
	}

	IEnumerator StartMusic(float syncTime)
	{
		yield return new WaitForSeconds(0.5f + halfSecondPerBit - syncTime);
		print("Play Music : " + StartTime);
		isMusicPlaying = true;
		SoundManager.Instance.PlayStageMusic();
	}

	IEnumerator gotoScene()
    {
		yield return new WaitForSeconds(3f);

		SceneManager.LoadScene("Main Scene");

    }

}