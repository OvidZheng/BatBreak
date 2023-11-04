using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    public float bulletSpeed;
    public LayerMask obstacleMask; // 障碍物的LayerMask
    void Update()
    {
        transform.position = transform.position + bulletSpeed * transform.forward * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检测障碍物LayerMask
        if ((obstacleMask.value & (1 << other.gameObject.layer)) > 0)
        {
            Destroy(gameObject); // 撞到障碍物，销毁子弹
        }
    }
}
