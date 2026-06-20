using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { get; set; }

    [Header("Weapon Slots")]
    public List<GameObject> weaponSlots; 
    public GameObject activeWeaponSlot; 

    [Header("Ammo Reserve")]
    public int totalRifleAmmo = 0;
    public int totalPistolAmmo = 0;

    [Header("Throwables General")]
    public float throwForce = 10f;
    public Transform throwableSpawn;
    public float forceMultiplier = 0f;
    public float forceMultiplierLimit = 2f;

    [Header("Lethals")]
    public int maxLethals = 2;
    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalType;
    public GameObject grenadePrefab;

    [Header("Tacticals")]
    public int maxTacticals = 2;
    public int tacticalsCount = 0;
    public Throwable.ThrowableType equippedTacticalType;
    public GameObject smokeGrenadePrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        equippedLethalType = Throwable.ThrowableType.None;
        equippedTacticalType = Throwable.ThrowableType.None;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchActiveSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchActiveSlot(1);

        foreach (GameObject slot in weaponSlots)
        {
            if (slot == activeWeaponSlot) slot.SetActive(true);
            else slot.SetActive(false);
        }

        if (Input.GetKey(KeyCode.G) || Input.GetKey(KeyCode.T))
        {
            forceMultiplier += Time.deltaTime;
            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            if (lethalsCount > 0)
            {
                ThrowLethal();
            }
            forceMultiplier = 0f;
        }

        if (Input.GetKeyUp(KeyCode.T))
        {
            if (tacticalsCount > 0)
            {
                ThrowTactical();
            }
            forceMultiplier = 0f;
        }
    }

    public void SwitchActiveSlot(int slotNumber)
    {
        if (activeWeaponSlot != weaponSlots[slotNumber])
        {
            activeWeaponSlot.SetActive(false); 
            activeWeaponSlot = weaponSlots[slotNumber]; 
            activeWeaponSlot.SetActive(true); 
        }
    }

    public void PickupWeapon(GameObject pickedUpWeapon)
    {
        Weapon weaponComponent = pickedUpWeapon.GetComponent<Weapon>();

        if (activeWeaponSlot.transform.childCount > 0)
        {
            DropCurrentWeapon(pickedUpWeapon);
        }

        pickedUpWeapon.transform.SetParent(activeWeaponSlot.transform); 
        pickedUpWeapon.transform.localPosition = weaponComponent.spawnPosition;
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weaponComponent.spawnRotation);

        weaponComponent.isActiveWeapon = true; 
        if (weaponComponent.animator != null) weaponComponent.animator.enabled = true; 
    }

    private void DropCurrentWeapon(GameObject pickedUpWeapon)
    {
        GameObject weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;
        Weapon weaponComponent = weaponToDrop.GetComponent<Weapon>();

        weaponComponent.isActiveWeapon = false;
        if (weaponComponent.animator != null) weaponComponent.animator.enabled = false;

        weaponToDrop.transform.SetParent(pickedUpWeapon.transform.parent);
        weaponToDrop.transform.position = pickedUpWeapon.transform.position;
        weaponToDrop.transform.rotation = pickedUpWeapon.transform.rotation;
    }

    public void PickupAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmo += ammo.ammoAmount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmo += ammo.ammoAmount;
                break;
        }
    }

    public void DecreaseTotalAmmo(int bulletsToDecrease, Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.AK:
                totalRifleAmmo -= bulletsToDecrease;
                break;
            case Weapon.WeaponModel.Pistol:
                totalPistolAmmo -= bulletsToDecrease;
                break;
        }
    }

    public int CheckAmmoLeftFor(Weapon.WeaponModel thisWeaponModel)
    {
        switch (thisWeaponModel)
        {
            case Weapon.WeaponModel.AK:
                return totalRifleAmmo;
            case Weapon.WeaponModel.Pistol:
                return totalPistolAmmo;
            default:
                return 0;
        }
    }

    public void PickupThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                PickupThrowableAsLethal(Throwable.ThrowableType.Grenade);
                break;
            case Throwable.ThrowableType.Smoke_Grenade:
                PickupThrowableAsTactical(Throwable.ThrowableType.Smoke_Grenade);
                break;
        }
    }

    private void PickupThrowableAsLethal(Throwable.ThrowableType lethal)
    {
        if (equippedLethalType == lethal || equippedLethalType == Throwable.ThrowableType.None)
        {
            equippedLethalType = lethal;

            if (lethalsCount < maxLethals)
            {
                lethalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
        }
    }

    private void PickupThrowableAsTactical(Throwable.ThrowableType tactical)
    {
        if (equippedTacticalType == tactical || equippedTacticalType == Throwable.ThrowableType.None)
        {
            equippedTacticalType = tactical;

            if (tacticalsCount < maxTacticals)
            {
                tacticalsCount += 1;
                Destroy(InteractionManager.Instance.hoveredThrowable.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
        }
    }

    private void ThrowLethal()
    {
        GameObject lethalPrefab = GetThrowablePrefab(equippedLethalType);
        GameObject throwable = Instantiate(lethalPrefab, throwableSpawn.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        
        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        lethalsCount -= 1;

        if (lethalsCount <= 0)
        {
            equippedLethalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }

    private void ThrowTactical()
    {
        GameObject tacticalPrefab = GetThrowablePrefab(equippedTacticalType);
        GameObject throwable = Instantiate(tacticalPrefab, throwableSpawn.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();
        
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);
        
        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        tacticalsCount -= 1;

        if (tacticalsCount <= 0)
        {
            equippedTacticalType = Throwable.ThrowableType.None;
        }

        HUDManager.Instance.UpdateThrowablesUI();
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case Throwable.ThrowableType.Grenade:
                return grenadePrefab;
            case Throwable.ThrowableType.Smoke_Grenade:
                return smokeGrenadePrefab;
        }
        return new GameObject();
    }
}    