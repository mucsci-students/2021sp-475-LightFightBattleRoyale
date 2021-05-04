using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private bool disabled = false;
    public string type;

    Animator anim;
    private CharacterController m_CharacterController;
    private float moveSpeed = 5f;
    private float gravityMultiplier = 4f;

    // Variables for movement
    private GameObject closestEnemy;
    private float rotateAngle;
    private float prevDistanceToContact;
    private float distanceToContact;
    private float rotateDirection;
    private float prevRotateDirection;
    private bool wall = false;
    private bool inUse = false;
    public bool trailIsDetected = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        m_CharacterController = GetComponent<CharacterController>();
        prevRotateDirection = 1f;

        // Get rotate angle
        rotateAngle = transform.eulerAngles.y;

        if (type == "Swift")
        {
            InvokeRepeating("SpeedSkill", 0, Random.Range(GetComponent<SpeedSkillScript>().cooldown + GetComponent<SpeedSkillScript>().abilityLength, 45f));
        } else if (type == "Covert")
        {
            InvokeRepeating("InvisSkill", 0, Random.Range(GetComponent<InvisSkillScript>().cooldown + GetComponent<InvisSkillScript>().abilityLength, 45f));
        } else if (type == "Phaser")
        {
            InvokeRepeating("PhaseSkill", 0, 1f);
        }

        // Change the direction every every 2 seconds after 10 seconds
        InvokeRepeating("ChangeDirection", 10.0f, 1.0f);
    }

    private void SpeedSkill()
    {
        StartCoroutine(Speed());
    }

    private void InvisSkill()
    {
        StartCoroutine(Invis());
    }

    private void PhaseSkill()
    {
        if (inUse == false && trailIsDetected == true)
        {
            StartCoroutine(Phase());
        }
    }

    private IEnumerator Speed()
    {
        print(this + " is zoomin.");
        moveSpeed = 7f;
        setSpeed();
        yield return new WaitForSeconds (GetComponent<SpeedSkillScript>().abilityLength);
        moveSpeed = 3f;
        setSpeed();
        yield return new WaitForSeconds(GetComponent<SpeedSkillScript>().cooldown);
    }

    private IEnumerator Invis()
    {
        print(this + " is invisible.");
        changeRenderState(false);
        GetComponentInChildren<TrailCollision>().isInvis = true;
        yield return new WaitForSeconds (GetComponent<InvisSkillScript>().abilityLength);
        changeRenderState(true);
        GetComponentInChildren<TrailCollision>().isInvis = false;
        yield return new WaitForSeconds(GetComponent<InvisSkillScript>().cooldown);
    }

    private void changeRenderState(bool state)
    {
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        foreach(Renderer r in rs)
        {
            r.enabled = state;
        }
    }

    private IEnumerator Phase()
    {
        print(this + " is phasing.");
        inUse = true;
        GetComponent<CollisionScript>().phasing = true;
        yield return new WaitForSeconds(GetComponent<PhaseSkillScript>().abilityLength);
        GetComponent<CollisionScript>().phasing = false;
        yield return new WaitForSeconds(GetComponent<PhaseSkillScript>().cooldown);
        inUse = false;
    }

    private void setSpeed()
    {
        // Move character
        anim.SetFloat("Speed", 1);
        // Add gravity
        Vector3 m_MoveDir = transform.forward;
        m_MoveDir += Physics.gravity * Time.fixedDeltaTime * gravityMultiplier;
        // Multiply by transform.forward so that the character is always moving forward
        m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime*moveSpeed);
    }

    void FixedUpdate()
    {
        // For debugging
        if(disabled)
            return;

        MovementControl();
        setSpeed();
    }

    private void MovementControl() {
        //print("wall: " + wall + ", trailIsDetected: " + trailIsDetected + ", phasing: " + GetComponent<CollisionScript>().phasing);
        if(wall || (trailIsDetected && !GetComponent<CollisionScript>().phasing)) {
            // Rotate opposite direction until away from wall/trail
            float rotateSpeed = 80f;

            //print("prevDistance: " + prevDistanceToContact + ", currDistance: " + distanceToContact);
            rotateDirection = prevRotateDirection;
            if(distanceToContact < prevDistanceToContact)
                rotateDirection = -prevRotateDirection;
            prevRotateDirection = rotateDirection;

            float rotateBy = rotateDirection * rotateSpeed * Time.fixedDeltaTime;
            //print("avoiding and rotating by " + rotateBy);
            transform.Rotate(0, rotateBy, 0);
            prevDistanceToContact = distanceToContact;
        }
        else if(closestEnemy != null) {
            // Get position in front of enemy
            Vector3 enemyFront = closestEnemy.transform.position + (closestEnemy.transform.forward * 3);
            // Rotate character to look at the target position to begin moving in that direction
            transform.LookAt(enemyFront);
            // Reset euler angles so that the character isn't rotated upwards or downwards
            transform.eulerAngles = new Vector3(
                0,
                transform.eulerAngles.y,
                transform.eulerAngles.z
            );
        }
        // If character does not have an enemy, randomly move
        else {
            // Smooth rotation
            float rotateSpeed = 20f;
            float lookDirection = Mathf.Clamp(rotateAngle - transform.eulerAngles.y, -1.0f, 1.0f);
            transform.Rotate(0, lookDirection * rotateSpeed * Time.fixedDeltaTime, 0);
        }

    } 

    // Rotate the character within a 80 degrees of where the character is already looking
    private void ChangeDirection() {
        if(!wall && (!trailIsDetected || GetComponent<CollisionScript>().phasing)) {
            rotateAngle = transform.eulerAngles.y;
            rotateAngle += GenerateAngleInRange(-80.0f, 80.0f);
        }
    }

    private float GenerateAngleInRange(float min, float max) {
        return Random.Range(min, max);
    }

    // Function to inform AI Controller that another character was found
    public void CharacterUpdate(GameObject character) {
        if(character == null) {
            closestEnemy = null;
            return;
        }
        else if(closestEnemy == null) {
            closestEnemy = character;
            return;
        }
        else {
            // Determine whether the previous closest enemy is closer than the one sent
            float prevDistance = Vector3.Distance(transform.position, closestEnemy.transform.position);
            float newDistance = Vector3.Distance(transform.position, character.transform.position);
            if (newDistance < prevDistance) {
                closestEnemy = character;
            }
        }
    }

    // Function to notify controller that a trail has been spotted
    public void NotifyTrail(bool detected, float distance) {
        trailIsDetected = detected;
        distanceToContact = distance;
    }

    // Function to notify controller that character is about to hit a wall
    public void NotifyWall(bool col, float distance) {
        wall = col;
        distanceToContact = distance;
    }
}
