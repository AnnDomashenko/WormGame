using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public void ExitGame()
    {
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

}
