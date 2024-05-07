using UnityEngine;

// Controls the ammo, when the bullets come out of the gun, how fast they come out of the gun, etc. 
// DOES NOT CONTROL GUN ROTATION, that's in PlayerControls 
public class GunController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    private float nextFireTime;
    public static int bulletDamage = 5;
    private readonly float delay = 1.0f;
    public static int MaxAmmo = 10000;
    public static int CurrentAmmo;
    public GunFill gunFill;
    public InventoryItemData ItemDataAmmo;
    void Awake()
    {
        CurrentAmmo = MaxAmmo;
        gunFill.SetMaxAmmo(MaxAmmo);
        gunFill.SetAmmo(CurrentAmmo);
        gunFill.fill.color = Color.clear;
    }

    void Update()
    {
        if (PlayerControls.isUsingGreen)
        {
            gunFill.FillGreen();
        }

        if ((Input.GetKey(KeyCode.JoystickButton5) || Input.GetKey(KeyCode.Mouse0)) && Time.time >= nextFireTime && PlayerControls.inventory.Contains("GreenOoze"))
        {
            Shoot();
            nextFireTime = Time.time * delay;
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
        DepleteAmmo(10);
        if (CurrentAmmo == 0)
        {
            PlayerControls.inventory.Remove("GreenOoze");
            if (PlayerControls.inventory.Contains("GreenOoze"))
            {
                Reload();
            }
            PlayerControls.PrintInventory();
        }
        Destroy(bullet, 1f);
    }

    public void DepleteAmmo(int ammoGone)
    {
        CurrentAmmo -= ammoGone;
        gunFill.SetAmmo(CurrentAmmo);
        if (CurrentAmmo <= 0)
        {
            CurrentAmmo = 0;
        }
    }

    public void Reload()
    {
        PlayerControls.isUsingGreen = true;
        gunFill.SetMaxAmmo(GunController.MaxAmmo);
        GunController.CurrentAmmo = GunController.MaxAmmo;
    }
}
