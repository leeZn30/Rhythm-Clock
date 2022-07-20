using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class NewGameBtnInfo : MonoBehaviour
{
    public void resetData()
    {
        GameData.Instance.resetData();
    }

}
