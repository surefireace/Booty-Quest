using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// By Michael Taylor

public class PirateIsland : MonoBehaviour
{
    private static int m_bankTreasure = 20;
    private static int m_maxBankTreasure;

    public static int BankTreasure
    {
        get
        {
            return m_bankTreasure;
        }

        set
        {
            m_bankTreasure = value;
        }
    }

    public static int MaxBankTreasure
    {
        get
        {
            return m_maxBankTreasure;
        }

        set
        {
            m_maxBankTreasure = value;
        }
    }
}
