using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Michael Taylor

public static class PlayerShipData
{
    private static int m_armor = 1;
    private static int m_maxArmor = 2;
    private static int m_cannonDamage = 8;
    private static int m_maxCannonDamage = 12;
    private static float m_speed = 13.0f;
    private static float m_maxSpeed = 21.0f;
    private static int m_treasure = 0;
    private static int m_maxShipTreasure = 500;
    private static int m_currAmmo = 65;
    private static int m_maxAmmo = 130;
    private static float m_health = 100;
    private static float m_maxHealth = 100;
    private static bool m_hullUpgraded = false;
    private static float m_maxSteerSpeed = 0.5f;
    private static float m_steerSpeed = 0.25f;

    public static int Armor
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

    public static int CannonDamage
    {
        get
        {
            return m_cannonDamage;
        }

        set
        {
            m_cannonDamage = value;
        }
    }

    public static float Speed
    {
        get
        {
            return m_speed;
        }

        set
        {
            m_speed = value;
        }
    }

    public static int Treasure
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

    public static int CurrAmmo
    {
        get
        {
            return m_currAmmo;
        }

        set
        {
            m_currAmmo = value;
        }
    }

    public static float Health
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

    public static int MaxShipTreasure
    {
        get
        {
            return m_maxShipTreasure;
        }

        set
        {
            m_maxShipTreasure = value;
        }
    }

    public static bool HullUpgraded
    {
        get
        {
            return m_hullUpgraded;
        }

        set
        {
            m_hullUpgraded = value;
        }
    }

    public static float MaxSteerSpeed
    {
        get
        {
            return m_maxSteerSpeed;
        }

        set
        {
            m_maxSteerSpeed = value;
        }
    }

    public static int MaxCannonDamage
    {
        get
        {
            return m_maxCannonDamage;
        }

        set
        {
            m_maxCannonDamage = value;
        }
    }

    public static float SteerSpeed
    {
        get
        {
            return m_steerSpeed;
        }

        set
        {
            m_steerSpeed = value;
        }
    }

    public static float MaxSpeed
    {
        get
        {
            return m_maxSpeed;
        }

        set
        {
            m_maxSpeed = value;
        }
    }

    public static int MaxArmor
    {
        get
        {
            return m_maxArmor;
        }

        set
        {
            m_maxArmor = value;
        }
    }

    public static int MaxAmmo
    {
        get
        {
            return m_maxAmmo;
        }

        set
        {
            m_maxAmmo = value;
        }
    }

    public static float MaxHealth
    {
        get
        {
            return m_maxHealth;
        }

        set
        {
            m_maxHealth = value;
        }
    }
}
