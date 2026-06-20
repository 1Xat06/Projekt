using UnityEngine;
using TMPro; 
using UnityEngine.UI; 

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; set; }

    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public TextMeshProUGUI lethalAmmoUI; 
    public TextMeshProUGUI tacticalAmmoUI;

    public Image ammoTypeUI;
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;
    public Image lethalUI;
    public Image tacticalUI;

    public GameObject middleDot;

    public Sprite emptySlot; 
    public Sprite greySlot; 

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        GameObject activeWeaponSlot = WeaponManager.Instance.activeWeaponSlot;
        Weapon activeWeapon = activeWeaponSlot.GetComponentInChildren<Weapon>();

        GameObject unActiveWeaponSlot = GetUnActiveWeaponSlot();
        Weapon unActiveWeapon = unActiveWeaponSlot.GetComponentInChildren<Weapon>();

        if (activeWeapon != null) 
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft / activeWeapon.bulletsPerBurst}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}"; 

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);
            activeWeaponUI.sprite = GetWeaponSprite(model);

            if (unActiveWeapon != null)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }
            else
            {
                unActiveWeaponUI.sprite = emptySlot;
            }
        }   
        else 
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";
            ammoTypeUI.sprite = emptySlot;
            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }

        if (WeaponManager.Instance.lethalsCount <= 0)
        {
            lethalUI.sprite = greySlot;
        }

        if (WeaponManager.Instance.tacticalsCount <= 0)
        {
            tacticalUI.sprite = greySlot;
        }
    }

    private GameObject GetUnActiveWeaponSlot()
    {
        foreach (GameObject slot in WeaponManager.Instance.weaponSlots)
        {
            if (slot != WeaponManager.Instance.activeWeaponSlot)
            {
                return slot;
            }
        }
        return null;
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Pistol:
                return Resources.Load<GameObject>("Pistol_Weapon").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.AK:
                return Resources.Load<GameObject>("AK_Weapon").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Pistol:
                return Resources.Load<GameObject>("Pistol_Ammo").GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.AK:
                return Resources.Load<GameObject>("Rifle_Ammo").GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    public void UpdateThrowablesUI()
    {
        lethalAmmoUI.text = $"{WeaponManager.Instance.lethalsCount}";

        switch (WeaponManager.Instance.equippedLethalType)
        {
            case Throwable.ThrowableType.Grenade:
                lethalUI.sprite = Resources.Load<GameObject>("Grenade").GetComponent<SpriteRenderer>().sprite;
                break;
        }

        tacticalAmmoUI.text = $"{WeaponManager.Instance.tacticalsCount}";

        switch (WeaponManager.Instance.equippedTacticalType)
        {
            case Throwable.ThrowableType.Smoke_Grenade:
                tacticalUI.sprite = Resources.Load<GameObject>("Smoke_Grenade").GetComponent<SpriteRenderer>().sprite;
                break;
        }
    }
}