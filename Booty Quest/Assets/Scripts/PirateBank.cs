using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Michael Taylor

public class PirateBank : MonoBehaviour
{
    private TextMesh m_textMesh;
    private GameObject m_textObj;
    private bool m_isTriggered;

	// Use this for initialization
	void Start ()
    {
        m_textObj = GameObject.FindGameObjectWithTag("BankTextMesh");
        if (!m_textObj)
            Debug.LogError("Text Object not attached!");

        m_textMesh = m_textObj.GetComponent<TextMesh>();
        if (!m_textMesh)
            Debug.LogError("TextMesh not attached!");

        m_isTriggered = false;
        m_textMesh.text = "Bank";
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_isTriggered)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
            {
                Debug.Log("Dropping off Loot!");
                PirateIsland.BankTreasure += PlayerShipData.Treasure;
                PlayerShipData.Treasure = 0;
            }
        }
        m_textObj.transform.LookAt(2 * m_textObj.transform.position - Camera.main.transform.position);
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player touched bank!");
            m_isTriggered = true;
            m_textMesh.text = "Spacebar to deposit loot";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player left bank!");
            m_isTriggered = false;
            m_textMesh.text = "Bank";
        }
    }
}
