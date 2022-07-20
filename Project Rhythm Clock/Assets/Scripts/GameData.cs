using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    private static GameData instance; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    public static GameData Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    // 저장 정보
    public bool[] StageClearinfo;
    public float syncTime;

    public bool[] DialogueClearinfo;

    // Stage 선택시 넘어갈 데이터들
    public int selecttedStage = -1;

    public bool isCleared(int stagenum)
    {

        if (StageClearinfo[stagenum])
            return true;
        else
            return false;
    }

    public bool Dialogue_isCleared(int dialoguenum)
    {
        if (DialogueClearinfo[dialoguenum])
            return true;
        else
            return false;
    }

    public void SetCurrStageNum(int cur)
    {
        selecttedStage = cur;
    }

    public void Clear_Stage()
    {
        PlayerPrefs.SetInt("stage" + selecttedStage.ToString(), 1); // 클리어 했으면 1 저장
        StageClearinfo[selecttedStage] = true;
    }

    public void Check_DialogueClearinfo(int dialogueindex)
    {
        DialogueClearinfo[dialogueindex] = true;
    }

    public void LoadData()
    {
        syncTime = PlayerPrefs.GetFloat("syncTime");

        int tmpstage;
        for (int i = 0; i < 4; i++)
        {
            tmpstage = PlayerPrefs.GetInt("stage" + i.ToString());
            if (tmpstage == 0) // 잠겨져 있을 때
                StageClearinfo[i] = false;
            else
                StageClearinfo[i] = true;
        }

        int tmpdialogue;
        for (int i = 0; i < 4; i++) // Load Dialogue Data
        {
            tmpdialogue = PlayerPrefs.GetInt("dialogue" + i.ToString());
            if (tmpdialogue == 0) // 잠겨져 있을 때
                DialogueClearinfo[i] = false;
            else
                DialogueClearinfo[i] = true;
        }

    }

    public void resetData()
    {
        PlayerPrefs.DeleteAll();

        LoadData();
    }

    public void saveSyncTime(float synctime)
    {
        PlayerPrefs.SetFloat("syncTime", synctime);
        syncTime = synctime;
    }

    void OnEnable()
    {
        // 씬 매니저의 sceneLoaded에 체인을 건다.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Main Scene")
        {
            // 가장 마지막에 풀어진 stage랑 dialogue 가져오기
            int lastStage = 0;
            for (int i = 0; i < 4; i++)
            {
                if (isCleared(i))
                {
                    lastStage = i;
                }
            }

            int lastDialogue = 0;
            for (int i = 0; i < 5; i++)
            {
                if (Dialogue_isCleared(i))
                {
                    lastDialogue = i;
                }
            }

            Debug.Log(lastStage + " " + lastDialogue);

            // 같다면 DialogueScene으로
            if (lastStage == lastDialogue)
            {
                SceneManager.LoadScene(3);
            }
        }

    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
