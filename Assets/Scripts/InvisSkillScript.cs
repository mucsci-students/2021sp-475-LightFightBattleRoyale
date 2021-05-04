using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisSkillScript : MonoBehaviour
{
    [HideInInspector]
    public float abilityLength = 5f;
    [HideInInspector]
    public float cooldown = 15f;
    [HideInInspector]
    public GameObject CovertIconAnimator;
    public bool isPlayer = false;
    private bool inUse = false;

    private void Start()
    {
        CovertIconAnimator = GameObject.Find("Covert");
    }

    void Update()
    {
        if (Input.GetButtonDown("SkillButton") && isPlayer == true && inUse == false)
        {
            StartCoroutine(Skill());
        }
    }

    // Invisible skill
    private IEnumerator Skill()
    {
        inUse = true;
        changeRenderState(false);
        GetComponentInChildren<TrailCollision>().isInvis = true;
        CovertIconAnimator.GetComponent<Animator>().Play("Covert Active");
        yield return new WaitForSeconds (abilityLength);
        changeRenderState(true);
        GetComponentInChildren<TrailCollision>().isInvis = false;
        CovertIconAnimator.GetComponent<Animator>().Play("Covert Cooldown");
        yield return new WaitForSeconds(cooldown);
        inUse = false;
    }

    private void changeRenderState(bool state)
    {
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rs)
        {
            r.enabled = state;
        }
    }
}
