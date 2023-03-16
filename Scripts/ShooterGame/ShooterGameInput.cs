using AnotherWorld.Core;
using UnityEngine;

public class ShooterGameInput : InputManager
{
    public ShooterGameShooter m_Shooter;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_Shooter.ShootBall();
        }
    }
}