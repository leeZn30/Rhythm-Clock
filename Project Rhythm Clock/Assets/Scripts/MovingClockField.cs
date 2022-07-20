using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingClockField : MonoBehaviour
{
	[SerializeField] private StageSettings settings = null;

	enum MovingKinds
	{
		moving = 1,
		rotate = 2,
		scale = 4,
	}
	private float Totaltime = 0.0f;
	private bool isStart = false;

	private float MoveMultiple;
	private float MoveSpeed = 1.5f;
	private Vector3 MovingVector, TargetVector;
	private Vector3 teleportVector;

	
	private float RotateMultiple;
	private float RotateSpeed = 0f;

	private float ScaleMultiple;
	private Vector3 ChangeScaleVector, TargetScaleVector;

	private Transform ClockField;

	public int movingKind = 0;

	private void Start()
	{
		print(Screen.width);
		print(Screen.height);
		ClockField = transform.GetChild(0);
	}

	void Update()
	{
		if (isStart)
		{
			Totaltime += Time.deltaTime;
			
			if ((movingKind & (int)MovingKinds.moving) != 0)
			{
				transform.Translate(MovingVector * Time.deltaTime);
			}

			if ((movingKind & (int)MovingKinds.rotate) != 0)
			{
				ClockField.Rotate(Vector3.forward, RotateSpeed * Time.deltaTime);
			}
			
			if ((movingKind & (int)MovingKinds.scale) != 0)
			{
				ClockField.localScale += ChangeScaleVector * Time.deltaTime;
			}
		}
	}
	
	public void SetTargetPosition(float targetX, float targetY)
	{
		movingKind ^= (int)MovingKinds.moving;
		// bpm이 올라가면 속도는 증가
		MoveMultiple = settings.BPM / (120 * 4);
		TargetVector = new Vector3(targetX, targetY, 90);
		print(TargetVector);
		MovingVector = MoveMultiple * (TargetVector - transform.position);
		print(MovingVector);
	}

	public void SetRotateSpeed(float RotateSpeed)
	{
		movingKind ^= (int)MovingKinds.rotate;
		// bpm이 올라가면 속도 증가
		RotateMultiple = 90f * settings.BPM / 120f;
		this.RotateSpeed = RotateMultiple * RotateSpeed;
	}
	
	// 현재 Scale에서 이번 사이클 동안 얼마나 늘리고 줄일지
	public void TargetScale(float scaleX, float scaleY)
	{
		movingKind ^= (int)MovingKinds.scale;
		ScaleMultiple = settings.BPM / (120 * 4);
		TargetScaleVector = new Vector3(scaleX, scaleY, 90);
		ChangeScaleVector = ScaleMultiple * (TargetScaleVector - ClockField.localScale);
	}
	
	// 순간이동(좌표 기반)
	public void SetTeleportPosition(float teleportX, float teleportY)
	{
		teleportVector = new Vector3(teleportX, teleportY, 90);
		transform.position = teleportVector;
	}
	
	// 현재 Scale에서 순식간에 수치 변경
	public void SetScale(float scaleX, float scaleY)
	{
		ClockField.localScale = new Vector3(scaleX, scaleY, 1);
	}
	
	
	public void ResetMovingKind()
	{
		movingKind = 0;
	}

	public void TransformReset()
	{
		ResetMovingKind();
		RotateSpeed = 0;
		transform.position = new Vector3(0, 0, 90);
		ClockField.localScale = new Vector3(1, 1, 1);
	}
	
	public void startGame()
	{
		isStart = true;
	}
}