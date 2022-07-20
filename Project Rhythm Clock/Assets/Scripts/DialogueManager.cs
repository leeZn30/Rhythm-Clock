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

    public Queue<string> sentences; // Queue: ���� �� �����Ͱ� ���� ����
    public Queue<string> charctors;

    private string csvSentence;
    private string currentSentence;
    private string currentName;

    public float typingSpeed = 0.5f;
    private bool istyping = false;

    public int DialogueIndex; // ��ȭ �ε���
    public GameObject[] Dialogues; // ��ȭ ������Ʈ��

    private bool isSkip = false;


    public static DialogueManager instance; // �̱��� ����

    private void Awake()
    {
        instance = this; // �ʱ�ȭ
    }

    void Start()
    {
        sentences = new Queue<string>(); // �ʱ�ȭ
        charctors = new Queue<string>();

        SoundManager.Instance.PlayBGM(DialogueIndex);
    }

    public void Ondialogue(string[] lines, string[] names) // ȣ��� ������ ť�� ��ȭ�� �ְ� ��ȭâ�� ������ �ϱ�
    {
        sentences.Clear(); // Ȥ�� ť�� ���� ������ ����α�
        charctors.Clear();

        foreach (string line in lines) // ���޹��� ���ڵ��� ť�� ���ʷ� �־��ֱ�
        {
            sentences.Enqueue(line);
        }

        foreach (string line in names) // ���޹��� ���ڵ��� ť�� ���ʷ� �־��ֱ�
        {
            charctors.Enqueue(line);
        }

        dialoguegroup.alpha = 1; // ĵ���� ��Ÿ��
        dialoguegroup.blocksRaycasts = true; // ���콺 �̺�Ʈ ����

        NextSentence();
    }

    public void NextSentence()
    {
        if (sentences.Count != 0)
        {
            currentSentence = sentences.Dequeue(); // Dequeue: ť�� �����ϴ� ������ �� ���� ���� ���� �����͸� ��ȯ�ϰ� ť���� �ش� �����͸� ����
            currentName = charctors.Dequeue();

            
            istyping = true;
            isSkip = false;
            nextText.SetActive(false);

            // �ڷ�ƾ            
            StartCoroutine(NTyping(currentName));
            StartCoroutine(STyping(currentSentence)); // Ÿ���� ȿ���ֱ�                                    
        }
        else // ��ȭâ ��������
        {
            dialoguegroup.alpha = 0;
            dialoguegroup.blocksRaycasts = false;

            // ��ȭ�� ����Ǿ��ٴ� ���� ������
            Clear_Dialogue();
        }
    }

    IEnumerator STyping(string line) // ĳ���� ���� Ÿ���� ȿ���ֱ�
    {
        csvSentence = "";
        dialogueText.text = ""; // �� ���ڿ��� �ʱ�ȭ

        foreach (char letter in line.ToCharArray())
        {
            csvSentence += letter;

            if (letter == '@')
                dialogueText.text += '\n';
            else
                dialogueText.text += letter; // �� ���ھ� ���ϱ�

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

    IEnumerator NTyping(string line) // Ÿ���� ȿ���ֱ�
    {
        nameText.text = ""; // �� ���ڿ��� �ʱ�ȭ
        foreach (char letter in line.ToCharArray())
        {
            nameText.text += letter; // �� ���ھ� ���ϱ�
            yield return new WaitForSeconds(0f); // Ÿ���� �ӵ� ����
        }
    }

    void Update()
    {
        // dialogueText == currentSentence ��� �� ���� ��
        if (csvSentence == currentSentence && nameText.text.Equals(currentName)) // �� ���� ������ ���� ���� �Ѿ����
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
        PlayerPrefs.SetInt("dialogue" + DialogueIndex.ToString(), 1); // Ŭ���� ������ 1 ����
        Dialogues[DialogueIndex].SetActive(false); 

        // ���� ������ �Ѿ�ٰ� ������ �ٽ� ���ƿ���
        if (DialogueIndex == 0) // tutorial dialogue�� �Ϸ��ϰ�, stage1 dialogue�� �Ϸ����� �ʾҴٸ� syncScene
        {
            GameData.Instance.selecttedStage = 0;
            SceneManager.LoadScene(1);
        }
        else if (DialogueIndex == 4)
        {
            SceneManager.LoadScene(5);
        }
        else // �������� mainScene
        {
            SceneManager.LoadScene(2);
        }

        GameData.Instance.Check_DialogueClearinfo(DialogueIndex); // DialogueClearinfo�� üũ

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
