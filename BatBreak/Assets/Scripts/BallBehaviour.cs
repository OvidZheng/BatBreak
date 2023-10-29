using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallBehaviour : MonoBehaviour
{
    private AudioControl mAudioControl;
    
    public float speed = 5.0f; // 你可以调整速度

    private Rigidbody rb;
    private Vector3 direction;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        mAudioControl = GetComponent<AudioControl>();

        // 随机选择一个水平方向
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)).normalized;

        
    }



    private void FixedUpdate()
    {
        MoveBall();
    }
    
    private void MoveBall()
    {
        Vector3 newPosition = transform.position + direction * speed * Time.fixedDeltaTime;
        rb.transform.SetPositionAndRotation(newPosition, quaternion.identity);
    }

    private void OnTriggerEnter(Collider other)
    {
        Vector3 castDirection = other.ClosestPoint(transform.position) - transform.position;
        Ray ray = new Ray(transform.position, castDirection);
        if(Physics.Raycast(ray, out RaycastHit hit,  castDirection.magnitude * 2, LayerMask.GetMask("Obastacle")))
        {
            // 当球与其他物体碰撞时
            Vector3 hitNormal = Vector3.Reflect(direction, hit.normal);
            direction = hitNormal.normalized;
            
            mAudioControl.PlayBounce();
        }

    }
}
