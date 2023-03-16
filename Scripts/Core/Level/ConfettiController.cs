using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnotherWorld.Core
{
    public class ConfettiController : MonoBehaviour
    {
        private List<ParticleSystem> particleSystems = new List<ParticleSystem>();

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            particleSystems = GetComponentsInChildren<ParticleSystem>().ToList();
        }

        public void PlayConfetti()
        {
            Invoke(nameof(StartPlay), 1.5f);
        }

        void StartPlay()
        {
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Play();
            }
        }
    }

}