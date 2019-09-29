using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

// By Donovan Colen

// required components
[RequireComponent(typeof(NavMeshAgent))]


public class EnemyShipAI : MonoBehaviour
{

    //tuneables
    [SerializeField]
    private float m_sightRange = 15;      // the range at which the ship will detect other ships
    [SerializeField]
    private float m_chaseRange = 20;      // the range at which the ship will stop chaseing other ships
    [SerializeField]
    private float m_combatRange = 10;      // the range at which the ship will try to attack other ships
    [SerializeField]
    private float m_maxHealth = 100;      // the maximum health for the ship
    [SerializeField]
    private GameObject m_projectile = null;      // the projectile the ship shoots
    [SerializeField]
    private float m_fireAngleTolerance = .1f;      // the angle tolerance for when the ship shoots
    [SerializeField]
    private float m_followDist = 7;      // the distance the ship follows the merchant ships
    [SerializeField]
    private float m_reloadTime = 5;     // the time it takes the ship to reload
    [SerializeField]
    private GameObject m_ammoDrop;      // the ammo drop it drops on death
    [SerializeField]
    private int m_ammoToDrop = 10; // the amount of ammo that will be dropped on death
    [SerializeField]
    private int m_cannonDamage = 6;
    [SerializeField]
    private int m_avoidPirateIslandDist = 50;
    [SerializeField]
    private int m_sideShotThreshold = 5;
    [SerializeField]
    private int m_sideDist = 20;




    //private variables
    private NavMeshAgent m_navAgent = null;         // the nav mesh agent for the ship. it determines the speed, stopping distance, ect.. 
    private GameObject[] m_patrolRoute = null;      // the patrol route the ship takes
    private int m_patrolIndex = 0;
    private float m_health = 100;                   // the ships current health
    private bool m_playerFound = false;
    private bool m_canFire = true;
    private float m_force = 300;                    // the force aplied on the cannon ball when fireing
    private float m_angleToTarget = 0;              // the angle from the ships forward to the player
    private float m_angleToEnemy = 0;               // the angle from the player's forward to this ship
    private float m_orgSpeed = 0;                   // the orignal speed of the ship
    private float m_orgStopDist = 0;                // the original stopping distance of the ship
    private float m_followTolerance = 5;
    private float m_armor = 1;
    private float m_cannonBallLife = 1f;

    // Use this for initialization
    void Start ()
    {
        m_navAgent = GetComponent<NavMeshAgent>();
        m_navAgent.destination = m_patrolRoute[m_patrolIndex].transform.position;
        ++m_patrolIndex;
        m_orgSpeed = m_navAgent.speed;
        m_orgStopDist = m_navAgent.stoppingDistance;
        m_health = m_maxHealth;
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

    public float Armor
    {
        get
        {
            return m_armor;
        }
        set
        {
            m_armor = value;
        }

    }


    public float MaxHealth
    {
        get
        {
            return m_maxHealth;
        }
    }


    public void SetPatrolRoute(GameObject[] route)
    {
        m_patrolRoute = route;
    }

    private void ToNextPoint()
    {
        if (m_patrolIndex >= m_patrolRoute.Length)
        {
            m_patrolIndex = 0;
        }

        if (m_navAgent.remainingDistance < m_navAgent.stoppingDistance)
        {
            m_navAgent.destination = m_patrolRoute[m_patrolIndex].transform.position;
            ++m_patrolIndex;

        }

    }


    private void SeekOutOtherShips()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // if player is in sight and not under islands protection
        if(Vector3.Distance(gameObject.transform.position, player.transform.position) < m_sightRange &&
           Vector3.Distance(player.transform.position, Vector3.zero) > m_avoidPirateIslandDist)
        {
            m_navAgent.destination = player.transform.position;
            m_navAgent.speed = m_orgSpeed;
            m_playerFound = true;

            // get the vector that points toward the player from this location
            Vector3 RotationToTarget = player.transform.position - transform.position;
            RotationToTarget.y = 0;
            RotationToTarget = RotationToTarget.normalized;

            m_angleToTarget = Vector3.SignedAngle(transform.forward.normalized, RotationToTarget, Vector3.up);

            // get the vector that points from the player to this location
            Vector3 RotationToEnemy = transform.position - player.transform.position;
            RotationToEnemy.y = 0;
            RotationToEnemy = RotationToEnemy.normalized;

            m_angleToEnemy = Vector3.SignedAngle(player.transform.forward.normalized, RotationToEnemy, Vector3.up);


            return;
        }

        GameObject[] merchants = GameObject.FindGameObjectsWithTag("Merchant");
        GameObject merchToFollow = null;
        bool ignorePoor = false;
        for(int i = 0; i < merchants.Length; ++i)
        {
            if (Vector3.Distance(gameObject.transform.position, merchants[i].transform.position) < m_sightRange)
            {
                if(merchants[i].GetComponent<MerchantShipAI>().MerchantType == MerchantShipAI.MerchType.k_royalty)  // perfer the royalty above all else
                {
                    merchToFollow = merchants[i];   // a reoyalty ship was found
                    break;  // no need to continue because already found the perfered ship
                }
                else if(merchants[i].GetComponent<MerchantShipAI>().MerchantType == MerchantShipAI.MerchType.k_wealthy) // perfer the wealthy above the poor
                {
                    merchToFollow = merchants[i];   // a wealthy ship was found
                    ignorePoor = true;  // no longer care about poor ships
                    continue;   // continue to see if a better ship is in range
                }
                else if(!ignorePoor)
                {
                    merchToFollow = merchants[i];
                }
            }

        }

        // if a ship in range was found follow it
        if (merchToFollow != null)
        {
            m_navAgent.destination = merchToFollow.transform.position;
            Debug.Assert(merchToFollow.GetComponent<NavMeshAgent>());

            if (Vector3.Distance(gameObject.transform.position, merchToFollow.transform.position) > m_followDist + m_followTolerance)
            {
                m_navAgent.speed = merchToFollow.GetComponent<NavMeshAgent>().speed;
                m_navAgent.speed += 2;
            }
            else if (Vector3.Distance(gameObject.transform.position, merchToFollow.transform.position) < m_followDist - m_followTolerance)
            {
                m_navAgent.speed = merchToFollow.GetComponent<NavMeshAgent>().speed;
                m_navAgent.speed -= 2;
            }
            else
            {
                m_navAgent.speed = merchToFollow.GetComponent<NavMeshAgent>().speed;
            }
        }

    }

    private void Fire(bool rightSide)
    {
        GameObject cannonGroup = gameObject.transform.Find("FirePoints").gameObject;

        if(cannonGroup == null)
        {
            Debug.LogError("could not find FirePoints chid");
        }

        for (int i = 0; i < cannonGroup.transform.childCount; ++i)
        {
            GameObject cannon = cannonGroup.transform.GetChild(i).gameObject;

            if (rightSide)
            {
                if (cannon.tag == "Right Side")
                {
                    Vector3 location = cannon.transform.position;
                    GameObject clone = Instantiate(m_projectile, location, cannon.transform.rotation);
                    clone.GetComponent<Rigidbody>().AddForce(clone.transform.forward * m_force);
                    clone.GetComponent<CBallMovement>().Damage = m_cannonDamage;
                    cannon.GetComponent<ParticleSystem>().Play();
                    Destroy(clone, m_cannonBallLife);
                    m_canFire = false;

                    StartCoroutine(Reload());
                }
            }
            else
            {
                if (cannon.tag == "Left Side")
                {
                    Vector3 location = cannon.transform.position;
                    GameObject clone = Instantiate(m_projectile, location, cannon.transform.rotation);
                    clone.GetComponent<Rigidbody>().AddForce(clone.transform.forward * m_force);
                    clone.GetComponent<CBallMovement>().Damage = m_cannonDamage;
                    cannon.GetComponent<ParticleSystem>().Play();
                    Destroy(clone, m_cannonBallLife);
                    m_canFire = false;

                    StartCoroutine(Reload());
                }

            }
        }
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(m_reloadTime);  // stops the gun from firing again till time is up
        m_canFire = true;
    }

    private void SailBy(GameObject player, Rigidbody playerRigid)
    {
        m_navAgent.isStopped = false;
        m_navAgent.autoBraking = false;
        m_navAgent.stoppingDistance = 0.1f;

        // get the vector that points toward the player from this location
        Vector3 RotationToTarget = player.transform.position - transform.position;
        RotationToTarget.y = 0;
        RotationToTarget = RotationToTarget.normalized;

        m_angleToTarget = Vector3.SignedAngle(transform.forward.normalized, RotationToTarget, Vector3.up);


        if (m_angleToEnemy > 0) //ship is on the right side of the player
        {
            Vector3 target = player.transform.position + (player.transform.right * m_sideDist);
            m_navAgent.destination = target;
            float dist = Vector3.Distance(transform.position, target);

            if (dist < 5)
            {
                m_navAgent.speed = playerRigid.velocity.magnitude;
                if (m_navAgent.speed < 5)
                {
                    m_navAgent.speed = m_orgSpeed;
                }
                if (m_canFire)
                {
                    float dot = Vector3.Dot(transform.forward, player.transform.forward);
                    if (dot > 0)   // faceing the same way
                    {
                        Fire(false);     // fire the left side cannons because the player is to the left
                    }
                    else if(dot < 0) // faceing opposite way
                    {
                        Fire(true);     // fire the right side cannons because the player is to the right
                    }
                }
            }
            else
            {
                m_navAgent.speed = m_orgSpeed;
            }
        }
        else if (m_angleToEnemy < 0) //ship is on the left side of the player
        {
            Vector3 target = player.transform.position + (-player.transform.right * m_sideDist);
            m_navAgent.destination = target;
            float dist = Vector3.Distance(transform.position, target);

            if (dist < 5)
            {
                m_navAgent.speed = playerRigid.velocity.magnitude;
                if (m_navAgent.speed < 5)
                {
                    m_navAgent.speed = m_orgSpeed;
                }
                if (m_canFire)
                {
                    float dot = Vector3.Dot(transform.forward, player.transform.forward);
                    if (dot > 0)   // faceing the same way
                    {
                        Fire(true);     // fire the left side cannons because the player is to the left
                    }
                    else if (dot < 0) // faceing opposite way
                    {
                        Fire(false);     // fire the right side cannons because the player is to the right
                    }
                }
            }
            else
            {
                m_navAgent.speed = m_orgSpeed;
            }
        }


    }

    private void Encircle(GameObject player)
    {
        m_navAgent.isStopped = true;
        m_navAgent.autoBraking = true;
        m_navAgent.stoppingDistance = m_orgStopDist;


        // get the vector that points from the player to this location
        Vector3 RotationToEnemy = transform.position - player.transform.position;
        RotationToEnemy.y = 0;
        RotationToEnemy = RotationToEnemy.normalized;

        m_angleToEnemy = Vector3.SignedAngle(player.transform.forward.normalized, RotationToEnemy, Vector3.up);


        // get the vector that points toward the player from this location
        Vector3 RotationToTarget =  player.transform.position - transform.position;
        RotationToTarget.y = 0;
        RotationToTarget = RotationToTarget.normalized;
        
        if(m_angleToTarget > 0)   // player is to the right turn left
        {
            m_navAgent.Move(transform.forward.normalized * m_orgSpeed / 200);

            // have the enemy turn guns toward the player instead of snaping to aim at them
            if (transform.right.normalized.x > RotationToTarget.x + m_fireAngleTolerance || transform.right.normalized.x < RotationToTarget.x - m_fireAngleTolerance ||
                transform.right.normalized.z > RotationToTarget.z + m_fireAngleTolerance || transform.right.normalized.z < RotationToTarget.z - m_fireAngleTolerance)
            {
                transform.right = Vector3.Slerp(transform.right, RotationToTarget, (m_navAgent.angularSpeed / 10) * Time.deltaTime);
            }
            else
            {
                if (m_canFire)
                {
                    Fire(true);     // fire the right side cannons because the player is to the right
                }
            }
        }
        else if (m_angleToTarget < 0)  // player is to the left turn right
        {
            m_navAgent.Move(transform.forward.normalized * m_orgSpeed / 200);
            Vector3 left = -transform.right.normalized;

            // have the enemy turn guns toward the player instead of snaping to aim at them
            if (left.x > RotationToTarget.x + m_fireAngleTolerance || left.x < RotationToTarget.x - m_fireAngleTolerance ||
                left.z > RotationToTarget.z + m_fireAngleTolerance || left.z < RotationToTarget.z - m_fireAngleTolerance)
            {
                left = Vector3.Slerp(left, RotationToTarget, (m_navAgent.angularSpeed / 10) * Time.deltaTime);
                transform.right = -left;
            }
            else
            {
                if (m_canFire)
                {
                    Fire(false);    // fire the left side cannons because the player is to the left
                }
            }

        }


    }

    private void Chase()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        float dist = Vector3.Distance(gameObject.transform.position, player.transform.position);

        if(Vector3.Distance(player.transform.position, Vector3.zero) <= m_avoidPirateIslandDist) // check to see if the player is protected by the island
        {
            m_navAgent.isStopped = false;
            m_navAgent.autoBraking = true;
            m_navAgent.destination = m_patrolRoute[m_patrolIndex].transform.position;
            m_playerFound = false;
            m_navAgent.speed = m_orgSpeed;
            m_navAgent.stoppingDistance = m_orgStopDist;
        }
        else if (dist < m_combatRange)   // try to shoot the player if in range
        {
            Rigidbody playerRigid = player.GetComponent<Rigidbody>();

            if (playerRigid.velocity.magnitude > m_sideShotThreshold)
            {
                SailBy(player, playerRigid);
            }
            else
            {
                Encircle(player);
            }
        }
        else if (dist > m_chaseRange)   // lost the player
        {
            m_navAgent.isStopped = false;
            m_navAgent.autoBraking = true;
            m_navAgent.destination = m_patrolRoute[m_patrolIndex].transform.position;
            m_playerFound = false;
            m_navAgent.speed = m_orgSpeed;
            m_navAgent.stoppingDistance = m_orgStopDist;
        }
        else    // continue chaseing the player
        {
            m_navAgent.isStopped = false;
            m_navAgent.stoppingDistance = m_orgStopDist;
            m_navAgent.autoBraking = true;
            m_navAgent.destination = player.transform.position;
        }

    }

    // Update is called once per frame
    void Update ()
    {
        ToNextPoint();
        if (!m_playerFound)
        {
            SeekOutOtherShips();
        }
        else
        {
            Chase();
        }

        // check health
        if (m_health <= 0)
        {
            // destroy ship
            Vector3 location = gameObject.transform.position;
            location.y = m_ammoDrop.transform.position.y;
            GameObject clone = Instantiate(m_ammoDrop, location, m_ammoDrop.transform.rotation);
            clone.GetComponent<AmmoDrop>().AmmoValue = m_ammoToDrop;
            AIManager.Instance.DestroyEnemy(gameObject);
            UILayer.Instance.RemoveEnemy(gameObject);
        }

    }
}
