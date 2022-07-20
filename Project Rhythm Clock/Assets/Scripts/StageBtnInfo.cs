using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageBtnInfo : MonoBehaviour
{
    public int stageNum;
    public bool isAble;

    void Start()
    {
        BtnUpdate();
    }


    public void setStageNum()
    {
        GameData.Instance.SetCurrStageNum(stageNum);
    }


    public void BtnUpdate()
    {
        if (stageNum == 1)
            return;

        if (GameData.Instance.isCleared(stageNum - 1))
            isAble = true;
        else
            isAble = false;

        if (!isAble)
        {
            GetComponent<Button>();
            GetComponent<Button>().interactable = false;
        }
    }
}
