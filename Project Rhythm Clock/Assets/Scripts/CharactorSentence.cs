using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharactorSentence : MonoBehaviour
{
    public string[] sentences; // ��� �� ����
    public string[] names;
    public string Line;

    public List<Dictionary<string, object>> data_Dialogue { get; set; }

    void Start()
    {
        // Dialogue_isCleared�� false�� �� �ε����� ���̾�α� Ȱ��ȭ

        for(int i = 0; i < 5; i++)
        {
            if (!GameData.Instance.Dialogue_isCleared(i))
            {
                DialogueManager.instance.Active_Dialogue(i);
                break;
            }
            else
                DialogueManager.instance.Unactive_Dialogue(i);
        }

        data_Dialogue = CSVReader.Read("Dialogue/" + Line);


        Array.Resize<string>(ref sentences, data_Dialogue.Count); // �迭 ũ�� ������
        Array.Resize<string>(ref names, data_Dialogue.Count);

        Talk(data_Dialogue, sentences, names);
    }

    private void OnMouseDown() // Ŭ�� �̺�Ʈ �ޱ�
    {
        if (DialogueManager.instance.dialoguegroup.alpha == 0)// ��� ���� �� ȣ����� �ʵ���
        {
            DialogueManager.instance.Ondialogue(sentences, names);
        }
    }

    public void Talk(List<Dictionary<string, object>> data_Dialogue, string[] sentences, string[] names)
    {
        for(int i = 0; i < data_Dialogue.Count; i++)
        {
            sentences[i] = data_Dialogue[i]["Line"].ToString();
            names[i] = data_Dialogue[i]["Character_name"].ToString();
        }
    }
}
