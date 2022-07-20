using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBtnInfo : MonoBehaviour
{
    public void loadData()
    {
        GameData.Instance.LoadData();
    }

}
