using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class TimingManager : MonoBehaviour
{
	public GameObject character = null;
	public Animator characterAnim;

	enum Score
	{
		NaN = -1,
		Miss = 0,
		Good = 2,
		Perfect = 5,
	}

	[SerializeField] private GameObject HpBar;


	private List<Dictionary<string, object>> NoteChart;

	[SerializeField] private PlayerController playerController;
	[SerializeField] private StageSettings stageSettings;
	[Header("+-")] [SerializeField] private float PerfectTiming = 0.05f;
	[SerializeField] private float GoodTiming = 0.1f;
	[SerializeField] private float MissTiming = 0.15f;

	private float CorrectAlpha = 1.0f;
	private float GoodAlpha = 0.6f;
	private float MissAlpha = 0.1f;
	private float FadeTime = 0f;

	internal float CurrentTime;
	private float holdStartTime;
	private bool holdStart = false;
	private bool allyHoldStart = false;

	private bool DestroyableNote;
	[ItemCanBeNull] public Queue<float> AllyNoteTimingQueue = new Queue<float>();
	[ItemCanBeNull] public Queue<GameObject> AllyNoteQueue = new Queue<GameObject>();
	[ItemCanBeNull] public Queue<float> AllyHoldNoteTimingQueue = new Queue<float>();
	[ItemCanBeNull] public Queue<float> AllyHoldEndNoteTimingQueue = new Queue<float>();
	[ItemCanBeNull] public Queue<GameObject> AllyHoldNoteQueue = new Queue<GameObject>();
	// [ItemCanBeNull] public Queue<float> AllyHoldMiddleNoteTimingQueue = new Queue<float>();
	[ItemCanBeNull] public Queue<GameObject> AllyHoldMiddleNoteQueue = new Queue<GameObject>();

	[ItemCanBeNull] public Queue<float> EnemyNoteTimingQueue = new Queue<float>();
	[ItemCanBeNull] public Queue<GameObject> EnemyNoteQueue = new Queue<GameObject>();
	[ItemCanBeNull] public Queue<float> EnemyHoldNoteTimingQueue = new Queue<float>();
	[ItemCanBeNull] public Queue<GameObject> EnemyHoldNoteQueue = new Queue<GameObject>();

	[ItemCanBeNull] public Queue<float> HoldMiddleNoteTimingQueue = new Queue<float>();
	[ItemCanBeNull] public Queue<GameObject> HoldMiddleNoteQueue = new Queue<GameObject>();

	void Start()
	{
		FadeTime = stageSettings.CPB / stageSettings.CPS * 0.0625f;

		characterAnim = character.GetComponentInChildren<Animator>();
	}

	void Update()
	{
		DestroyableNote = false;
		CurrentTime = (stageSettings.StartTime <= 0f) ? 0f : Time.time - stageSettings.StartTime;

		if (CurrentTime > 0)
		{
			// click만 안누르는거 미스 작동중
			if ((AllyNoteTimingQueue.Count != 0) && (AllyNoteQueue.Count != 0))
			{
				if (AllyNoteTimingQueue.Peek() + MissTiming <= CurrentTime)
				{
					SpriteRenderer spriteRenderer = AllyNoteQueue.Peek().GetComponent<SpriteRenderer>();
					spriteRenderer.color =
						new Color(spriteRenderer.color.r, spriteRenderer.color.g,
							spriteRenderer.color.b, MissAlpha);

					spawnLogo(2);


					HpBar.GetComponent<HpBarOperation>().takeDamage();
					Debug.Log(HpBar.GetComponent<HpBarOperation>().curHp);


					SoundManager.Instance.PlaygameSFX(2);
					characterAnim.SetTrigger("Debuff");

					// hold노트를 안눌러서 미스 처리되면 holdMiddleAlpha변경 + holdEnd 아에 판정에도 안들어가게 처리
					if (AllyNoteQueue.Peek().CompareTag("HoldNote"))
					{
						if (/*(AllyHoldMiddleNoteTimingQueue.Count != 0) &&*/ (AllyHoldMiddleNoteQueue.Count != 0))
						{
							GameObject holdNoteMiddle = AllyHoldMiddleNoteQueue.Peek();
							Transform holdNoteImage = holdNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0);
							Image tmpImage = holdNoteImage.GetComponent<Image>();
							tmpImage.color = new Color(tmpImage.color.r, tmpImage.color.g, tmpImage.color.b, MissAlpha);
							
							// AllyHoldMiddleNoteTimingQueue.Dequeue();
							// AllyHoldMiddleNoteTimingQueue.Dequeue(); 
							AllyHoldEndNoteTimingQueue.Dequeue();
							AllyHoldMiddleNoteQueue.Dequeue();
						}

						if ((AllyHoldNoteTimingQueue.Count != 0) && (AllyHoldNoteQueue.Count != 0))
						{
							spriteRenderer = AllyHoldNoteQueue.Peek().GetComponent<SpriteRenderer>();
							spriteRenderer.color =
								new Color(spriteRenderer.color.r, spriteRenderer.color.g,
									spriteRenderer.color.b, MissAlpha);
							AllyHoldNoteTimingQueue.Dequeue();
							AllyHoldNoteTimingQueue.Dequeue();
							AllyHoldNoteQueue.Dequeue();
						}
					}

					AllyNoteQueue.Dequeue();
					AllyNoteTimingQueue.Dequeue();
				}
			}
			
			// holdMiddle부분 시계 따라서 진행
			if ((HoldMiddleNoteTimingQueue.Count != 0) && (HoldMiddleNoteQueue.Count != 0))
			{
				if (!holdStart && HoldMiddleNoteTimingQueue.Peek() <= CurrentTime)
				{
					holdStartTime = HoldMiddleNoteTimingQueue.Dequeue();
					holdStart = true;
				}

				// 시작했고, end시간보다 작을경우에는 fillAmount변경해야함
				if (holdStart && HoldMiddleNoteTimingQueue.Peek() >= CurrentTime)
				{
					GameObject holdNoteMiddle = HoldMiddleNoteQueue.Peek();
					Transform holdNoteImage = holdNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0);
					Transform holdNoteMaskImage = holdNoteMiddle.transform.GetChild(1).GetChild(0).GetChild(0);
					float fillAmountSpeed = 1 / stageSettings.CPS;
					
					holdNoteImage.GetComponent<Image>().fillAmount += fillAmountSpeed * Time.deltaTime;
					if (holdNoteImage.GetComponent<Image>().fillAmount >=
					    (HoldMiddleNoteTimingQueue.Peek() - holdStartTime) * fillAmountSpeed)
					{
						holdNoteImage.GetComponent<Image>().fillAmount =
							(HoldMiddleNoteTimingQueue.Peek() - holdStartTime) * fillAmountSpeed;
					}

					holdNoteMaskImage.GetComponent<Image>().fillAmount -= fillAmountSpeed * Time.deltaTime;
				}

				// 시작했는데 end시간보다 커지면 종료
				if (holdStart && HoldMiddleNoteTimingQueue.Peek() < CurrentTime)
				{
					HoldMiddleNoteQueue.Dequeue();
					HoldMiddleNoteTimingQueue.Dequeue();
					holdStart = false;
				}
			}


			// 적 효과음 
			if ((EnemyNoteTimingQueue.Count != 0) && (EnemyNoteQueue.Count != 0))
			{
				if (EnemyNoteTimingQueue.Peek() <= CurrentTime)
				{
					SpriteRenderer spriteRenderer = EnemyNoteQueue.Peek().GetComponent<SpriteRenderer>();
					spriteRenderer.color =
						new Color(spriteRenderer.color.r, spriteRenderer.color.g,
							spriteRenderer.color.b, CorrectAlpha);

					SoundManager.Instance.PlaygameSFX(3);

					if(!EnemyNoteQueue.Peek().CompareTag("HoldNote"))
                    {
	                    GameManager.Instance.SpawnEnemy();
                    }
					EnemyNoteQueue.Dequeue();
					EnemyNoteTimingQueue.Dequeue();
				}
			}

		}
	}

	private void spawnLogo(int logotype)
	{
		GameObject aLogo = ObjectPool.Instance.LogoQueue.Dequeue();
		aLogo.GetComponent<Logo>().getSprite(logotype);
		aLogo.SetActive(true);
		aLogo.transform.position = new Vector3(0, 1.5f, 0);
	}

	public int CheckClickTiming()
	{
		int score = (int)Score.NaN;

		DestroyableNote = false;
		if (AllyNoteTimingQueue.Count != 0)
		{
			if (AllyNoteTimingQueue.Peek() - PerfectTiming <= CurrentTime &&
			    CurrentTime <= AllyNoteTimingQueue.Peek() + PerfectTiming)
			{
				SpriteRenderer spriteRenderer = AllyNoteQueue.Peek().GetComponent<SpriteRenderer>();
				spriteRenderer.color =
					new Color(spriteRenderer.color.r, spriteRenderer.color.g,
						spriteRenderer.color.b, CorrectAlpha);
				DestroyableNote = true;
				score = (int)Score.Perfect;

				spawnLogo(0);

				SoundManager.Instance.PlaygameSFX(0);
				if (AllyNoteQueue.Peek().CompareTag("HoldNote"))
				{
					playerController.isHold = true;
				}
                else
				{
					characterAnim.SetTrigger("Attack");
				}
			}

			else if (AllyNoteTimingQueue.Peek() - GoodTiming <= CurrentTime &&
			         CurrentTime <= AllyNoteTimingQueue.Peek() + GoodTiming)
			{
				SpriteRenderer spriteRenderer = AllyNoteQueue.Peek().GetComponent<SpriteRenderer>();
				spriteRenderer.color =
					new Color(spriteRenderer.color.r, spriteRenderer.color.g,
						spriteRenderer.color.b, GoodAlpha);
				DestroyableNote = true;
				score = (int)Score.Good;

				spawnLogo(1);

				SoundManager.Instance.PlaygameSFX(1);
				
				if (AllyNoteQueue.Peek().CompareTag("HoldNote"))
				{
					playerController.isHold = true;
				}
				else
				{
					characterAnim.SetTrigger("Attack");
				}
			}

			else if (AllyNoteTimingQueue.Peek() - MissTiming <= CurrentTime &&
			         CurrentTime <= AllyNoteTimingQueue.Peek() + MissTiming)
			{
				SpriteRenderer spriteRenderer = AllyNoteQueue.Peek().GetComponent<SpriteRenderer>();
				spriteRenderer.color =
					new Color(spriteRenderer.color.r, spriteRenderer.color.g,
						spriteRenderer.color.b, MissAlpha);
				DestroyableNote = true;
				score = (int)Score.Miss;

				HpBar.GetComponent<HpBarOperation>().takeDamage();
				Debug.Log(HpBar.GetComponent<HpBarOperation>().curHp);

				spawnLogo(2);

				SoundManager.Instance.PlaygameSFX(2);
				characterAnim.SetTrigger("Debuff");

				// 만약 holdStart의 경우 holdMiddle부분 alpha 변경 + holdEnd 판정에도 없게 처리
				if (AllyNoteQueue.Peek().CompareTag("HoldNote"))
				{
					if (/*(AllyHoldMiddleNoteTimingQueue.Count != 0) && */(AllyHoldMiddleNoteQueue.Count != 0))
					{
						GameObject holdNoteMiddle = AllyHoldMiddleNoteQueue.Peek();
						Transform holdNoteImage = holdNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0);
						Image tmpImage = holdNoteImage.GetComponent<Image>();
						tmpImage.color = new Color(tmpImage.color.r, tmpImage.color.g, tmpImage.color.b, MissAlpha);
					}

					if ((AllyHoldNoteTimingQueue.Count != 0) && (AllyHoldNoteQueue.Count != 0))
					{
						spriteRenderer = AllyHoldNoteQueue.Peek().GetComponent<SpriteRenderer>();
						spriteRenderer.color =
							new Color(spriteRenderer.color.r, spriteRenderer.color.g,
								spriteRenderer.color.b, MissAlpha);
						AllyHoldNoteTimingQueue.Dequeue();
						AllyHoldNoteTimingQueue.Dequeue();
						AllyHoldNoteQueue.Dequeue();
					}
				}
			}
		}

		if (DestroyableNote && (AllyNoteQueue.Count != 0) && (AllyNoteTimingQueue.Count != 0))
		{
			AllyNoteQueue.Dequeue();
			AllyNoteTimingQueue.Dequeue();
		}

		return score;
	}


	public int CheckHoldEndTiming()
	{
		int score = (int)Score.NaN;
		DestroyableNote = false;
		
		print(AllyHoldNoteTimingQueue.Count);
		print(AllyHoldNoteQueue.Count);
		
		if ((AllyHoldNoteTimingQueue.Count != 0) && (AllyHoldNoteQueue.Count != 0))
		{
			if (CurrentTime > AllyHoldNoteTimingQueue.Peek())
			{
				float holdStart = AllyHoldNoteTimingQueue.Dequeue();

				if (AllyHoldNoteTimingQueue.Peek() - PerfectTiming <= CurrentTime &&
				    CurrentTime <= AllyHoldNoteTimingQueue.Peek() + PerfectTiming)
				{
					SpriteRenderer spriteRenderer = AllyHoldNoteQueue.Peek().GetComponent<SpriteRenderer>();
					spriteRenderer.color =
						new Color(spriteRenderer.color.r, spriteRenderer.color.g,
							spriteRenderer.color.b, CorrectAlpha);
					DestroyableNote = true;
					score = (int)Score.Perfect;

					spawnLogo(0);

					SoundManager.Instance.PlaygameSFX(0);

					// AllyHoldMiddleNoteTimingQueue.Dequeue();
					AllyHoldMiddleNoteQueue.Dequeue();
				}

				else if (AllyHoldNoteTimingQueue.Peek() - GoodTiming <= CurrentTime &&
				         CurrentTime <= AllyHoldNoteTimingQueue.Peek() + GoodTiming)
				{
					SpriteRenderer spriteRenderer = AllyHoldNoteQueue.Peek().GetComponent<SpriteRenderer>();
					spriteRenderer.color =
						new Color(spriteRenderer.color.r, spriteRenderer.color.g,
							spriteRenderer.color.b, GoodAlpha);
					DestroyableNote = true;
					score = (int)Score.Good;

					spawnLogo(1);
					
					SoundManager.Instance.PlaygameSFX(1);

					// HoldMiddleNote 부분이 하나라도 있고 holdEnd 판정이 났을 경우에 그 판정이 Good이면 alpha를 변경
					if (/*(AllyHoldMiddleNoteTimingQueue.Count != 0) &&*/ (AllyHoldMiddleNoteQueue.Count != 0))
					{
						GameObject holdNoteMiddle = AllyHoldMiddleNoteQueue.Peek();
						Transform holdNoteImage = holdNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0);
						Image tmpImage = holdNoteImage.GetComponent<Image>();
						tmpImage.color = new Color(tmpImage.color.r, tmpImage.color.g, tmpImage.color.b, GoodAlpha);

						// AllyHoldMiddleNoteTimingQueue.Dequeue();
						AllyHoldMiddleNoteQueue.Dequeue();

						// print(AllyHoldMiddleNoteTimingQueue.Count);
						print(AllyHoldMiddleNoteQueue.Count);
					}
				}

				// holdStart ~ holdEnd 사이에 떼면 무조건 미스
				else if (holdStart <= CurrentTime)
				{
					SpriteRenderer spriteRenderer = AllyHoldNoteQueue.Peek().GetComponent<SpriteRenderer>();
					spriteRenderer.color =
						new Color(spriteRenderer.color.r, spriteRenderer.color.g,
							spriteRenderer.color.b, MissAlpha);

					DestroyableNote = true;
					score = (int)Score.Miss;
					HpBar.GetComponent<HpBarOperation>().takeDamage();
					Debug.Log(HpBar.GetComponent<HpBarOperation>().curHp);

					spawnLogo(2);

					SoundManager.Instance.PlaygameSFX(2);
					characterAnim.SetTrigger("Debuff");

					// HoldMiddleNote 부분이 하나라도 있고 holdEnd 판정이 났을 경우에 그 판정이 Miss이면 alpha를 변경
					if (/*(AllyHoldMiddleNoteTimingQueue.Count != 0) &&*/ (AllyHoldMiddleNoteQueue.Count != 0))
					{
						GameObject holdNoteMiddle = AllyHoldMiddleNoteQueue.Peek();
						Transform holdNoteImage = holdNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0);
						Image tmpImage = holdNoteImage.GetComponent<Image>();
						tmpImage.color = new Color(tmpImage.color.r, tmpImage.color.g, tmpImage.color.b, MissAlpha);

						
						// AllyHoldMiddleNoteTimingQueue.Dequeue();
						AllyHoldMiddleNoteQueue.Dequeue();

						// print(AllyHoldMiddleNoteTimingQueue.Count);
						print(AllyHoldMiddleNoteQueue.Count);
					}
				}
				else
				{
					print("불리면 안되는데?");
				}
			}
			else
			{
				print("범위 아님");
			}
		}

		if (DestroyableNote && (AllyHoldNoteQueue.Count != 0) && (AllyHoldNoteTimingQueue.Count != 0))
		{
			AllyHoldNoteQueue.Dequeue();
			AllyHoldNoteTimingQueue.Dequeue();
			AllyHoldEndNoteTimingQueue.Dequeue();
		}

		return score;
	}

	public bool isOverHoldTime()
	{
		if (CurrentTime > AllyHoldEndNoteTimingQueue.Peek() + MissTiming)
		{
			print(CurrentTime);
			print(AllyHoldEndNoteTimingQueue.Peek() + MissTiming);
			
			AllyHoldEndNoteTimingQueue.Dequeue();
			
			print("시간 초과");
			
			return true;
		}
		else
		{
			return false;
		}
	}

	IEnumerator FadeCorrectNote(GameObject aNote, float alpha)
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
			tmpColor.a = alpha;
		tmpSR.color = tmpColor;
	}

	IEnumerator FadeFailedNote(GameObject aNote, float alpha)
	{
		SpriteRenderer tmpSR = aNote.GetComponent<SpriteRenderer>();
		Color tmpColor = tmpSR.color;
		while (tmpColor.a > alpha)
		{
			tmpColor.a -= Time.deltaTime / FadeTime;
			tmpSR.color = tmpColor;
			yield return null;
		}

		if (tmpColor.a < alpha)
			tmpColor.a = alpha;
		tmpSR.color = tmpColor;
	}
}