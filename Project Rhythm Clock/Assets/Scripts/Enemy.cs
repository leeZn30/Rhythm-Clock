using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public Vector3 targetPosition;
    public float moveSpeed; // 120bpm 7f

    public int enemyType;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = (target.transform.position - new Vector3(9, 2, -1)).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= -6f)
        {
            switch (enemyType)
            {
                case 0:
                    ObjectPool.Instance.RatEnemyQueue.Enqueue(gameObject);
                    break;

                case 1:
                    ObjectPool.Instance.BatEnemyQueue.Enqueue(gameObject);
                    break;

                case 2:
                    ObjectPool.Instance.CrabEnemyQueue.Enqueue(gameObject);
                    break;

                default:
                    break;

            }

            gameObject.SetActive(false);
        }
        else
        {
            transform.Translate(targetPosition * Time.deltaTime * moveSpeed);

        }
    }

}
