using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    public bool phasing = false;
    public GameObject GM;
    public GameObject dummyPrefab;
    [HideInInspector]
    public int playerID;

    private void Start()
    {
        GM = GameObject.Find("GameManager");
    }

    private void OnCollisionEnter(Collision other) {
        // Detect if character ran into another character
        if(phasing == false && other.gameObject.tag == "Character") {
            print("collided with a character");
            Destroy(this.gameObject);
            GM.GetComponent<GameManager>().PlayerHasDied(playerID);
        }
    }

    private void OnTriggerExit (Collider col)
    {
        if (col.gameObject.tag == "ZoneWall")
        {
            // animation?
            print("character hit a wall");
            Destroy(this.gameObject);
            GM.GetComponent<GameManager>().PlayerHasDied(playerID);
        }
    }
}
