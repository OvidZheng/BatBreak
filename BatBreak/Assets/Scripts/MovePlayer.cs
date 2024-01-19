using Unity.Netcode;
using UnityEngine;

public class MovePlayer : NetworkBehaviour
{
    public float moveSpeed = 5.0f;
    public NetworkVariable<bool> moveLock = new NetworkVariable<bool>(false);
    public float rotateSpeed = 200.0f; // 用于旋转的速度
    
    private Rigidbody rb;
    private Vector3 movementInput;
    
    // 定义矩形边界的两个对角点
    public Vector3 boundaryPoint1 = new Vector3(-10.0f, 0, -10.0f);
    public Vector3 boundaryPoint2 = new Vector3(10.0f, 0, 10.0f);

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (moveLock.Value)
        {
            return;
        }

        if (IsOwner)
        {
            CollectInput();
        }
    }

    void FixedUpdate()
    {
        if (IsOwner && !moveLock.Value)
        {
            Move();
            RotateTowardsMouse();
            CheckBoundary(); // 检查边界
        }
    }

    void CollectInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movementInput = moveHorizontal * Vector3.right + moveVertical * Vector3.forward;
    }

    void Move()
    {
        // 使用 Rigidbody 进行移动
        Vector3 movement = movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        movementInput = Vector3.zero;
    }

    void RotateTowardsMouse()
    {
        // 计算鼠标位置和将其转换为世界坐标
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.nearClipPlane;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);

        // 计算朝向鼠标的方向
        Vector3 directionToMouse = mouseWorldPosition - transform.position;
        directionToMouse.y = 0; // 保持水平旋转

        // 如果方向有效，则旋转
        if (directionToMouse.sqrMagnitude > 0.0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
        }
    }
    
    // 检查并处理边界情况
    void CheckBoundary()
    {
        // 计算边界值
        float  minX = Mathf.Min(boundaryPoint1.x, boundaryPoint2.x);
        float maxX = Mathf.Max(boundaryPoint1.x, boundaryPoint2.x);
        float minZ = Mathf.Min(boundaryPoint1.z, boundaryPoint2.z);
        float maxZ = Mathf.Max(boundaryPoint1.z, boundaryPoint2.z);
        Vector3 position = rb.position;

        if (position.x > maxX)
        {
            position.x = minX + 1;
        }
        else if (position.x < minX)
        {
            position.x = maxX - 1;
        }

        if (position.z > maxZ)
        {
            position.z = minZ + 1;
        }
        else if (position.z < minZ)
        {
            position.z = maxZ - 1;
        }
        ResetForces();
        rb.position = position;
        
    }
    
    void ResetForces()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}