using Cinemachine;
using System;
using UnityEngine;

public class Camera_Behavior : MonoBehaviour
{

    public CinemachineVirtualCamera AimingSetup,ThirdPersonSetup;
    
    public InputManager inputManager;
    public Transform camPos;

    public float xGain, yGain;

    public float yRecentring, xRecentering;
    float yTime, xTime;

    Vector3 currentRot;
float xRot = 0, yRot = 0;
    // Start is called before the first frame update
    void Start()
    {
        yTime = yRecentring;
        xTime = xRecentering;


    }
    private void Update()
    {
        AimingSetup.enabled = inputManager.aim;
        ThirdPersonSetup.enabled = !inputManager.aim;

        transform.position = Vector3.Lerp(transform.position, camPos.position, 0.7f);
        if (Mathf.Abs(inputManager.horizontal) + Mathf.Abs(inputManager.vertical) > 0 && !inputManager.aim && inputManager.mouseY == 0) // when character moves while not aiming 
        {
            yTime -= Time.deltaTime;
            //xTime -= Time.deltaTime;
            
        }
        else
        {
            yTime = yRecentring;
            xTime = xRecentering;
        }

        
        
        

    }

    private void LateUpdate()
    {
        xRot = inputManager.mouseX * xGain;
        yRot = inputManager.mouseY * yGain;
        currentRot = transform.rotation.eulerAngles;
        if (yTime < 0 && inputManager.mouseY == 0)
        {

            yRot = Mathf.LerpAngle(currentRot.x, 0f, 0.05f) - currentRot.x;


        }
        Vector3 newRot = new Vector3(currentRot.x + yRot, currentRot.y + xRot, currentRot.z);


        transform.rotation = Quaternion.Slerp(Quaternion.Euler( currentRot), Quaternion.Euler(newRot), 0.1f);

        
    }



}
