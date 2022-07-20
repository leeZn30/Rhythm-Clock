using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Schema;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;


public class NoteManager : MonoBehaviour
{
	private class NoteInfo
	{
		public int type;
		public float width;
		public float pos;
		public int offset;
		public float alpha;
		public float doubleTime;

		public NoteInfo(int type, float width, float pos, int offset, float alpha, float doubleTime)
		{
			this.type = type;
			this.width = width;
			this.pos = pos;
			this.offset = offset;
			this.alpha = alpha;
			this.doubleTime = doubleTime;
		}
	}

	[SerializeField] private TimingManager timingManager;
	[SerializeField] private PlayerController playerController;
	[SerializeField] private NotePositionManager notePositionManager;
	[SerializeField] private StageSettings stageSettings;
	[SerializeField] private HourHandMoving hourHandMoving;
	[SerializeField] private MovingClockField movingClockField;

	private float TotalTime = 0;
	private float CycleTime = 0;
	private float CPS = 0;
	private int CPB = 0;
	private int HalfCPB = 0;
	private int Cycle = 0;
	private bool isStart = false;
	private float SecondsPerBit = 0;
	private int ChartIndex = 0;
	private float FadeTime = 0;
	private float IdleAlpha = 0.5f;
	private float holdMiddleNoteIdleAlpha = 0.25f;
	private float doubleTime = 0.1f;
	private bool Destroyable = false;

	int nextBPM;
	int currBPM;
	bool tempoDestroyable = false;

	private float RotateSpeed,
		TeleportX,
		TeleportY,
		TargetX,
		TargetY,
		SetScaleX,
		SetScaleY,
		ScaleX,
		ScaleY;

	private List<Dictionary<string, object>> NoteChart;
	private List<Dictionary<string, object>> TimingChart;


	private void Start()
	{
		CPS = stageSettings.CPS;
		CPB = stageSettings.CPB;
		SecondsPerBit = CPS / CPB;
		HalfCPB = CPB / 2;
		FadeTime = SecondsPerBit * 0.25f;

		NoteChart = GameManager.Instance.NoteChart;
		TimingChart = new List<Dictionary<string, object>>();
	}

	private void Update()
	{
		if (isStart)
		{
			CycleTime += Time.deltaTime;
			if (tempoDestroyable && (CycleTime >= CPS - FadeTime - 1f))
			{
				spawnTempoInfo();
			}

			if (Destroyable && (CycleTime >= CPS - FadeTime - 0.01f))
			{
				DestroyNotes();
			}

			if (CycleTime >= CPS)
			{
				TotalTime += CPS;
				CycleTime -= CPS;
				Cycle++;
				if (ChartIndex < NoteChart.Count)
				{
					SpwanOneCycleNotes();
				}
				else
				{
					movingClockField.TransformReset();
				}
			}
		}
	}

	void spawnTempoInfo()
	{
		if (currBPM < nextBPM)
		{
			switch (nextBPM / currBPM)
			{
				case 2:
					GameObject aTempoInfo1 = ObjectPool.Instance.TempoInfoQueue.Dequeue();
					aTempoInfo1.GetComponent<TempoInfo>().getSprite(0);
					aTempoInfo1.SetActive(true);
					aTempoInfo1.transform.position = new Vector3(5.5f, -1.5f, 0);
					break;

				case 4:
					GameObject aTempoInfo2 = ObjectPool.Instance.TempoInfoQueue.Dequeue();
					aTempoInfo2.GetComponent<TempoInfo>().getSprite(1);
					aTempoInfo2.SetActive(true);
					aTempoInfo2.transform.position = new Vector3(5.5f, -1.5f, 0);

					break;

				default:
					break;
			}
		}
		else if (currBPM > nextBPM)
		{
			switch (currBPM / nextBPM)
			{
				case 2:
					GameObject aTempoInfo3 = ObjectPool.Instance.TempoInfoQueue.Dequeue();
					aTempoInfo3.GetComponent<TempoInfo>().getSprite(2);
					aTempoInfo3.SetActive(true);
					aTempoInfo3.transform.position = new Vector3(5.5f, -1.5f, 0);
					break;

				case 4:
					GameObject aTempoInfo4 = ObjectPool.Instance.TempoInfoQueue.Dequeue();
					aTempoInfo4.GetComponent<TempoInfo>().getSprite(3);
					aTempoInfo4.SetActive(true);
					aTempoInfo4.transform.position = new Vector3(5.5f, -1.5f, 0);
					break;

				default:
					break;
			}
		}

		tempoDestroyable = false;
	}

	void SetBPM()
	{
		CPS = stageSettings.CPS;
		SecondsPerBit = CPS / CPB;
		FadeTime = SecondsPerBit * 0.25f;
		hourHandMoving.SetBPM();
	}

	public void startGame()
	{
		isStart = true;
		stageSettings.StartTime = Time.time;
		hourHandMoving.HourHandStart();
		SpwanOneCycleNotes();
	}

	public void finishGame()
	{
		isStart = false;
		stageSettings.StartTime = -1f;
		hourHandMoving.HourHandStop();
	}

	// 현재 사이클의 노트만 추출
	void SpwanOneCycleNotes()
	{
		print(int.Parse(NoteChart[ChartIndex]["BPM"].ToString()));
		currBPM = int.Parse(NoteChart[ChartIndex]["BPM"].ToString());

		stageSettings.ChangeBPM(int.Parse(NoteChart[ChartIndex]["BPM"].ToString()));
		SetBPM();
		SetMoving();

		//ClearNoteQueue();

		ExtractCycleChart();

		try
		{
			nextBPM = int.Parse(NoteChart[ChartIndex]["BPM"].ToString());
		}
		catch (ArgumentOutOfRangeException)
		{
			Debug.Log("Index out of range!");
		}


		print("nextBPM" + nextBPM + " currBPM" + currBPM);

		SpwanEnemyNotes();
		SpwanAllyNotes();

		Destroyable = true;
		tempoDestroyable = true;
	}

	void SetMoving()
	{
		movingClockField.ResetMovingKind();
		if (float.TryParse(NoteChart[ChartIndex]["TeleportX"].ToString(), out TeleportX) &&
		    float.TryParse(NoteChart[ChartIndex]["TeleportY"].ToString(), out TeleportY))
		{
			movingClockField.SetTeleportPosition(TeleportX, TeleportY);
		}

		if (float.TryParse(NoteChart[ChartIndex]["RotateSpeed"].ToString(), out RotateSpeed))
		{
			movingClockField.SetRotateSpeed(RotateSpeed);
		}

		if (float.TryParse(NoteChart[ChartIndex]["TargetX"].ToString(), out TargetX) &&
		    float.TryParse(NoteChart[ChartIndex]["TargetY"].ToString(), out TargetY))
		{
			movingClockField.SetTargetPosition(TargetX, TargetY);
		}

		if (float.TryParse(NoteChart[ChartIndex]["SetScaleX"].ToString(), out SetScaleX) &&
		    float.TryParse(NoteChart[ChartIndex]["SetScaleY"].ToString(), out SetScaleY))
		{
			movingClockField.SetScale(SetScaleX, SetScaleY);
		}

		if (float.TryParse(NoteChart[ChartIndex]["ScaleX"].ToString(), out ScaleX) &&
		    float.TryParse(NoteChart[ChartIndex]["ScaleY"].ToString(), out ScaleY))
		{
			movingClockField.TargetScale(ScaleX, ScaleY);
		}
	}

	void ExtractCycleChart()
	{
		for (int i = ChartIndex; i < NoteChart.Count; i++)
		{
			if (int.Parse(NoteChart[i]["Timing"].ToString()) == Cycle)
			{
				TimingChart.Add(NoteChart[i]);
			}
			else
				break;

			ChartIndex++;
		}
	}

	void SpwanEnemyNotes()
	{
		SpwanNotes(0, IdleAlpha, true);
	}

	void SpwanAllyNotes()
	{
		SpwanNotes(HalfCPB, IdleAlpha, false);
	}

	void SpwanNotes(int offset, float alpha, bool isEnemy)
	{
		NoteInfo holdStart = null;
		foreach (var Note in TimingChart)
		{
			int kind = int.Parse(Note["Kinds"].ToString());
			float pos = float.Parse(Note["Position"].ToString());
			float width = float.Parse(Note["Width"].ToString());

			NoteInfo noteInfo = new NoteInfo(kind, width, pos, offset, alpha, 0);
			switch (kind)
			{
				case 0:
					SpwanOneNote(noteInfo, isEnemy);
					break;
				case 1:
					SpwanOneNote(noteInfo, isEnemy);
					noteInfo.doubleTime = doubleTime;
					SpwanOneNote(noteInfo, isEnemy);
					break;
				case 2:
					SpwanOneNote(noteInfo, isEnemy);
					holdStart = noteInfo;
					break;
				case 3:
					SpwanHoldEndNote(holdStart, noteInfo, isEnemy);
					SpwanHoldMiddleNote(holdStart, noteInfo, isEnemy);
					break;
				default:
					print("NO MATCH NOTE : "
					      + Note["Timing"].ToString() + ", "
					      + Note["Position"].ToString() + ", "
					      + Note["Kinds"].ToString());
					break;
			}
		}
	}

	void SpwanOneNote(NoteInfo noteInfo, bool isEnemy)
	{
		GameObject tmpNote = null;
		
		print("Note 정보 : " + noteInfo.type
		      + " " + noteInfo.width
		      + " " + noteInfo.pos
		      + " " + noteInfo.offset
		      + " " + noteInfo.alpha
		      + " " + noteInfo.doubleTime
		      );
		print(ObjectPool.Instance.NoteQueue[noteInfo.type].Count);
		
		tmpNote = ObjectPool.Instance.NoteQueue[noteInfo.type].Dequeue();
		tmpNote.transform.localPosition =
			notePositionManager.makePosition(
				noteInfo.width, noteInfo.pos + noteInfo.offset + noteInfo.doubleTime);
		tmpNote.transform.localRotation =
			notePositionManager.makeRotation(
				noteInfo.pos + noteInfo.offset + noteInfo.doubleTime);

		print(tmpNote.transform.localPosition);
		print(tmpNote.transform.localRotation);

		if (isEnemy)
		{
			timingManager.EnemyNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (noteInfo.pos + noteInfo.offset) + noteInfo.doubleTime
			);
			timingManager.EnemyNoteQueue.Enqueue(tmpNote);
		}
		else
		{
			timingManager.AllyNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (noteInfo.pos + noteInfo.offset) + noteInfo.doubleTime
			);
			timingManager.AllyNoteQueue.Enqueue(tmpNote);
		}

		tmpNote.SetActive(true);
		StartCoroutine(FadeInNote(tmpNote, noteInfo.alpha));
	}

	void SpwanHoldMiddleNote(NoteInfo holdStartNoteInfo, NoteInfo holdEndNoteInfo, bool isEnemy)
	{
		GameObject tmpNote = null;
		tmpNote = ObjectPool.Instance.NoteQueue[holdEndNoteInfo.type].Dequeue();
		notePositionManager.makeHoldMiddleTransform(
			tmpNote, holdEndNoteInfo.width, holdStartNoteInfo.pos, holdEndNoteInfo.pos, holdEndNoteInfo.offset
		);


		if (isEnemy)
		{
			timingManager.HoldMiddleNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (holdStartNoteInfo.pos + holdStartNoteInfo.offset)
				          + holdStartNoteInfo.doubleTime
			);
			timingManager.HoldMiddleNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (holdEndNoteInfo.pos + holdEndNoteInfo.offset)
				          + holdEndNoteInfo.doubleTime
			);

			timingManager.HoldMiddleNoteQueue.Enqueue(tmpNote);
		}
		else
		{
			timingManager.HoldMiddleNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (holdStartNoteInfo.pos + holdStartNoteInfo.offset)
				          + holdStartNoteInfo.doubleTime
			);
			timingManager.HoldMiddleNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (holdEndNoteInfo.pos + holdEndNoteInfo.offset)
				          + holdEndNoteInfo.doubleTime
			);

			timingManager.HoldMiddleNoteQueue.Enqueue(tmpNote);

			// timingManager.AllyHoldMiddleNoteTimingQueue.Enqueue(
			// 	TotalTime + SecondsPerBit * (holdStartNoteInfo.pos + holdStartNoteInfo.offset)
			// 	          + holdStartNoteInfo.doubleTime
			// );
			//
			// timingManager.AllyHoldMiddleNoteTimingQueue.Enqueue(
			// 	TotalTime + SecondsPerBit * (holdEndNoteInfo.pos + holdEndNoteInfo.offset)
			// 	          + holdEndNoteInfo.doubleTime
			// );

			timingManager.AllyHoldMiddleNoteQueue.Enqueue(tmpNote);
		}


		tmpNote.SetActive(true);
		StartCoroutine(FadeInHoldMiddleNote(tmpNote, holdMiddleNoteIdleAlpha));
	}

	void SpwanHoldEndNote(NoteInfo holdStartNoteInfo, NoteInfo holdEndNoteInfo, bool isEnemy)
	{
		GameObject tmpNote = null;
		tmpNote = ObjectPool.Instance.NoteQueue[holdStartNoteInfo.type].Dequeue();
		tmpNote.transform.localPosition =
			notePositionManager.makePosition(holdEndNoteInfo.width,
				holdEndNoteInfo.pos + holdEndNoteInfo.offset + holdEndNoteInfo.doubleTime);
		tmpNote.transform.localRotation =
			notePositionManager.makeRotation(
				holdEndNoteInfo.pos + holdEndNoteInfo.offset + holdEndNoteInfo.doubleTime);


		if (isEnemy)
		{
			timingManager.EnemyNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (holdEndNoteInfo.pos + holdEndNoteInfo.offset)
			);
			timingManager.EnemyNoteQueue.Enqueue(tmpNote);
		}
		else
		{
			timingManager.AllyHoldNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (holdStartNoteInfo.pos + holdStartNoteInfo.offset)
				          + holdStartNoteInfo.doubleTime
			);

			timingManager.AllyHoldNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (holdEndNoteInfo.pos + holdEndNoteInfo.offset)
				          + holdEndNoteInfo.doubleTime
			);

			timingManager.AllyHoldEndNoteTimingQueue.Enqueue(
				TotalTime + SecondsPerBit * (holdEndNoteInfo.pos + holdEndNoteInfo.offset)
				          + holdEndNoteInfo.doubleTime
			);

			timingManager.AllyHoldNoteQueue.Enqueue(tmpNote);
		}

		tmpNote.SetActive(true);
		StartCoroutine(FadeInNote(tmpNote, holdEndNoteInfo.alpha));
	}

	[SerializeField] private GameObject[] NotePrefabs;

	public void DestroyNotes()
	{
		GameObject[] ClickNotes = GameObject.FindGameObjectsWithTag(NotePrefabs[0].tag);
		GameObject[] DoubleNotes = GameObject.FindGameObjectsWithTag(NotePrefabs[1].tag);
		GameObject[] HoldNotes = GameObject.FindGameObjectsWithTag(NotePrefabs[2].tag);
		GameObject[] HoldMiddleNotes = GameObject.FindGameObjectsWithTag(NotePrefabs[3].tag);

		foreach (var Note in ClickNotes)
		{
			StartCoroutine(FadeOutNote(Note, 0));
		}

		foreach (var Note in DoubleNotes)
		{
			StartCoroutine(FadeOutNote(Note, 1));
		}

		foreach (var Note in HoldNotes)
		{
			StartCoroutine(FadeOutNote(Note, 2));
		}

		foreach (var Note in HoldMiddleNotes)
		{
			StartCoroutine(FadeOutHoldMiddleNote(Note, 3));
		}

		TimingChart.Clear();
		Destroyable = false;
	}


	IEnumerator FadeInNote(GameObject aNote, float alpha)
	{
		SpriteRenderer tmpSR = aNote.GetComponent<SpriteRenderer>();
		Color tmpColor = tmpSR.color;
		while (tmpColor.a < alpha)
		{
			tmpColor.a += Time.deltaTime / FadeTime;
			tmpSR.color = tmpColor;
			yield return null;
		}

		if (tmpColor.a > alpha)
		{
			tmpColor.a = alpha;
		}

		tmpSR.color = tmpColor;
	}

	IEnumerator FadeOutNote(GameObject aNote, int type)
	{
		SpriteRenderer tmpSR = aNote.GetComponent<SpriteRenderer>();
		Color tmpColor = tmpSR.color;
		while (tmpColor.a > 0f)
		{
			tmpColor.a -= Time.deltaTime / FadeTime;
			tmpSR.color = tmpColor;
			yield return null;
		}

		if (tmpColor.a < 0f)
		{
			tmpColor.a = 0f;
		}

		tmpSR.color = tmpColor;
		EnqueueNote(aNote, type);
	}

	IEnumerator FadeInHoldMiddleNote(GameObject holdNoteMiddle, float alpha)
	{
		Image holdNoteImage = holdNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
		Image holdNoteMaskImage = holdNoteMiddle.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();

		holdNoteImage.color = new Color(
			holdNoteImage.color.r, holdNoteImage.color.g, holdNoteImage.color.b, 1f
		);

		Color maskColor = holdNoteMaskImage.color;
		while (maskColor.a < alpha)
		{
			maskColor.a += Time.deltaTime / FadeTime;
			holdNoteMaskImage.color = maskColor;
			yield return null;
		}

		if (maskColor.a > alpha)
		{
			maskColor.a = alpha;
			holdNoteMaskImage.color = maskColor;
		}
	}

	IEnumerator FadeOutHoldMiddleNote(GameObject holdNoteMiddle, int type)
	{
		Image holdNoteImage = holdNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
		Image holdNoteMaskImage = holdNoteMiddle.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Image>();

		holdNoteMaskImage.color = new Color(
			holdNoteMaskImage.color.r, holdNoteMaskImage.color.g, holdNoteMaskImage.color.b, 0.25f
		);

		Color holdColor = holdNoteImage.color;

		while (holdColor.a > 0f)
		{
			holdColor.a -= Time.deltaTime / FadeTime;
			holdNoteImage.color = holdColor;
			yield return null;
		}

		if (holdColor.a < 0f)
		{
			holdColor.a = 0f;
		}


		holdNoteImage.color = holdColor;
		holdNoteImage.fillAmount = 0f;
		EnqueueNote(holdNoteMiddle, type);
	}


	void EnqueueNote(GameObject aNote, int type)
	{
		if (ObjectPool.Instance.NoteQueue.Length <= type)
		{
			print("NO TYPE NOTE QUEUE IS AVAILABLE"
			      + "Note : " + gameObject.ToString());
		}

		ObjectPool.Instance.NoteQueue[type].Enqueue(aNote);
		aNote.SetActive(false);
	}
}