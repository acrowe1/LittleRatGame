using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 20f;
    public static int bulletDamage = 1;
    private float nextFireTime;
    private float delay = 1.0f;

    void Update()
    {
        if ((Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.Mouse0)) || Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime && PlayerControls.inventory.Contains("GreenOoze"))
        {
            Shoot();
            nextFireTime = Time.time * delay;
        }
        else if ((Input.GetKey(KeyCode.JoystickButton4) || Input.GetKey(KeyCode.Mouse0)) && !PlayerControls.inventory.Contains("GreenOoze"))
        {
            Debug.Log("You need Green Ooze ammo first!");
        }
    }

    void Shoot()
    {
        Vector3 offset = new Vector3(2.5f, 1.7f, 18.2099991f);
        Vector3 spawnPosition = firePoint.position + firePoint.right * offset.x + firePoint.up * offset.y;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = firePoint.right * bulletSpeed;
        }
        Destroy(bullet, 1f);
    }


}
