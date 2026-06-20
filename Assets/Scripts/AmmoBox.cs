using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public enum AmmoType
    {
        RifleAmmo,
        PistolAmmo
    }

    public int ammoAmount = 200;
    public AmmoType ammoType;
}   