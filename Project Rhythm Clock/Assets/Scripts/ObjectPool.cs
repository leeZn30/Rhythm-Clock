using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class ObjectInfo
{
	public GameObject goPrefab;
	public int count;
	public Transform tfPoolParent;
}

public class ObjectPool : MonoBehaviour
{
	[SerializeField] private ObjectInfo[] objectInfos = null;
	public static ObjectPool Instance { get; set; }
	public Queue<GameObject>[] NoteQueue = new Queue<GameObject>[4];

	[CanBeNull] public Queue<GameObject> LogoQueue = new Queue<GameObject>();
	[CanBeNull] public Queue<GameObject> RatEnemyQueue = new Queue<GameObject>();
	[CanBeNull] public Queue<GameObject> BatEnemyQueue = new Queue<GameObject>();
	[CanBeNull] public Queue<GameObject> CrabEnemyQueue = new Queue<GameObject>();
	[CanBeNull] public Queue<GameObject> TempoInfoQueue = new Queue<GameObject>();


	void Start()
	{
		Instance = this;

		NoteQueue[0] = InsertQueue(objectInfos[0]);
		NoteQueue[1] = InsertQueue(objectInfos[1]);
		NoteQueue[2] = InsertQueue(objectInfos[2]);
		NoteQueue[3] = InsertHoldNoteQueue(objectInfos[3]);

		LogoQueue = InsertQueue(objectInfos[4]);
		RatEnemyQueue = InsertColoredQueue(objectInfos[5]);
		BatEnemyQueue = InsertColoredQueue(objectInfos[6]);
		CrabEnemyQueue = InsertColoredQueue(objectInfos[7]);
		TempoInfoQueue = InsertQueue(objectInfos[8]);
	}

	Queue<GameObject> InsertQueue(ObjectInfo objectInfo)
	{
		Queue<GameObject> tmpQueue = new Queue<GameObject>();
		for (int i = 0; i < objectInfo.count; i++)
		{
			GameObject tmpClone = Instantiate(
				objectInfo.goPrefab,
				objectInfo.tfPoolParent.transform.position,
				Quaternion.identity,
				objectInfo.tfPoolParent
			);
			Color tmpColor = tmpClone.GetComponent<SpriteRenderer>().color;
			tmpClone.GetComponent<SpriteRenderer>().color
				= new Color(tmpColor.r, tmpColor.g, tmpColor.b, 0);
			tmpClone.SetActive(false);
			tmpQueue.Enqueue(tmpClone);
		}

		return tmpQueue;
	}

	Queue<GameObject> InsertHoldNoteQueue(ObjectInfo objectInfo)
	{
		Queue<GameObject> tmpQueue = new Queue<GameObject>();
		for (int i = 0; i < objectInfo.count; i++)
		{
			GameObject tmpClone = Instantiate(
				objectInfo.goPrefab,
				objectInfo.tfPoolParent.transform.position,
				Quaternion.identity,
				objectInfo.tfPoolParent
			);
			tmpClone.SetActive(false);
			tmpQueue.Enqueue(tmpClone);
		}

		return tmpQueue;
	}


	Queue<GameObject> InsertColoredQueue(ObjectInfo objectInfo)
	{
		Queue<GameObject> tmpQueue = new Queue<GameObject>();
		for (int i = 0; i < objectInfo.count; i++)
		{
			GameObject tmpClone = Instantiate(
				objectInfo.goPrefab,
				transform.position,
				Quaternion.identity,
				objectInfo.tfPoolParent
			);

			tmpClone.SetActive(false);
			tmpQueue.Enqueue(tmpClone);
		}

		return tmpQueue;
	}
}