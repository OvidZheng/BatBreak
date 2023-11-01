using UnityEngine;


public class MovePlayer : MonoBehaviour
{
    public float moveUnit = 1f;  // 移动的单位距离
    public float timePerMoveUnit = 1;
    public LayerMask obstacleMask; //障碍物的LayerMask
    private Vector3 currentMovingDestination;
    private float curretTotalLength;
    private Vector3 moveDirection;
    private bool isStop;

    private void Start()
    {
        isStop = true;
    }

    private void FixedUpdate()
    {



    }

    private void Update()
    {
        if (isStop && ListenUserMoveInpute())
        {
            isStop = false;
        }
        
        if (isStop && LinstenUserRotateInput())
        {
            //whatever
        }
        
        if (!isStop &&  CheckArriveDestination())
        {
            transform.position = currentMovingDestination;
            isStop = true;
        }
        
        if (!isStop)
        {
            float speed = moveUnit / timePerMoveUnit;
            Vector3 newPosition = transform.position + moveDirection * speed * Time.deltaTime;
            transform.position = newPosition;
        }
    }

    private bool LinstenUserRotateInput()
    {
        // 旋转控制
        if (Input.GetKeyDown(KeyCode.Q))
        {
            transform.Rotate(0, -90f, 0, Space.Self);
            return true;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            transform.Rotate(0, 90f, 0, Space.Self);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ListenUserMoveInpute()
    {
        moveDirection = Vector3.zero;
        
        // 检测按键和障碍物，确定移动方向
        if (Input.GetKeyDown(KeyCode.W))
        {
            moveDirection = transform.forward;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            moveDirection = -transform.forward;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            moveDirection = -transform.right;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            moveDirection = transform.right;
        }
        
        if (moveDirection != Vector3.zero)
        {
            if(!IsObstacleInDirection(moveDirection))
            {
                currentMovingDestination = transform.position + moveDirection * moveUnit;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool CheckArriveDestination()
    {
        float dist = Vector3.Distance(transform.position, currentMovingDestination);
        if (dist < 0.1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // 检查指定方向上是否有障碍物
    private bool IsObstacleInDirection(Vector3 direction)
    {
        float checkDistance = moveUnit;
        return Physics.Raycast(transform.position, direction, checkDistance - 0.01f, obstacleMask);
    }
}
