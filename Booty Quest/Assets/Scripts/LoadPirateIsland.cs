using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// By Michael Taylor

public class LoadPirateIsland : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(1);
            Debug.Log("Loaded Pirate Island");
            Debug.Log(this.gameObject.name);
        }
    }
}
