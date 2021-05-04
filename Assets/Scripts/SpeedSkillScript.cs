using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedSkillScript : MonoBehaviour
{
    [HideInInspector]
    public float abilityLength = 7f;
    [HideInInspector]
    public float cooldown = 20f;
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;
    [HideInInspector]
    public GameObject SwiftIconAnimator;
    public bool isPlayer = false;
    private bool inUse = false;

    private void Start()
    {
        SwiftIconAnimator = GameObject.Find("Swift");
        controller = GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("SkillButton") && isPlayer == true && inUse == false)
        {
            StartCoroutine(Skill());
        }  
    }

    private IEnumerator Skill ()
    {
        inUse = true;
        changeSpeedTo(7f, 15f);
        SwiftIconAnimator.GetComponent<Animator>().Play("Swift Active");
        yield return new WaitForSeconds (abilityLength);
        changeSpeedTo(5f, 10f);
        SwiftIconAnimator.GetComponent<Animator>().Play("Swift Cooldown");
        yield return new WaitForSeconds(cooldown);
        inUse = false;
    }

    private void changeSpeedTo(float walk, float run)
    {
        controller.m_WalkSpeed = (walk);
        controller.m_RunSpeed = (run);
    }
}
