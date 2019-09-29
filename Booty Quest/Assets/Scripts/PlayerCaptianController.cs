using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Michael Taylor

public class PlayerCaptianController : MonoBehaviour
{
    [SerializeField]
    private float m_speed = 1.0f;
    [SerializeField]
    private float m_rotationSpeed = 90.0f;

    private CharacterController m_characterController;
    private Vector3 m_startPosition;

    private void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        if (!m_characterController)
            Debug.LogError("CharacterController not attached to Captain!");

        m_startPosition = gameObject.transform.position;
    }

    private void Movement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 moveVector = new Vector3(0.0f, 0.0f, verticalInput);
        moveVector.Normalize();
        m_characterController.Move(transform.forward * verticalInput * Time.deltaTime * m_speed);
        Vector3 rotationVector = new Vector3(0.0f, horizontalInput, 0.0f);
        transform.Rotate(rotationVector * Time.deltaTime * m_rotationSpeed);

        Vector3 gravityVector = new Vector3(0.0f, 0.0f, 0.0f);
        if (!m_characterController.isGrounded)
            gravityVector.y += Physics.gravity.y * Time.deltaTime;
        m_characterController.Move(gravityVector);
    }

	// Update is called once per frame
	private void Update ()
    {
        Movement();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Sea")
        {
            gameObject.transform.position = m_startPosition;
            gameObject.transform.rotation = Quaternion.identity;
            Debug.Log("Touching sea");
        }
    }
}
