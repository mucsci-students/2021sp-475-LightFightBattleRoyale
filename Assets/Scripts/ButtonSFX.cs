using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSFX : MonoBehaviour
{
    public AudioSource UIButton;
    
    public void PlayButtonSFX()
    {
        UIButton.Play();
    }
}
