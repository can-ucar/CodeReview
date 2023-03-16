using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AnotherWorld.UI
{
    public class CoinPanel : UIBase
    {
        [Header("Indicator")]
        [SerializeField] private NumberCounter m_moneyText;

        public void SetLevelCoin(int value)
        {
            StartCoroutine(SetCoin(value));
        }

        IEnumerator SetCoin(int value)
        {
            if (value > 0) yield return new WaitForSeconds(0.9f);
            m_moneyText.Value = value;
        }
    }
}



