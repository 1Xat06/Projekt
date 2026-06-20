using UnityEngine;

public class Zombie : MonoBehaviour
{
    public GameObject zombieHand;
    public int zombieDamage;

    private void Start()
    {
        zombieHand.GetComponent<ZombieHand>().damage = zombieDamage;
    }
}    