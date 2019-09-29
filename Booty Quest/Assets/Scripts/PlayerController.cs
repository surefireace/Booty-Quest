using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// By: Aaron Peneueta

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    // TODO: May remove
    //private Vector3 m_centerOfMass;
    //Transform m_centerOfMassTransform;

    [SerializeField]
    private float m_speed = 5.0f;
    [SerializeField]
    private float m_lerpTime = 2.0f;
    //[SerializeField]
    //private float m_decelerateSpeed = 4.0f;
    //[SerializeField]
    //private float m_speed = 1.0f;
    [SerializeField]
    private float m_steerSpeed = 1.0f;
    [SerializeField]
    private float m_movementThreshold = 0.0f;
    [SerializeField]
    private float m_reloadSpeed = 5.0f;
    [SerializeField]
    private GameObject m_cannonball = null;         // This is the gameobject that is fired from the cannons
    [SerializeField]
    private float m_maxHealth = 100;      // the maximum health for the ship
    [SerializeField]
    private int m_maxTreasure = 600;  // the max treasure the ship can carry
    [SerializeField]
    private float m_treasureSpeedFactor = .25f; // how much treasure effects the speed of the ship

    //public float m_maxTurn = 5.0f;
    //public float m_turnLerpTime = 2.0f;
    //public float m_decreaseTurn = 1.0f;
    //private float m_currentTurn = 0.0f;

    private float m_verticalInput;
    private float m_movementFactor;
    private float m_horizontalInput;
    private float m_steerFactor;
    private float m_currentSpeed = 0.0f;
    private float m_maxSteerSpeed = 0.75f;
    private float m_maxSpeed = 23;

    private int m_startingHealth = 100;

    private Rigidbody m_rigidbody;
    private Vector3 m_forward;

    private GameObject[] m_leftCannons;
    private GameObject[] m_rightCannons;
    private bool m_canFire = true;
    private bool m_fireLeft = true;
    private bool m_fireRight = true;
    private float m_force = 300;    // the force aplied on the cannon ball when fireing
    private float m_cannonBallLife = 1f;


    [SerializeField] private int m_ammo;
    [SerializeField] private float m_health;
    [SerializeField] private int m_treasure;
    [SerializeField] private int m_cannonDamage;
    [SerializeField] private int m_maxCannonDamage;

    // Variable used for the range between this numbers positive and negative value
    // When velocity is in this range player cannot turn
    [SerializeField] private float m_stopTurn = 0.07f;

    [SerializeField]
    private GameObject m_movementFX= null;

    //private int m_cannons;
    private int m_armor;
    private int m_maxArmor;
    private int m_maxAmmo;


    private void Awake()
    {
        m_ammo = PlayerShipData.CurrAmmo;
        m_maxAmmo = PlayerShipData.MaxAmmo;
        m_treasure = PlayerShipData.Treasure;
        m_maxTreasure = PlayerShipData.MaxShipTreasure;
        m_health = PlayerShipData.Health;
        m_maxHealth = PlayerShipData.MaxHealth;
        m_cannonDamage = PlayerShipData.CannonDamage;
        m_speed = PlayerShipData.Speed;
        m_maxSpeed = PlayerShipData.MaxSpeed;
        m_armor = PlayerShipData.Armor;
        m_maxArmor = PlayerShipData.MaxArmor;
        m_maxCannonDamage = PlayerShipData.MaxCannonDamage;
        m_steerSpeed = PlayerShipData.SteerSpeed;
        m_maxSteerSpeed = PlayerShipData.MaxSteerSpeed;
    }

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        m_leftCannons = GameObject.FindGameObjectsWithTag("PlayerLeft");
        m_rightCannons = GameObject.FindGameObjectsWithTag("PlayerRight");
    }

    public float Health
    {
        get
        {
            return m_health;
        }
        set
        {
            m_health = value;
        }

    }

    public float MaxHealth
    {
        get
        {
            return m_maxHealth;
        }
    }

    public int Ammo
    {
        get
        {
            return m_ammo;
        }
    }

    public int Treasure
    {
        get
        {
            return m_treasure;
        }
        set
        {
            m_treasure = value;
        }

    }

    public int MaxTreasure
    {
        get
        {
            return m_maxTreasure;
        }
    }

    private void Update()
    {
        if (PlayerShipData.CurrAmmo <= 0)
        {
            m_canFire = false;
            if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                UILayer.Instance.WriteToGameLog("Out of Ammo!");
            }
        }
        else
        {
            m_canFire = true;
        }

        if (m_canFire)
        {
            if (m_fireRight)
            {

                // Fires the right side cannons
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    Fire(m_rightCannons, 1);
                }
            }
            if (m_fireLeft)
            {
                // Fires the left side cannons
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Fire(m_leftCannons, 2);
                }
            }
        }
        // for debugging
        if (Input.GetKeyDown(KeyCode.H))
        {
            DamageHealth(10);
        }

        // update static data
        PlayerShipData.CannonDamage = m_cannonDamage;
        PlayerShipData.MaxCannonDamage = m_maxCannonDamage;
        PlayerShipData.CurrAmmo = m_ammo;
        PlayerShipData.MaxAmmo = m_maxAmmo;
        PlayerShipData.Speed = m_speed;
        PlayerShipData.MaxSpeed = m_maxSpeed;
        PlayerShipData.Armor = m_armor;
        PlayerShipData.MaxArmor = m_maxArmor;
        PlayerShipData.Health = m_health;
        PlayerShipData.MaxHealth = m_maxHealth;
        PlayerShipData.Treasure = m_treasure;
        PlayerShipData.MaxShipTreasure = m_maxTreasure;
        PlayerShipData.SteerSpeed = m_steerSpeed;
        PlayerShipData.MaxSteerSpeed = m_maxSteerSpeed;

        if (PlayerShipData.Health <= 0)
        {
            // reset the ship back to starting values
            PlayerShipData.Treasure = 0;
            PlayerShipData.MaxShipTreasure = 500;
            PlayerShipData.Armor = 1;
            PlayerShipData.MaxArmor = 2;
            PlayerShipData.Health = m_startingHealth;
            PlayerShipData.MaxHealth = m_startingHealth;
            PlayerShipData.CurrAmmo = 65;
            PlayerShipData.MaxAmmo = 130;
            PlayerShipData.HullUpgraded = false;
            PlayerShipData.Speed = 13.0f;
            PlayerShipData.MaxSpeed = 23.0f;
            PlayerShipData.CannonDamage = 8;
            PlayerShipData.MaxCannonDamage = 16;
            PlayerShipData.SteerSpeed = 0.25f;
            PlayerShipData.MaxSteerSpeed = 0.75f;

            SceneManager.LoadScene(0);
        }
    }

    void FixedUpdate()
    {
        if (transform.position.y > 0.5f || transform.position.y < 0.5f)
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
        //Balance();
        Movement();
    }

    // TODO: remove if not using
    // This is for the buoyancy effect
    //void Balance()
    //{
    //    if (!m_centerOfMassTransform)
    //    {
    //        m_centerOfMassTransform = new GameObject("m_centerMass").transform;
    //        m_centerOfMassTransform.SetParent(transform);
    //    }

    //    m_centerOfMassTransform.position = m_centerOfMass + m_centerOfMassTransform.position;
    //    m_rigidbody.centerOfMass = m_centerOfMassTransform.position;
    //}

    void Movement()
    {
        if(m_rigidbody.angularVelocity != Vector3.zero)
        {
            m_rigidbody.angularVelocity = Vector3.zero;
        }

        m_verticalInput = Input.GetAxis("Vertical");
        m_horizontalInput = Input.GetAxis("Horizontal");

        bool movingX = (m_rigidbody.velocity.x > m_stopTurn || m_rigidbody.velocity.x < -m_stopTurn);
        bool movingZ = (m_rigidbody.velocity.z > m_stopTurn || m_rigidbody.velocity.z < -m_stopTurn);


        if (m_horizontalInput != 0 && (movingX || movingZ))
        {
            Steer(m_horizontalInput);
        }

        if(movingX || movingZ)
        {
            if (m_movementFX.GetComponent<ParticleSystem>().isStopped)
            {
                m_movementFX.GetComponent<ParticleSystem>().Play();
            }

        }
        else
        {
            m_movementFX.GetComponent<ParticleSystem>().Stop();
        }

        m_currentSpeed += (m_speed / m_lerpTime) * Time.deltaTime;
        float speedLoss = ((float)m_treasure / m_maxTreasure) * m_treasureSpeedFactor;
        float maxSpeed = m_speed * (1 - speedLoss);

        m_currentSpeed = Mathf.Clamp(m_currentSpeed, 0, maxSpeed);
        m_rigidbody.AddForce(transform.forward * m_verticalInput * m_currentSpeed);

    }

    void Steer(float horizontalInput)
    {
        m_steerFactor = Mathf.Lerp(m_steerFactor, horizontalInput, Time.deltaTime / m_movementThreshold);
        transform.Rotate(0.0f, m_steerFactor * m_steerSpeed, 0.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // TODO: Temporary fix when colliding with gameobjects
        Vector3 temp = m_rigidbody.velocity;
        temp.y = 0;
        m_rigidbody.velocity = temp;
        m_rigidbody.angularVelocity = Vector3.zero;
    }

    private void Fire(GameObject[] cannons, int side)
    {
        foreach (GameObject muzzle in cannons)
        {
            // Creates the Cannon ball
            GameObject cannonball = Instantiate(
                m_cannonball,
                muzzle.transform.position,
                muzzle.transform.rotation);

            muzzle.GetComponent<ParticleSystem>().Play();

            // Sets the cannonball damage to ships Damage variable
            cannonball.GetComponent<CBallMovement>().Damage = m_cannonDamage;

            cannonball.GetComponent<Rigidbody>().AddForce(cannonball.transform.forward * m_force);
            Destroy(cannonball, m_cannonBallLife);


            if(side == 1)
            {
                m_fireRight = false;
                StartCoroutine(ReloadRight());
            }
            if(side == 2)
            {
                m_fireLeft = false;
                StartCoroutine(ReloadLeft());
            }

            --m_ammo;
            if (m_ammo <= 0)
            {
                m_ammo = 0;
                break;
            }

        }
    }

    private IEnumerator ReloadRight()
    {
        yield return new WaitForSeconds(m_reloadSpeed);
        m_fireRight = true;
    }

    private IEnumerator ReloadLeft()
    {
        yield return new WaitForSeconds(m_reloadSpeed);
        m_fireLeft = true;
    }

    public void DamageHealth(int damage)
    {   
        m_health -= (damage/m_armor);

        // For debugging
        print("health is" + m_health);
    }

    // [Mike] adds ammo to player ship
    public void AddAmmo(int amount)
    {
        m_ammo += amount;

        if (m_ammo >= PlayerShipData.MaxAmmo)
        {
            m_ammo = PlayerShipData.MaxAmmo;
        }

        UILayer.Instance.WriteToGameLog("+" + amount + "Ammo");
        Debug.Log("Ammo: " + amount + " added to player");
    }

    // [Mike] adds treasure to player ship
    public void AddTreasure(int amount)
    {
        m_treasure += amount;

        if (m_treasure >= PlayerShipData.MaxShipTreasure)
        {
            m_treasure = PlayerShipData.MaxShipTreasure;
        }

        UILayer.Instance.WriteToGameLog("+" + amount + "G");
        Debug.Log("Treasure: " + amount + " added to player");
    }

}
