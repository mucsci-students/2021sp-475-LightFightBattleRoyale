using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharButtonScript : MonoBehaviour
{
    // Public
    public string currentChar;
    public int currentPlayerCount;
    public GameObject Char1;
    public GameObject Char2;
    public GameObject Char3;
    public GameObject CharModelSpawnPoint;
    public Button char1Button;
    public Button char2Button;
    public Button char3Button;
    public Button fourButton, sixButton, eightButton;
    public Animator panel1Anim;
    public Animator panel2Anim;
    public Animator panel3Anim;

    // Private
    private string pastAnimator;

    public void Start ()
    {
        currentChar = "none";
        StartCoroutine("buttonCoroutine");
    }

    private IEnumerator buttonCoroutine ()
    {
        var waitForButton = new WaitForUIButtons(char1Button, char2Button, char3Button, fourButton, sixButton, eightButton);
        while (true)
        {  
            yield return waitForButton.Reset();
            if(waitForButton.PressedButton == char1Button && currentChar != "Char1")
            {
                figureOutPastAnimator();
                panel1Anim.Play("Unfold Panel");
                pastAnimator = "Panel1";
                if(CharModelSpawnPoint.transform.childCount > 0 && CharModelSpawnPoint.transform.GetChild(0).gameObject != null)
                {
                    Destroy(CharModelSpawnPoint.transform.GetChild(0).gameObject);
                }
                Instantiate(Char1, CharModelSpawnPoint.transform.position, Quaternion.Euler(new Vector3(0, 200, 0)), CharModelSpawnPoint.transform);
                currentChar = "Char1";
            } else if (waitForButton.PressedButton == char2Button && currentChar != "Char2") 
            {
                figureOutPastAnimator();
                panel2Anim.Play("Unfold Panel2");
                pastAnimator = "Panel2";
                if(CharModelSpawnPoint.transform.childCount > 0 && CharModelSpawnPoint.transform.GetChild(0).gameObject != null)
                {
                    Destroy(CharModelSpawnPoint.transform.GetChild(0).gameObject);
                }
                Instantiate(Char2, CharModelSpawnPoint.transform.position, Quaternion.Euler(new Vector3(0, 200, 0)), CharModelSpawnPoint.transform);
                currentChar = "Char2";
            } else if (waitForButton.PressedButton == char3Button&& currentChar != "Char3") 
            {
                figureOutPastAnimator();
                panel3Anim.Play("Unfold Panel3");
                pastAnimator = "Panel3";
                if(CharModelSpawnPoint.transform.childCount > 0 && CharModelSpawnPoint.transform.GetChild(0).gameObject != null)
                {
                    Destroy(CharModelSpawnPoint.transform.GetChild(0).gameObject);
                }
                Instantiate(Char3, CharModelSpawnPoint.transform.position, Quaternion.Euler(new Vector3(0, 200, 0)), CharModelSpawnPoint.transform);
                currentChar = "Char3";
            } else if (waitForButton.PressedButton == fourButton && currentPlayerCount != 4)
            {
                currentPlayerCount = 4;
            } else if (waitForButton.PressedButton == sixButton && currentPlayerCount != 6)
            {
                currentPlayerCount = 6;
            } else if (waitForButton.PressedButton == eightButton && currentPlayerCount != 8)
            {
                currentPlayerCount = 8;
            }
        }   
    }

    private void figureOutPastAnimator()
    {
        if (pastAnimator == null)
        {
            return;
        } else if (pastAnimator == "Panel1")
        {
            panel1Anim.Play("Refold Panel");
        } else if (pastAnimator == "Panel2")
        {
            panel2Anim.Play("Refold Panel 2");
        } else if (pastAnimator == "Panel3")
        {
            panel3Anim.Play("Refold Panel3");
        }
    }
}
