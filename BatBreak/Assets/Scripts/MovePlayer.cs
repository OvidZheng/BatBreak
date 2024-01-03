using Unity.Netcode;
using UnityEngine;


public class MovePlayer : NetworkBehaviour
{
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 200.0f;
    void Update()
    {
        
        if (IsOwner)
        {
            HandleInput();
        }
    }
    
    void HandleInput()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = moveHorizontal * transform.right + moveVertical * transform.forward;
        MoveServerRpc(movement);

        float rotate = 0.0f;
        if (Input.GetKey(KeyCode.Q))
            rotate = -1.0f;
        else if (Input.GetKey(KeyCode.E))
            rotate = 1.0f;

        RotateServerRpc(rotate);
    }
    
    [ServerRpc]
    void MoveServerRpc(Vector3 movement)
    {
        transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
    }

    [ServerRpc]
    void RotateServerRpc(float rotate)
    {
        transform.Rotate(Vector3.up, rotate * rotateSpeed * Time.deltaTime);
    }
}
