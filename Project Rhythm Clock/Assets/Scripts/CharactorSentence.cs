using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharactorSentence : MonoBehaviour
{
    public string[] sentences; // 대사 들어갈 변수
    public string[] names;
    public string Line;

    public List<Dictionary<string, object>> data_Dialogue { get; set; }

    void Start()
    {
        // Dialogue_isCleared가 false면 그 인덱스의 다이얼로그 활성화

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


        Array.Resize<string>(ref sentences, data_Dialogue.Count); // 배열 크기 재정의
        Array.Resize<string>(ref names, data_Dialogue.Count);

        Talk(data_Dialogue, sentences, names);
    }

    private void OnMouseDown() // 클릭 이벤트 받기
    {
        if (DialogueManager.instance.dialoguegroup.alpha == 0)// 대사 중일 땐 호출되지 않도록
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
