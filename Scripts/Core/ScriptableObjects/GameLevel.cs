using UnityEngine;

namespace AnotherWorld.Core
{
    [CreateAssetMenu(fileName = "GameLevel", menuName = "ScriptableObjects/Game Level")]
    public class GameLevel : ScriptableObject
    {
        public LevelObject m_LevelPrefab;

        [HideInInspector] public LevelObject m_LevelInstance;

        public LevelObject Load()
        {
            m_LevelInstance = GameObject.Instantiate(m_LevelPrefab);
            return m_LevelInstance;
        }

        public void Unload()
        {
            Destroy(m_LevelInstance.gameObject);
        }

        public LevelObject Reload()
        {
            Unload();
            return Load();
        }
    }
}

