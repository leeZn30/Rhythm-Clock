using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoldNoteUI : MonoBehaviour
{
	private GameObject goHoldNoteMiddle;
	[SerializeField] private StageSettings stageSettings;
	private float m_fSpeed = 0.0f;

	private float totalTime = 0.0f;
	private float size = 108;

	private bool isStart = false;

	void Start()
	{
		m_fSpeed = 1 / stageSettings.CPS;

		goHoldNoteMiddle = GameObject.Find("Canvas/HoldNote");
		goHoldNoteMiddle.transform.GetChild(0).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta =
			new Vector2((3) * size, (3) * size);
		goHoldNoteMiddle.transform.GetChild(0).transform.GetChild(0).transform.GetChild(0)
				.GetComponent<RectTransform>().sizeDelta =
			new Vector2((4) * size, (4) * size);
		goHoldNoteMiddle.transform.GetChild(1).transform.GetChild(0).GetComponent<RectTransform>().sizeDelta =
			new Vector2((3) * size, (3) * size);
		goHoldNoteMiddle.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0)
				.GetComponent<RectTransform>().sizeDelta =
			new Vector2((4) * size, (4) * size);
	}

	void Update()
	{
		if (isStart)
		{									
			goHoldNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0).    GetComponent<Image>()
				.fillAmount += m_fSpeed * Time.deltaTime;
			
			if (goHoldNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0).    GetComponent<Image>().fillAmount >= 0.125)
			{
				goHoldNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0).    GetComponent<Image>().fillAmount = 0.125f;
			}
			
			goHoldNoteMiddle.transform.GetChild(1).GetChild(0).GetChild(0).    GetComponent<Image>()
				.fillAmount -= m_fSpeed * Time.deltaTime;
		}
	}

	public void StartHoldNote()
	{
		isStart = true;
	}
	
}