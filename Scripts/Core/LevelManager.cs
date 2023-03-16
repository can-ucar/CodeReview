using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnotherWorld.Core
{
    public interface ILevelManager
    {
        public void LoadLevel(int levelIndex);
        public void ReloadLevel();
    }

    public class LevelManager : Manager<LevelManager>, ILevelManager
    {
        [SerializeField] private LevelContainer m_LevelContainer;

#if UNITY_EDITOR
        [Tooltip("Load certain level for development. Set -1 to load current level")]
        [SerializeField] private int m_LoadLevelOverride = -1;
#endif
        [Tooltip("Loop levels starting from this 'level index' (inclusive) after completion of all levels.\nSet to '0' to loop starting from the initial level")]
        [SerializeField] private int m_LoopStartIndex = 0;
        [Tooltip("Should looping be random instead of sequential after completion of all levels")]
        [SerializeField] private bool m_LoopLevelsRandomly = false;


        private int m_CurrentLevelIndex;
        private GameLevel m_CurrentLevel;

        public event Action OnLevelLoad;

        /// <summary>
        /// Loads the level stored at given 'index' of the 'Level Container'
        /// </summary>
        /// <param name="index">Index of the level (zero based)</param>
        public void LoadLevel(int index)
        {
#if UNITY_EDITOR
            index = m_LoadLevelOverride != -1 ? m_LoadLevelOverride : index;
#endif
            var levelCount = m_LevelContainer.m_Levels.Count;

            if (index >= levelCount)
            {
                if (m_LoopLevelsRandomly)
                {
                    m_CurrentLevelIndex = Random.Range(m_LoopStartIndex, levelCount);
                }
                else
                {
                    var n = m_LevelContainer.m_Levels.Count - m_LoopStartIndex;
                    m_CurrentLevelIndex = m_CurrentLevelIndex % n + m_LoopStartIndex;
                }
            }
            else
            {
                m_CurrentLevelIndex = index;
            }

            var level = m_LevelContainer.m_Levels[m_CurrentLevelIndex];
            if (m_CurrentLevel)
            {
                m_CurrentLevel.Unload();
            }
            m_CurrentLevel = level;
            m_CurrentLevel.Load();
        }

        public void ReloadLevel()
        {
            m_CurrentLevel.Reload();
        }

        public LevelObject GetCurrentLevelObject()
        {
            if (m_CurrentLevel)
            {
                return m_CurrentLevel.m_LevelInstance ? m_CurrentLevel.m_LevelInstance : m_CurrentLevel.m_LevelPrefab;
            }
            else
                return null;
        }
    }

}

