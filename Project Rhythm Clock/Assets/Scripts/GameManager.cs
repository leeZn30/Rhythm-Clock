using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            return getInstance();
        }
    }

    private static GameManager getInstance()
    {
        if (instance == null)
        {
            return null;
        }
        return instance;
    }

    [SerializeField] private int stageNum;

    [SerializeField] private Material[] materials;
    public GameObject map = null;
    public Color[] backgroundColors;

    [SerializeField] private GameObject[] characters;
    [SerializeField] private GameObject PlayerObject = null;

    [SerializeField] private TextAsset[] noteCharts;
    public List<Dictionary<string, object>> NoteChart { get; set; }

    [SerializeField] private StageSettings settings = null;

    [SerializeField] private float[] FinishStageTimes;
    public float FinishTime;

    public GameObject StageInfo = null;
    public GameObject StartInfo = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        stageNum = GameData.Instance.selecttedStage;

        if (stageNum != 0)
        {

            NoteChart = CSVReader.Read("NoteChart/" + noteCharts[stageNum].name);

            map.GetComponent<Renderer>().material = materials[stageNum];
            Camera.main.backgroundColor = backgroundColors[stageNum];

            GameObject character = Instantiate(characters[stageNum], new Vector3(-7, 2, -1), Quaternion.identity);
            character.transform.localScale = new Vector3(-3, 3, 1);
            character.transform.SetParent(PlayerObject.transform);

            FinishTime = FinishStageTimes[stageNum];

            StageInfo.SetActive(false);
            
            if (stageNum == 3)
            {
                StartInfo.GetComponentInChildren<Text>().text = "Press S to Start!\n-BOSS STAGE-";
            }

        }
    }


    public void SpawnEnemy()
    {
        GameObject enemy = null;
        
        switch (stageNum)
        {
            case 1:
                enemy = ObjectPool.Instance.RatEnemyQueue.Dequeue();
                break;

            case 2:
                enemy = ObjectPool.Instance.BatEnemyQueue.Dequeue();
                break;

            case 3:
                enemy = ObjectPool.Instance.CrabEnemyQueue.Dequeue();
                break;

            default:
                enemy = ObjectPool.Instance.RatEnemyQueue.Dequeue();
                break;

        }
        enemy.SetActive(true);
        enemy.transform.position = new Vector3(9, 2.5f, -1); // 9, 2, -1
        enemy.GetComponent<Enemy>().target = PlayerObject;
        enemy.GetComponent<Enemy>().moveSpeed = 7.0f / 120.0f * settings.BPM;
    }

    public List<Dictionary<string, object>> getNoteChart()
    {
        return NoteChart;
    }

    public void SuccessStage()
    {
        StageInfo.GetComponentInChildren<Text>().text = "Stage Clear!";
        StageInfo.SetActive(true);
        GameData.Instance.Clear_Stage();
    }

    public void FailStage()
    {
        StageInfo.GetComponentInChildren<Text>().text = "Stage Failed!";
        StageInfo.SetActive(true);
    }

}
