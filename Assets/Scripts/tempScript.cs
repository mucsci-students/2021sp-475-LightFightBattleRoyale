using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tempScript : MonoBehaviour
{
    GameObject m_cam;

    // Start is called before the first frame update
    void Start()
    {
        m_cam = Camera.main.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float scale = .05f;
        float xMove = Input.GetAxis("Horizontal") * scale;
        float yMove = Input.GetAxis("Vertical") * scale;
        float zMove = 0;

        if(Input.GetKey(KeyCode.K)) {
            zMove = scale;
        }
        if(Input.GetKey(KeyCode.M)) {
            zMove = -scale;
        }

        transform.Translate(xMove, yMove, zMove);

        m_cam.transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
        m_cam.transform.eulerAngles = new Vector3(
            0,
            m_cam.transform.eulerAngles.y,
            m_cam.transform.eulerAngles.z
        );
    }
}
