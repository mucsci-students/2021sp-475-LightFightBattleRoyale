using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseSkillScript : MonoBehaviour
{
    [HideInInspector]
    public float abilityLength = 2f;
    [HideInInspector]
    public float cooldown = 20f;
    [HideInInspector]
    public GameObject PhaserIconAnimator;
    public bool isPlayer = false;
    private bool inUse = false;

    private void Start()
    {
        PhaserIconAnimator = GameObject.Find("Phaser");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("SkillButton") && isPlayer == true && inUse == false)
        {
            StartCoroutine(Skill());
        } 
    }

    private IEnumerator Skill()
    {
        inUse = true;
        GetComponent<CollisionScript>().phasing = true;
        PhaserIconAnimator.GetComponent<Animator>().Play("Phaser Active");
        yield return new WaitForSeconds(abilityLength);
        GetComponent<CollisionScript>().phasing = false;
        PhaserIconAnimator.GetComponent<Animator>().Play("Phaser Cooldown");
        yield return new WaitForSeconds(cooldown);
        inUse = false;
    }
}
