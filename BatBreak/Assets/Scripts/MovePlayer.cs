using Unity.Netcode;
using UnityEngine;

public class MovePlayer : NetworkBehaviour
{
    public float moveSpeed = 5.0f;
    public NetworkVariable<bool> moveLock = new NetworkVariable<bool>(false);
    public float rotateSpeed = 200.0f; // 用于旋转的速度
    
    private Rigidbody rb;
    private Vector3 movementInput;

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
        if (IsOwner)
        {
            Move();
            RotateTowardsMouse();
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
}