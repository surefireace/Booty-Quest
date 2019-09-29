using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By: Aaron Peneueta

public class CameraController : MonoBehaviour
{

    private const float kMinYRot = -25.0f;
    private const float kMaxYRot = 45.0f;

    [SerializeField]
    private Transform m_lookAt;
    [SerializeField]
    private Transform m_camTransform;

    //private Camera cam;

    private Transform m_pivot;

    private Vector3 m_localRotation;
    [SerializeField]
    private float m_mouseSensitivity = 4.0f;
    [SerializeField]
    private float m_orbitDampening = 10.0f;


    // Use this for initialization
    private void Start()
    {
        m_camTransform = transform;
        //m_camTransform.position = new Vector3(0, 25, -m_distance);
        //cam = Camera.main;
        m_pivot = transform.parent.transform;
        m_localRotation.x = 90;
        m_localRotation.y = 0;

        m_pivot.rotation = m_lookAt.rotation;
        m_pivot.forward = m_lookAt.forward;
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            m_localRotation = m_lookAt.forward * 90;
            m_pivot.rotation = Quaternion.Lerp(m_pivot.rotation, m_lookAt.rotation, Time.deltaTime * m_orbitDampening);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Rotate();
        }

        //Quaternion qt = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        //m_pivot.rotation = Quaternion.Lerp(m_pivot.rotation, qt, Time.deltaTime * orbitDampening);
    }

    private void LateUpdate()
    {
        m_camTransform.LookAt(m_pivot.position);
        m_camTransform.Rotate(new Vector3(-15.0f, 0, 0));
    }

    public void Rotate()
    {
        //transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * 20);

        //Rotation of camera based on mouse coordinates
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            m_localRotation.x += Input.GetAxis("Mouse X") * m_mouseSensitivity;
            m_localRotation.y -= Input.GetAxis("Mouse Y") * m_mouseSensitivity;

            // Clamp the rotation to horizon and not flipping over at the top
            m_localRotation.y = Mathf.Clamp(m_localRotation.y, kMinYRot, kMaxYRot);
        }

        // Camera rig transformations
        Quaternion qt = Quaternion.Euler(m_localRotation.y, m_localRotation.x, 0);
        m_pivot.rotation = Quaternion.Lerp(m_pivot.rotation, qt, Time.deltaTime * m_orbitDampening);
    }
}