using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SelectButton : MonoBehaviour
{
    public GameObject buttonGameObject;
    public Image textFlash;
    public TextMeshProUGUI text;

    public void Select()
    {
        // Maybe should asyncronously load the game here
        if(buttonGameObject.GetComponent<CharButtonScript>().currentChar == "none")
        {
            PopUpSystem pop = GameObject.Find("Pop Up Box").GetComponent<PopUpSystem>();
            pop.PopUp("Select A Character");
        } 
        else {
            if (buttonGameObject.GetComponent<CharButtonScript>().currentChar == "Char1")
            {
                PlayerPrefs.SetString("selectedChar", "char1");
                DetermineNumberOfPlayers();
            } 
            else if (buttonGameObject.GetComponent<CharButtonScript>().currentChar == "Char2")
            {
                PlayerPrefs.SetString("selectedChar", "char2");
                DetermineNumberOfPlayers();
            } 
            else if (buttonGameObject.GetComponent<CharButtonScript>().currentChar == "Char3")
            {
                PlayerPrefs.SetString("selectedChar", "char3");
                DetermineNumberOfPlayers();
            } 
            else 
            {
                print("error starting coroutine");
            }
            GameObject.Find("LevelLoader").GetComponent<LevelLoader>().LoadNextLevel(); 
        }      
    }

    private void DetermineNumberOfPlayers()
    {
        if (buttonGameObject.GetComponent<CharButtonScript>().currentPlayerCount == 4)
        {
            PlayerPrefs.SetInt("selectedPlayerCount", 4);
        } else if (buttonGameObject.GetComponent<CharButtonScript>().currentPlayerCount == 6)
        {
            PlayerPrefs.SetInt("selectedPlayerCount", 6);
        } else if (buttonGameObject.GetComponent<CharButtonScript>().currentPlayerCount == 8)
        {
            PlayerPrefs.SetInt("selectedPlayerCount", 8);
        }
    }     
}
