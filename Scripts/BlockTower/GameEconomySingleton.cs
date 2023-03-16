using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEconomySingleton : MonoBehaviour
{
    [SerializeField] private GameEconomySO m_GameEconomySO;

    public static GameEconomySingleton Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<GameEconomySingleton>();

            return instance;
        }
    }

    private static GameEconomySingleton instance;

    public GameEconomySO GetGameEconomy()
    {
        return m_GameEconomySO;
    }
}
