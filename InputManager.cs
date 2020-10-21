using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager: MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public bool jump;
    public bool run;
    public bool shoot, aim;
    public float mouseX, mouseY;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        jump = Input.GetKeyDown(KeyCode.Space);
        run = Input.GetKey(KeyCode.LeftShift);
        shoot = Input.GetKey(KeyCode.Mouse0);
        aim = Input.GetKey(KeyCode.Mouse1);
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

    }
}
