using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeDetect : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other) {
        // Make sure the trigger enter isn't caused from the parent character object
        bool isSame = transform.parent.transform == other.transform;

        // Check to see if the cone/character detected an enemy character
        if(!isSame && other.gameObject.tag == "Character") {
            //print(gameObject.transform.parent.gameObject.name + " SEES " + other.gameObject.name);
            // Notify AIController
            gameObject.GetComponentInParent<AIController>().CharacterUpdate(other.gameObject);
        }
        // Check to see if cone detected a wall
        if(other.gameObject.tag == "Wall") {
            // Get distance to wall
            float distance = 0;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
            foreach(RaycastHit hit in hits) {
                if(hit.collider.tag == "Wall") {
                    distance = hit.distance;
                }
            }
            gameObject.GetComponentInParent<AIController>().NotifyWall(true, distance);
        }
    }

    private void OnTriggerStay(Collider other) {
        bool isSame = transform.parent.transform == other.transform;
        if(!isSame && other.gameObject.tag == "Character") {
            gameObject.GetComponentInParent<AIController>().CharacterUpdate(other.gameObject);
        }
        if(other.gameObject.tag == "Wall") {
            // Get distance to wall
            float distance = 0;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
            foreach(RaycastHit hit in hits) {
                if(hit.collider.tag == "Wall") {
                    distance = hit.distance;
                }
            }
            gameObject.GetComponentInParent<AIController>().NotifyWall(true, distance);
        }
    }

    private void OnTriggerExit(Collider other) {
        bool isSame = transform.parent.transform == other.transform;
        if(!isSame && other.gameObject.tag == "Character") {
            gameObject.GetComponentInParent<AIController>().CharacterUpdate(null);
        }
        if(other.gameObject.tag == "Wall") {
            gameObject.GetComponentInParent<AIController>().NotifyWall(false, Mathf.Infinity);
        }
    }

    // Detection with Zone Wall needs to use Collision events instead
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "ZoneWall") {
            // Get distance to wall
            float distance = 0;
            RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
            foreach(RaycastHit hit in hits) {
                if(hit.collider.tag == "ZoneWall") {
                    distance = hit.distance;
                }
            }
            gameObject.GetComponentInParent<AIController>().NotifyWall(true, distance);
        }
    }

    private void OnCollisionExit(Collision other) {
        if(other.gameObject.tag == "ZoneWall") {
            gameObject.GetComponentInParent<AIController>().NotifyWall(false, Mathf.Infinity);
        }
    }
}
