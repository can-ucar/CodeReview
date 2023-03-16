using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnotherWorld.Core;

public class ShooterGameManager : GameManager
{
    private List<GameObject> m_Targets;


    protected override void Awake()
    {
        base.Awake();
        ShooterGameTarget.OnInitialized += RegisterTarget;
        ShooterGameTarget.OnHit += TargetHit;
    }

    private void OnDestroy()
    {
        ShooterGameTarget.OnInitialized -= RegisterTarget;
        ShooterGameTarget.OnHit -= TargetHit;
    }

    protected override void Initialize()
    {
        m_Targets = new List<GameObject>();

        base.Initialize();
    }


    private void RegisterTarget(GameObject target)
    {
        m_Targets.Add(target);
    }


    private void TargetHit(GameObject target)
    {
        AddCoin(10);

        m_Targets.Remove(target);
        if (m_Targets.Count < 1)
        {
            LevelWin();
        }
    }


}
