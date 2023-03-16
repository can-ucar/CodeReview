using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

namespace AnotherWorld.Core
{
    public enum Haptics { Selection, Success, Warning, Failure, LightImpact, MediumImpact, HeavyImpact, RigidImpact, SoftImpact, None }
    
    public static class HapticManager
    {
        private static float s_InitialTime = 0;
        private static float s_HapticInterval = 0.05f;
        
        public static void SetVibration(Haptics _HapticType)
        {
            if (Time.time < s_InitialTime + s_HapticInterval)
            {
                return;
            }
            else
            {
                s_InitialTime = Time.time;
            }
           // Debug.LogError("HAPTİC WORKS!!!");

            switch (_HapticType)
            {
                case Haptics.LightImpact:
                    if (Application.platform == RuntimePlatform.Android)
                        MMVibrationManager.Haptic(HapticTypes.LightImpact);
                    else MMVibrationManager.Haptic(HapticTypes.LightImpact);
                    break;
                case Haptics.MediumImpact:
                    if (Application.platform == RuntimePlatform.Android)
                        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                    else MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                    break;
                case Haptics.HeavyImpact:
                    if (Application.platform == RuntimePlatform.Android)
                        MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
                    else MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
                    break;
                case Haptics.RigidImpact:
                    if (Application.platform == RuntimePlatform.Android)
                        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                    else MMVibrationManager.Haptic(HapticTypes.RigidImpact);
                    break;
            }

        }
    }
}

