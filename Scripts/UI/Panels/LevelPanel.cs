using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AnotherWorld.UI
{
    public class LevelPanel : UIBase
    {
        [Header("Indicator")]
        [SerializeField] private TextMeshProUGUI levelText;

        /// <summary>
        /// Sets the LevelUI text to 'index + 1'
        /// </summary>
        /// <param name="index">Index of the level (zero based)</param>
        public void SetLevelText(int index)
        {
            levelText.text = "Level " + (index + 1).ToString();
        }      
    }
}