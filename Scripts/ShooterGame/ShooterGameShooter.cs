using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterGameShooter : MonoBehaviour
{
    public void ShootBall()
    {
        var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.layer = 1;
        var ballTransform = ball.transform;
        ballTransform.localScale = Vector3.one * 0.3f;
        var ballRigidBody = ball.AddComponent<Rigidbody>();
        ballRigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        var mousePos = Input.mousePosition;
        mousePos.z = 1f;
        var position = Camera.main.ScreenToWorldPoint(mousePos);
        var direction = position - Camera.main.transform.position;
        direction = direction.normalized;
        ballTransform.position = Camera.main.transform.position;
        ballRigidBody.velocity = direction * 100f;
    }
}
