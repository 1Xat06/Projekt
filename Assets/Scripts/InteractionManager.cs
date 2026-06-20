using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }

    public Weapon hoveredWeapon = null;
    public AmmoBox hoveredAmmoBox = null; 
    public Throwable hoveredThrowable = null;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHitByRaycast = hit.transform.gameObject;

            if (objectHitByRaycast.GetComponent<Weapon>() && !objectHitByRaycast.GetComponent<Weapon>().isActiveWeapon)
            {
                if (hoveredWeapon != null)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }

                hoveredWeapon = objectHitByRaycast.GetComponent<Weapon>();
                hoveredWeapon.GetComponent<Outline>().enabled = true; 

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupWeapon(objectHitByRaycast);
                }
            }
            else if (objectHitByRaycast.GetComponent<AmmoBox>())
            {
                if (hoveredAmmoBox != null)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;
                }

                hoveredAmmoBox = objectHitByRaycast.GetComponent<AmmoBox>();
                hoveredAmmoBox.GetComponent<Outline>().enabled = true; 

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);
                    Destroy(objectHitByRaycast); 
                }
            }
            else if (objectHitByRaycast.GetComponent<Throwable>())
            {
                if (hoveredThrowable != null)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                }

                hoveredThrowable = objectHitByRaycast.GetComponent<Throwable>();
                hoveredThrowable.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F))
                {
                    WeaponManager.Instance.PickupThrowable(hoveredThrowable);
                }
            }
            else
            {
                ClearHoveredObjects();
            }
        }
        else
        {
            ClearHoveredObjects();
        }
    }

    private void ClearHoveredObjects()
    {
        if (hoveredWeapon != null)
        {
            hoveredWeapon.GetComponent<Outline>().enabled = false;
            hoveredWeapon = null;
        }
        if (hoveredAmmoBox != null)
        {
            hoveredAmmoBox.GetComponent<Outline>().enabled = false;
            hoveredAmmoBox = null;
        }
        if (hoveredThrowable != null)
        {
            hoveredThrowable.GetComponent<Outline>().enabled = false;
            hoveredThrowable = null;
        }
    }
}   