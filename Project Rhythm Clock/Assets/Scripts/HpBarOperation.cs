using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarOperation : MonoBehaviour
{
    [SerializeField] public float curHp;
    [SerializeField] public float damage;

    [SerializeField] private Slider hpbar;

    public int maxHp = 100;

    [SerializeField] private PlayerController playercontroller;
    
    void Start()
    {
        hpbar.value = (float)curHp / (float)maxHp;
    }

    void Update()
    {
        HandleHp();
        
        if (curHp <= 0)
        {
            Debug.Log("Game Over");
            GameManager.Instance.FailStage();
            playercontroller.gameFinish();
        }
    }

    public void takeDamage()
    {
        curHp -= damage;
    }

    private void HandleHp()
    {
        hpbar.value = Mathf.Lerp(hpbar.value, (float)curHp / (float)maxHp, Time.deltaTime * 10);
    }
}
