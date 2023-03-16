using System.Collections.Generic;
using UnityEngine;

namespace AnotherWorld.Core
{
    [CreateAssetMenu(fileName = "LevelContainer", menuName = "ScriptableObjects/Level Container")]
    public class LevelContainer : ScriptableObject
    {
        public List<GameLevel> m_Levels;
    }

}

