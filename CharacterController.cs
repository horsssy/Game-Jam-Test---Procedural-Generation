using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class CharacterController : MonoBehaviour
{
    Rigidbody rb;

    public float walkSpeed = 1f;
    public float runSpeed = 2f;
    float jumpHeight = 2f;
    float moveSpeed = 1f;

    public InputManager inputManager;
    public AnimationController animationController;
    public Transform mainCamera;
    public Transform cameraAiming;
    Vector3 movDir; // direction to move character
    bool isRunning = false;
    bool isWalking = false;
    bool isAiming = false;
    bool jump = false;
    bool jumping = false;

    Vector3 targetPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

        
        Vector3 camera2dProjectionForward = Vector3.ProjectOnPlane(cameraAiming.forward, Vector3.up);
        Vector3 camera2dProjectionRight = Vector3.ProjectOnPlane(cameraAiming.right, Vector3.up);

        movDir = camera2dProjectionForward * inputManager.vertical + camera2dProjectionRight * inputManager.horizontal;// transfering inputs from InputManager class to here
       
        jump = inputManager.jump;

        if (jump) jumping = true;

        isAiming = inputManager.aim;
       // movDir = new Vector3 (inputManager.horizontal,0f, inputManager.vertical).normalized; 

        if (movDir.magnitude < 0.1f)
        {
            isRunning = false;
            isWalking = false;
        }
        else if(inputManager.run)
        {
            isRunning = true;
            moveSpeed = runSpeed;
            isWalking = false;
        }
        else
        {
            isWalking = true;
            moveSpeed = walkSpeed;
            isRunning = false;
        }

        if (isAiming)
        {

            Ray ray = mainCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            targetPoint = ray.GetPoint(75);
        }

        animationController.isRunning = isRunning;
        animationController.isWalking = isWalking;
        animationController.jump = jump;
        animationController.lmb = inputManager.shoot;
        animationController.rmb = inputManager.aim;


    }

    bool start = true;
    void LateUpdate() 
    {
        if (isAiming)
        {
            if (start)
            {
                
                
                
                start = false;
            }
            Vector3 lookDir = Vector3.ProjectOnPlane(targetPoint - transform.position, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), 0.5f);
            


        }
        else if (isWalking || isRunning)
        {
           

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movDir),0.1f);
            start = true;
        }
        else
        {
            start = true;
        }
        

        rb.MovePosition( transform.position + (movDir.normalized * moveSpeed * Time.fixedDeltaTime));


    }

    public void SetJumpFalse()
    {
        jumping = false;
    }

}
