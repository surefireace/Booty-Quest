using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// By Michael Taylor

public class LoadMainScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene(0);
            Debug.Log("Loaded Main Scene");
            Debug.Log(this.gameObject.name);
        }
    }
}
