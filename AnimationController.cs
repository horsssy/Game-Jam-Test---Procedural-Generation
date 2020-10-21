using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator animator;
    public bool isRunning = false;
    public bool isWalking = false;
    public bool jump = false;
    bool isIdle;
    public bool lmb = false, rmb = false;

    bool jumpDone = true;
    bool shootEnable = true;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        isIdle = (isRunning || isWalking) ? false : true;
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isJump", jump);
        animator.SetBool("isAiming", rmb);

        if (lmb && shootEnable && isIdle && rmb) // shouldn't be moving and should be aiming
        { 
            animator.SetTrigger("Fire");

            shootEnable = false;
        }

    }

    public void DebuggingAnim()
    {
        print("Fire");
    }
    public void EnableShooting()
    {
            shootEnable = true;

    }
}
