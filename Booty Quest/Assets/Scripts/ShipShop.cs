using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Michael Taylor

public class ShipShop : MonoBehaviour
{
    private TextMesh m_textMesh;
    private GameObject m_textObj;
    private bool m_isTriggered;

    // Use this for initialization
    void Start()
    {
        m_textObj = GameObject.FindGameObjectWithTag("ShopTextMesh");
        if (!m_textObj)
            Debug.LogError("Text Object not attached!");

        m_textMesh = m_textObj.GetComponent<TextMesh>();
        if (!m_textMesh)
            Debug.LogError("TextMesh not attached!");

        m_textMesh.text = "Ship Shop";
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isTriggered)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
            {
                Debug.Log("Open Shop!");
                UILayer.Instance.ToggleShopUI();
            }
        }
        m_textObj.transform.LookAt(2 * m_textObj.transform.position - Camera.main.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player touched shop!");
            m_isTriggered = true;
            m_textMesh.text = "Spacebar to work on ship";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player left shop!");
            m_isTriggered = false;
            m_textMesh.text = "Ship Shop";

            if (UILayer.Instance.UpgradeUIOpened())
                UILayer.Instance.ToggleShopUI();
        }
    }

    //----------------------------------------------------------------//
    // Button Delegates                                               //
    //----------------------------------------------------------------//
    public void RepairShip()
    {
        if (PlayerShipData.Health >= 100)
        {
            UILayer.Instance.WriteToGameLog("Ship at max health!");
            Debug.Log("Ship at max health");
            return;
        }

        if (PirateIsland.BankTreasure >= 10)
        {
            PlayerShipData.Health = 100;
            PirateIsland.BankTreasure -= 10;
            UILayer.Instance.WriteToGameLog("Ship Repaired!");
            Debug.Log("Ship Repaired");
        }
        else
        {
            UILayer.Instance.WriteToGameLog("Not enough gold in Bank");
            Debug.Log("Not enough gold in Bank");
        }
    }

    public void BuyAmmo()
    {
        if (PlayerShipData.CurrAmmo == PlayerShipData.MaxAmmo)
        {
            UILayer.Instance.WriteToGameLog("Max Ammo Capacity!");
            Debug.Log("Max Ammo Capacity!");
            return;
        }

        if (PirateIsland.BankTreasure >= 10)
        {
            PlayerShipData.CurrAmmo += 50;
            if (PlayerShipData.CurrAmmo > PlayerShipData.MaxAmmo)
                PlayerShipData.CurrAmmo = PlayerShipData.MaxAmmo;
            PirateIsland.BankTreasure -= 10;

            UILayer.Instance.WriteToGameLog("+Ammo");
            Debug.Log("Purchased Ship Ammo");
        }
        else
        {
            UILayer.Instance.WriteToGameLog("Not enough gold in Bank");
            Debug.Log("Not enough gold in Bank");
        }
    }

    public void IncreaseDamage()
    {
        if (PlayerShipData.CannonDamage == PlayerShipData.MaxCannonDamage)
        {
            UILayer.Instance.WriteToGameLog("Cannon Damage Maxed!");
            Debug.Log("Cannon Damage Maxed!");
            return;
        }

        if (PirateIsland.BankTreasure >= 100)
        {
            PlayerShipData.CannonDamage += 2;
            PirateIsland.BankTreasure -= 100;
            UILayer.Instance.WriteToGameLog("+Damage");
            Debug.Log("Upgraded cannon damage");
        }
        else
        {
            UILayer.Instance.WriteToGameLog("Not enough gold in Bank");
            Debug.Log("Not enough gold in Bank");
        }
    }

    public void IncreaseArmor()
    {
        if (PlayerShipData.Armor == PlayerShipData.MaxArmor)
        {
            UILayer.Instance.WriteToGameLog("Armor Maxed!");
            Debug.Log("Armor Maxed!");
            return;
        }

        if (PirateIsland.BankTreasure >= 60)
        {
            ++PlayerShipData.Armor;
            PirateIsland.BankTreasure -= 60;
            UILayer.Instance.WriteToGameLog("+Armor");
            Debug.Log("Upgraded ship armor");
        }
        else
        {
            UILayer.Instance.WriteToGameLog("Not enough gold in Bank");
            Debug.Log("Not enough gold in Bank");
        }
    }

    public void IncreaseSpeed()
    {
        if (PlayerShipData.Speed == PlayerShipData.MaxSpeed)
        {
            UILayer.Instance.WriteToGameLog("Speed Maxed!");
            Debug.Log("Speed Maxed!");
            return;
        }

        if (PirateIsland.BankTreasure >= 50)
        {
            PlayerShipData.Speed += 2;
            PirateIsland.BankTreasure -= 50;
            UILayer.Instance.WriteToGameLog("+Speed");
            Debug.Log("Upgraded Speed");
        }
        else
        {
            UILayer.Instance.WriteToGameLog("Not enough gold in Bank");
            Debug.Log("Not enough gold in Bank");
        }
    }

    public void UpgradeHull()
    {
        if (PlayerShipData.HullUpgraded)
        {
            UILayer.Instance.WriteToGameLog("Hull Maxed");
            Debug.Log("Hull Maxed!");
            return;
        }

        if (PirateIsland.BankTreasure >= 1000)
        {
            PlayerShipData.HullUpgraded = true;
            ++PlayerShipData.MaxArmor;
            PlayerShipData.MaxSpeed += 6.0f;
            PlayerShipData.MaxCannonDamage += 6;
            PlayerShipData.MaxSteerSpeed += 0.25f;
            PlayerShipData.MaxAmmo += 195;
            PlayerShipData.MaxShipTreasure *= 3;

            PirateIsland.BankTreasure -= 1000;
            UILayer.Instance.WriteToGameLog("Hull Upgraded.\nNew Upgrades Available.");
            Debug.Log("Hull Upgraded");
        }
        else
        {
            UILayer.Instance.WriteToGameLog("Not enough gold in Bank");
            Debug.Log("Not enough gold in Bank");
        }
    }

    public void IncreaseSteering()
    {
        if (PlayerShipData.SteerSpeed == PlayerShipData.MaxSteerSpeed)
        {
            UILayer.Instance.WriteToGameLog("SteerSpeed Maxed!");
            Debug.Log("SteerSpeed Maxed!");
            return;
        }

        if (PirateIsland.BankTreasure >= 50)
        {
            PlayerShipData.SteerSpeed += 0.25f;
            PirateIsland.BankTreasure -= 50;
            UILayer.Instance.WriteToGameLog("+SteerSpeed");
            Debug.Log("Upgraded Steerspeed!");
        }
        else
        {
            UILayer.Instance.WriteToGameLog("Not enough gold in Bank");
            Debug.Log("Not enough gold in bank!");
        }
    }
}
