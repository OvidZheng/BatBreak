using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class AudioByPosition : MonoBehaviour
{
    public GameObject listenObject;  // 引用玩家对象
    private AudioSource audioSource;

    // 设定频率范围
    public float minPitch = 0.5f;
    public float maxPitch = 1.5f;

    // 设定距离范围
    public float minDistance = 0f;
    public float maxDistance = 10f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play();
    }

    void Update()
    {
        // 计算在XZ平面上从玩家到小球的向量
        Vector3 playerToBall = transform.position - listenObject.transform.position;
        playerToBall.y = 0;  // 忽略Y轴

        // 计算距离
        float distance = playerToBall.magnitude;

        // 根据距离映射音频pitch
        audioSource.pitch = maxPitch * (1 - Mathf.InverseLerp(minDistance, maxDistance, distance)) + 1;
    }
}
