using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 리듬게임 판정
// 모바일인 경우면 40~60ms 언저리가 표준임니다
// 참고로 투덱은 16ms, 팝픈은 20ms,
// 유비트는 좀 특이한데 프레임 기준이라 마커 속도따라 다르지만 35ms정도에요


public class PlayerController : MonoBehaviour
{
	[SerializeField] private TimingManager theTimingManager;
	[SerializeField] private NoteManager theNoteManager;
	[SerializeField] private StageSettings stageSettings;
	[SerializeField] private MovingClockField theMovingClockField;
	[SerializeField] private float SyncTime = 0.0f;
	[SerializeField] private int StageCount = 0;
	[SerializeField] private GameObject StageInfo;
	
	
	private float StartTime = 0.0f;
	private float PlayTime = 0.0f;
	public float MusicPlayTime = 0.0f;
	private bool isStart = false;
	private bool isPlaying = false;
	private bool isMusicPlaying = false;
	private float halfSecondPerBit = 0.0f;
	private int score = 0;
	
	public bool isHold = false;
	private bool isHoldEnd = false;

	private void Start()
	{
		halfSecondPerBit = stageSettings.CPS / stageSettings.CPB / 2.0f;
		SyncTime = GameData.Instance.syncTime;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S) )
		{
			gameStart();
		}

		// click, double, holdStart
		if (Input.GetKeyDown(KeyCode.Space))
		{
			// hold note인지 확인한 다음에 해야할거 같은데, 안그러면 더블일때도 달릴려고 함
			// hold를 처음 누를때 시간이 hs랑 맞는지 확인해야함
			int tmpScore = theTimingManager.CheckClickTiming();
			if (tmpScore >= 0)
			{
				score += tmpScore;
				// print(score);
			}

		}

		// holdEnd
		if (Input.GetKeyUp(KeyCode.Space))
		{
			holdEnd();
		}

		// hold
		if (isHold)
		{
			GameManager.Instance.map.GetComponent<BackgroundOperation>().backgroundScroll();
			theTimingManager.characterAnim.SetTrigger("Run");
			if (theTimingManager.isOverHoldTime())
			{
				holdEnd();
			}
		}
        else
        {
			theTimingManager.characterAnim.SetTrigger("Idle");
		}

		if (Input.GetKeyDown(KeyCode.P))
		{
			stageSettings.ChangeBPM(240);
		}

		if (Input.GetKeyDown(KeyCode.O))
		{
			stageSettings.ChangeBPM(120);
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

		if (MusicPlayTime >= GameManager.Instance.FinishTime)
        {
			gameFinish();
			GameManager.Instance.SuccessStage();
        }

	}

	private void gameStart()
	{
		if (!isStart)
		{
			isStart = true;
			StartCoroutine(StartGame());
			StartCoroutine(StartBit());
			StartCoroutine(StartMusic(SyncTime));

			StageInfo.SetActive(false);
		}
	}

	public void gameFinish()
    {
		if (isStart)
        {
			isStart = false;
			isPlaying = false;
			isMusicPlaying = false;

			theNoteManager.finishGame();
			theNoteManager.DestroyNotes(); // Destroyable = true가 필요할까
			SoundManager.Instance.StopStageMusic();
        }
    }

	public void holdEnd()
	{
		if (isHold)
		{
			isHold = false;
			// hold를 뗏을때 이때 시간이 he랑 맞는지 확인해야함
			int tmpScore = theTimingManager.CheckHoldEndTiming();
			if (tmpScore >= 0)
			{
				score += tmpScore;
				// print(score);
			}
		}
		
	}

	IEnumerator StartGame()
	{
		yield return new WaitForSeconds(0.5f);
		theNoteManager.startGame();
		theMovingClockField.startGame();
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
}