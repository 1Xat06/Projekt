using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum ShootingMode { Single, Burst, Auto }
    public enum WeaponModel { Pistol, AK }

    [Header("Spawn Settings")]
    public Vector3 spawnPosition;
    public Vector3 spawnRotation;

    [Header("References")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public GameObject muzzleEffect;
    internal Animator animator; 

    [Header("Weapon Properties")]
    public bool isActiveWeapon;
    public WeaponModel thisWeaponModel; 
    public ShootingMode currentShootingMode; 
    public float bulletVelocity = 100f; 
    public float bulletLifeTime = 3f;
    public float shootingDelay = 0.3f; 
    public int weaponDamage;

    [Header("Spread Settings")]
    public float spreadIntensity;
    public float hipSpreadIntensity = 0.4f;
    public float adsSpreadIntensity = 0.04f;
    
    [Header("Burst Settings")]
    public int bulletsPerBurst = 3; 
    private int burstBulletsLeft; 

    [Header("Reloading Settings")]
    public float reloadTime = 1.5f; 
    public int magazineSize = 7; 
    public int bulletsLeft; 
    public bool isReloading; 

    [Header("ADS Settings")]
    public bool isADS;

    private bool isShooting;
    private bool readyToShoot;
    private bool allowReset = true;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
        spreadIntensity = hipSpreadIntensity;
    }
       
    void Update()
    {
        if (isActiveWeapon)
        {
            gameObject.layer = LayerMask.NameToLayer("WeaponRender");
            
            if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = false;

            if(GetComponent<Outline>() != null) GetComponent<Outline>().enabled = false;

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                EnterADS();
            }
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                ExitADS();
            }

            if (currentShootingMode == ShootingMode.Auto) isShooting = Input.GetKey(KeyCode.Mouse0); 
            else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst) isShooting = Input.GetKeyDown(KeyCode.Mouse0); 

            if (readyToShoot && isShooting && bulletsLeft > 0)
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }

            if (bulletsLeft == 0 && isShooting) SoundManager.Instance.emptyMagazineSound.Play();
            
            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
            {
                Reload();
            }
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            
            if (GetComponent<Collider>() != null) GetComponent<Collider>().enabled = true;
        }
    }

    private void EnterADS()
    {
        if (animator != null) animator.SetTrigger("enterADS");
        isADS = true;
        spreadIntensity = adsSpreadIntensity;
        HUDManager.Instance.middleDot.SetActive(false);
    }

    private void ExitADS()
    {
        if (animator != null) animator.SetTrigger("exitADS");
        isADS = false;
        spreadIntensity = hipSpreadIntensity;
        HUDManager.Instance.middleDot.SetActive(true);
    }

    private void FireWeapon()
    {
        readyToShoot = false;
        bulletsLeft--;

        if (muzzleEffect != null) muzzleEffect.GetComponent<ParticleSystem>().Play();

        if (isADS)
        {
            if (animator != null) animator.SetTrigger("RECOIL_ADS");
        }
        else
        {
            if (animator != null) animator.SetTrigger("RECOIL");
        }

        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().bulletDamage = weaponDamage;
        bullet.transform.forward = shootingDirection; 
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        if (currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private void Reload()
    {
        isReloading = true;
        if (animator != null) animator.SetTrigger("RELOAD");
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        Invoke("ReloadCompleted", reloadTime); 
    }

    private void ReloadCompleted()
    {
        int ammoReserve = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
        int bulletsToReload = magazineSize - bulletsLeft; 

        if (ammoReserve >= bulletsToReload)
        {
            bulletsLeft = magazineSize; 
            WeaponManager.Instance.DecreaseTotalAmmo(bulletsToReload, thisWeaponModel); 
        }
        else
        {
            bulletsLeft += ammoReserve; 
            WeaponManager.Instance.DecreaseTotalAmmo(ammoReserve, thisWeaponModel); 
        }

        isReloading = false; 
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

  private Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit)) 
        {
            targetPoint = hit.point;
            
        }
        else 
        {
            targetPoint = ray.GetPoint(100); 
        }

        Vector3 direction = targetPoint - bulletSpawn.position;
        
        float x = Random.Range(-spreadIntensity, spreadIntensity);
        float y = Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}