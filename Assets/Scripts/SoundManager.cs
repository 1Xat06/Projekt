using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("Weapon Channels")]
    public AudioSource shootingChannel; 
    public AudioSource reloadingSoundPistol; 
    public AudioSource reloadingSoundAK; 
    public AudioSource emptyMagazineSound; 

    [Header("Weapon Clips")]
    public AudioClip pistolShot; 
    public AudioClip akShot; 

    [Header("Throwables")]
    public AudioSource throwablesChannel;
    public AudioClip grenadeSound;

    [Header("Zombie Sounds")]
    public AudioSource zombieChannel;
    public AudioSource zombieChannel2;
    public AudioClip zombieWalking;
    public AudioClip zombieChase;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;

    [Header("Player Sounds")]
    public AudioSource playerChannel;
    public AudioClip playerHurt;
    public AudioClip playerDeath;
    public AudioClip gameOverMusic;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void PlayShootingSound(Weapon.WeaponModel weapon)
    {
        switch (weapon)
        {
            case Weapon.WeaponModel.Pistol:
                shootingChannel.PlayOneShot(pistolShot); 
                break;
            case Weapon.WeaponModel.AK:
                shootingChannel.PlayOneShot(akShot);
                break;
        }
    }

    public void PlayReloadSound(Weapon.WeaponModel weapon)
    {
        switch (weapon)
        {
            case Weapon.WeaponModel.Pistol:
                reloadingSoundPistol.Play();
                break;
            case Weapon.WeaponModel.AK:
                reloadingSoundAK.Play();
                break;
        }
    }
}    