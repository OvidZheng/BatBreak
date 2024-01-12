using Unity.Netcode;
using UnityEngine;

public class MovePlayer : NetworkBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 200.0f;

    private Vector3 movementInput;
    private float rotateInput;

    void Update()
    {
        if (IsOwner)
        {
            CollectInput();
            Move();
            Rotate();
        }
    }

    void CollectInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movementInput = moveHorizontal * transform.right + moveVertical * transform.forward;

        rotateInput = 0.0f;
        if (Input.GetKey(KeyCode.Q))
            rotateInput = -1.0f;
        else if (Input.GetKey(KeyCode.E))
            rotateInput = 1.0f;
    }

    void Move()
    {
        transform.Translate(movementInput * moveSpeed * Time.deltaTime, Space.World);
    }

    void Rotate()
    {
        transform.Rotate(Vector3.up, rotateInput * rotateSpeed * Time.deltaTime);
    }
}