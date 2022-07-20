using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class DialogueManager : MonoBehaviour
{
    public Text dialogueText;
    public Text nameText;

    public GameObject nextText;
    public CanvasGroup dialoguegroup;

    public Queue<string> sentences; // Queue: 먼저 들어간 데이터가 먼저 나감
    public Queue<string> charctors;

    private string csvSentence;
    private string currentSentence;
    private string currentName;

    public float typingSpeed = 0.5f;
    private bool istyping = false;

    public int DialogueIndex; // 대화 인덱스
    public GameObject[] Dialogues; // 대화 오브젝트들

    private bool isSkip = false;


    public static DialogueManager instance; // 싱글톤 패턴

    private void Awake()
    {
        instance = this; // 초기화
    }

    void Start()
    {
        sentences = new Queue<string>(); // 초기화
        charctors = new Queue<string>();

        SoundManager.Instance.PlayBGM(DialogueIndex);
    }

    public void Ondialogue(string[] lines, string[] names) // 호출될 때마다 큐에 대화를 넣고 대화창에 나오게 하기
    {
        sentences.Clear(); // 혹시 큐에 있을 데이터 비워두기
        charctors.Clear();

        foreach (string line in lines) // 전달받은 인자들을 큐에 차례로 넣어주기
        {
            sentences.Enqueue(line);
        }

        foreach (string line in names) // 전달받은 인자들을 큐에 차례로 넣어주기
        {
            charctors.Enqueue(line);
        }

        dialoguegroup.alpha = 1; // 캔버스 나타남
        dialoguegroup.blocksRaycasts = true; // 마우스 이벤트 감지

        NextSentence();
    }

    public void NextSentence()
    {
        if (sentences.Count != 0)
        {
            currentSentence = sentences.Dequeue(); // Dequeue: 큐메 존재하는 데이터 중 가장 먼저 들어온 데이터를 반환하고 큐에서 해당 데이터를 제거
            currentName = charctors.Dequeue();

            
            istyping = true;
            isSkip = false;
            nextText.SetActive(false);

            // 코루틴            
            StartCoroutine(NTyping(currentName));
            StartCoroutine(STyping(currentSentence)); // 타이핑 효과주기                                    
        }
        else // 대화창 꺼지도록
        {
            dialoguegroup.alpha = 0;
            dialoguegroup.blocksRaycasts = false;

            // 대화가 종료되었다는 사인 보내기
            Clear_Dialogue();
        }
    }

    IEnumerator STyping(string line) // 캐릭터 문장 타이핑 효과주기
    {
        csvSentence = "";
        dialogueText.text = ""; // 빈 문자열로 초기화

        foreach (char letter in line.ToCharArray())
        {
            csvSentence += letter;

            if (letter == '@')
                dialogueText.text += '\n';
            else
                dialogueText.text += letter; // 한 글자씩 더하기

            if (isSkip)
            {                
                yield return new WaitForSeconds(0f);
            }
            else
            {
                yield return new WaitForSeconds(typingSpeed);
            }
        }
    }

    IEnumerator NTyping(string line) // 타이핑 효과주기
    {
        nameText.text = ""; // 빈 문자열로 초기화
        foreach (char letter in line.ToCharArray())
        {
            nameText.text += letter; // 한 글자씩 더하기
            yield return new WaitForSeconds(0f); // 타이핑 속도 조절
        }
    }

    void Update()
    {
        // dialogueText == currentSentence 대사 한 줄의 끝
        if (csvSentence == currentSentence && nameText.text.Equals(currentName)) // 두 개가 같으면 다음 대사로 넘어가도록
        {
            nextText.SetActive(true);
            istyping = false;
        }

        if (Input.GetMouseButtonDown(0) && dialoguegroup.alpha == 1)
        {
            if (!istyping)
            {
                isSkip = false;
                NextSentence();
            }
            else
            {                
                isSkip = true;
            }
        }
    }

    public void Clear_Dialogue()
    {
        PlayerPrefs.SetInt("dialogue" + DialogueIndex.ToString(), 1); // 클리어 했으면 1 저장
        Dialogues[DialogueIndex].SetActive(false); 

        // 다음 씬으로 넘어갔다가 끝나면 다시 돌아오기
        if (DialogueIndex == 0) // tutorial dialogue는 완료하고, stage1 dialogue는 완료하지 않았다면 syncScene
        {
            GameData.Instance.selecttedStage = 0;
            SceneManager.LoadScene(1);
        }
        else if (DialogueIndex == 4)
        {
            SceneManager.LoadScene(5);
        }
        else // 나머지는 mainScene
        {
            SceneManager.LoadScene(2);
        }

        GameData.Instance.Check_DialogueClearinfo(DialogueIndex); // DialogueClearinfo에 체크

        if (DialogueIndex < 4)
        {
            DialogueIndex++;
            Dialogues[DialogueIndex].SetActive(true);
            //Debug.Log(DialogueIndex);
        }
    }

    public void Active_Dialogue(int dialogueindex)
    {
        Dialogues[dialogueindex].SetActive(true);
    }

    public void Unactive_Dialogue(int dialogueindex)
    {
        Dialogues[dialogueindex].SetActive(false);
    }


}
