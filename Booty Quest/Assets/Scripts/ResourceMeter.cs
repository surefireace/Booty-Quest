using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// By Michael Taylor

public class ResourceMeter : MonoBehaviour
{
    // tunables
    public int m_value;
    public int m_maxValue;

    // Slider component
    private Slider m_slider;

	// Use this for initialization
	void Start ()
    {
        m_slider = GetComponentInChildren<Slider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        // update the slider values
        m_slider.value = m_value;
        m_slider.maxValue = m_maxValue;
    }
}
