using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float walkSpeed = 5f;
    // Nhân đôi trọng lực để tăng tốc độ rơi
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    private float currentHeight;

    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    // Biến xử lý ngồi
    public float crouchHeight = 1f;
    public float normalHeight = 3f;
    public float crouchSpeed = 5f;
    public float crouchTransitionSpeed = 5f; // Tốc độ ngồi và đứng dậy
    private bool isCrouching;

    // Biến xử lý chạy
    public float runSpeed = 10f;
    public float smoothTime = 0.2f; // Thời gian làm mượt chuyển đổi giữa đi sang chạy
    private float currentSpeed;
    private float speedVelocity; // Sử dụng SmoothDamp

    private bool isJumping;
    private bool isRunning;

    Vector3 velocity;
    Vector3 move;
    

    public bool isGrounded;

    bool isWalking;

    //private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    

    
    

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentHeight = normalHeight;
        isCrouching = false;
        isJumping = false;
        isRunning = false;
        currentSpeed = walkSpeed;

        
    }

    // Update is called once per frame
    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Input di chuyển
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        // Vector di chuyển
        move = transform.right * x + transform.forward * z;

        ControllerMovement();

        // Di chuyển
        controller.Move(move * currentSpeed * Time.deltaTime);


        if ( (Mathf.Abs(x) > 0.01f || Mathf.Abs(z) > 0.01f) && !isRunning && !isCrouching && !isJumping)
        {
            isWalking = true;
           
        }
        else
        {
            isWalking = false;
        }

        //lastPosition = gameObject.transform.position;

        AdjustCrossHair();
    }

    private void AdjustCrossHair()
    {
        if (isRunning || isJumping)
        {
            HUDManager.Instance.crossHair.targetSize = 170f;
            
        }
        else if (isWalking)
        {
            HUDManager.Instance.crossHair.targetSize = 100f;
            
        }
        else if (isCrouching)
        {
            HUDManager.Instance.crossHair.targetSize = 70f;
            

        }
        else
        {
            HUDManager.Instance.crossHair.targetSize = HUDManager.Instance.crossHair.normalSize;
            
        }
    }

    private void ControllerMovement()
    {

        Jump();
        
        Crouch();
        
        Run();

    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Rơi
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl) && !isRunning && !isJumping)
        {
            isCrouching = true;
            currentHeight -= crouchTransitionSpeed * Time.deltaTime;
            if (currentHeight <= crouchHeight)
            {
                currentHeight = crouchHeight;
            }
        }
        else
        {
            isCrouching = false;
            currentHeight += crouchTransitionSpeed * Time.deltaTime;
            if (currentHeight >= normalHeight)
            {
                currentHeight = normalHeight;
            }
        }

        controller.height = currentHeight;
    }

    private void Run()
    {
        float targetSpeed;
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && isWalking)
        {
            isRunning = true;
            targetSpeed = runSpeed;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothTime);
            //cameraShake.Shake();
        }
        else
        {
            isRunning = false;
            targetSpeed = walkSpeed;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedVelocity, smoothTime);
        }
    }

}
