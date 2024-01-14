using Unity.Netcode;
using UnityEngine;

public class MovePlayer : NetworkBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 200.0f;
    public NetworkVariable<bool> moveLock = new NetworkVariable<bool>(false);
    private bool roundInputConnected;

    private Rigidbody rb;
    private Vector3 movementInput;
    private float rotateInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        roundInputConnected = false;
    }

    void Update()
    {
        if (moveLock.Value)
        {
            return;
        }
        if (IsOwner && !roundInputConnected)
        {
            CollectInput();
            roundInputConnected = true;
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            Move();
            Rotate();
            roundInputConnected = false;
        }
    }

    void CollectInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movementInput = moveHorizontal * Vector3.right + moveVertical * Vector3.forward;

        
        if (Input.GetKey(KeyCode.Q))
            rotateInput = -1.0f;
        else if (Input.GetKey(KeyCode.E))
            rotateInput = 1.0f;
    }

    void Move()
    {
        // 使用 Rigidbody 进行移动
        Vector3 movement = movementInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
        movementInput = Vector3.zero;
    }

    void Rotate()
    {
        // 使用 Rigidbody 进行旋转
        float rotation = rotateInput * rotateSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotation);
        rb.MoveRotation(rb.rotation * deltaRotation);
        rotateInput = 0.0f;
    }
}