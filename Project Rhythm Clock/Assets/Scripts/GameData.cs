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

    // ���� ����
    public bool[] StageClearinfo;
    public float syncTime;

    public bool[] DialogueClearinfo;

    // Stage ���ý� �Ѿ �����͵�
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
        PlayerPrefs.SetInt("stage" + selecttedStage.ToString(), 1); // Ŭ���� ������ 1 ����
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
            if (tmpstage == 0) // ����� ���� ��
                StageClearinfo[i] = false;
            else
                StageClearinfo[i] = true;
        }

        int tmpdialogue;
        for (int i = 0; i < 4; i++) // Load Dialogue Data
        {
            tmpdialogue = PlayerPrefs.GetInt("dialogue" + i.ToString());
            if (tmpdialogue == 0) // ����� ���� ��
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
        // �� �Ŵ����� sceneLoaded�� ü���� �Ǵ�.
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ü���� �ɾ �� �Լ��� �� ������ ȣ��ȴ�.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Main Scene")
        {
            // ���� �������� Ǯ���� stage�� dialogue ��������
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

            // ���ٸ� DialogueScene����
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
