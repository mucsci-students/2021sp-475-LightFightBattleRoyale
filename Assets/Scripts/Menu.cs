using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{
    public GameObject HTP_Menu;
    public Animator HTP_Start;

    public void Play()
    {
        GameObject.Find("LevelLoader").GetComponent<LevelLoader>().LoadNextLevel();
    }

    public void HTP()
    {
        HTP_Menu.SetActive(true);
        HTP_Start.Play("HTP_Drop");
    }

    public void HTP_Back()
    {
        HTP_Menu.SetActive(false);
    }

    public void Quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
