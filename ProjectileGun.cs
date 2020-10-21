using UnityEngine;
using TMPro;

public class ProjectileGun : MonoBehaviour
{
    //bullet
    public GameObject bullet;

    //bullet force
    public float shootForce, upwardForce;

    //gun stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera tpsCam;
    public Transform attackPoint;

    //Graphics(may want to change this later)
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammoDisplay;

    //bug fixing
    public bool allowInvoke = true;

    private void Awake(){
        //make sure mag is full
        bulletsLeft = magSize;
        readyToShoot = true;
    }

    private void Update(){
        MyInput();

        //Set ammo display if it exists
        if(ammoDisplay != null){
            ammoDisplay.SetText(bulletsLeft / bulletsPerTap + " / " + magSize / bulletsPerTap);
        }
    }

    private void MyInput(){
        //Check if allowed to hold down button
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Reloading
        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < magSize && !reloading) Reload();

        //Reload automatically if shooting without ammo
        if(readyToShoot && shooting && !reloading && bulletsLeft <=0) Reload();

        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0){
            //Set bullets shot to 0
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot(){
        readyToShoot = false;
        //Find exact hit position using raycast(may need to adjust for third person)
        Ray ray = tpsCam.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;
        //check if ray hits something
        Vector3 targetPoint;
        if(Physics.Raycast(ray, out hit)){
            targetPoint = hit.point;
        }
        else{
            targetPoint = ray.GetPoint(75); //just a point far away from player
        }
        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate new direction with spread
        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0); //Add spread

        //Instantiate bullet/projectile(we can make custom ones in the future)
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);//new bullet will be instantly stored to this
        //Rotate bullet to shoot direction
        currentBullet.transform.forward = directionWithSpread.normalized;

        //Add forces to bullet
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
        //currentBullet.GetComponent<Rigidbody>().AddForce(tpsCam.transform.up * upwardForce, ForceMode.Impulse); //use upward force if we want bouncing grenades or something
        bulletsLeft--;
        bulletsShot++;

        //Instantiate muzzle flash if we want one
        if(muzzleFlash !=null)
            Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        //Invoke resetShot function (if not already invoked)
        if(allowInvoke){
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }

        //if more than on bulletsPerTap make sure to repeat shoot function
        if(bulletsShot < bulletsPerTap && bulletsLeft > 0){
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot(){
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload(){
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished(){
        bulletsLeft = magSize;
        reloading = false;
    }
}
