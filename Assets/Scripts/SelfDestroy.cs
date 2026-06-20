using System.Collections;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}    