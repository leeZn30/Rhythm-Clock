using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NotePositionManager : MonoBehaviour
{
	[SerializeField] private StageSettings stageSettings;

	private int CPB = 0;
	private float HoldNoteMiddleSize = 2f;

	void Start()
	{
		CPB = stageSettings.CPB;
	}

	public Vector3 makePosition(float width, float position)
	{
		return new Vector3(
			(float)(width * 0.5 * Math.Sin(position / CPB * 2 * Math.PI)),
			(float)(width * 0.5 * Math.Cos(position / CPB * 2 * Math.PI)),
			0f);
	}

	public Quaternion makeRotation(float position)
	{
		return Quaternion.Euler(0, 0, (float)(-(position / CPB) * 360));
	}

	public void makeHoldMiddleTransform(GameObject holdNoteMiddle, float width, float startPos, float endPos,
		int offset)
	{
		Transform holdNoteInvisible = holdNoteMiddle.transform.GetChild(0).GetChild(0);
		Transform holdNoteImage = holdNoteMiddle.transform.GetChild(0).GetChild(0).GetChild(0);
		Transform holdNoteMaskInvisible = holdNoteMiddle.transform.GetChild(1).GetChild(0);
		Transform holdNoteMaskImage = holdNoteMiddle.transform.GetChild(1).GetChild(0).GetChild(0);

		holdNoteInvisible.GetComponent<RectTransform>().sizeDelta =
			new Vector2(0.5f * width * HoldNoteMiddleSize, 0.5f * width * HoldNoteMiddleSize);
		holdNoteImage.GetComponent<RectTransform>().sizeDelta =
			new Vector2(0.5f * (width + 1) * HoldNoteMiddleSize, 0.5f * (width + 1) * HoldNoteMiddleSize);
		holdNoteMaskInvisible.GetComponent<RectTransform>().sizeDelta =
			new Vector2(0.5f * (width) * HoldNoteMiddleSize, 0.5f * (width) * HoldNoteMiddleSize);
		holdNoteMaskImage.GetComponent<RectTransform>().sizeDelta =
			new Vector2(0.5f * (width + 1) * HoldNoteMiddleSize, 0.5f * (width + 1) * HoldNoteMiddleSize);

		holdNoteImage.GetComponent<RectTransform>().localRotation =
			Quaternion.Euler(0, 0, -(startPos + offset) * 45);
		holdNoteMaskImage.GetComponent<RectTransform>().localRotation =
			Quaternion.Euler(0, 0, -(endPos + offset) * 45);

		// 
		float length = endPos - startPos;
		holdNoteMaskImage.GetComponent<UnityEngine.UI.Image>().fillAmount = length * 0.125f;
	}
}