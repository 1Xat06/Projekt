using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public int HP = 100;
    public bool isDead;

    [Header("UI References")]
    public GameObject bloodyScreen;
    public TextMeshProUGUI playerHealthUI;
    public GameObject gameOverUI;

    private ScreenFader screenFader;

    private void Start()
    {
        screenFader = GetComponent<ScreenFader>();
        UpdateHealthUI();
    }

    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
        
        if (HP < 0) HP = 0;

        UpdateHealthUI();

        if (HP <= 0 && !isDead)
        {
            PlayerDead();
            isDead = true;
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDeath);
        }
        else if (!isDead)
        {
            StartCoroutine(BloodyScreenEffect());
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);
        }
    }

    private void UpdateHealthUI()
    {
        if (playerHealthUI != null)
        {
            playerHealthUI.text = $"Health: {HP}";
        }
    }

    private void PlayerDead()
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<MouseMovement>().enabled = false;

        Animator cameraAnimator = Camera.main.GetComponent<Animator>();
        if (cameraAnimator != null) cameraAnimator.enabled = true;

        playerHealthUI.gameObject.SetActive(false);
        screenFader.StartFade();
        StartCoroutine(ShowGameOverUI());
        
        SoundManager.Instance.playerChannel.clip = SoundManager.Instance.gameOverMusic;
        SoundManager.Instance.playerChannel.PlayDelayed(2f);

        int waveSurvived = GlobalReferences.Instance.waveNumber - 1;

        if (waveSurvived > SaveLoadManager.Instance.LoadHighScore())
        {
            SaveLoadManager.Instance.SaveHighScore(waveSurvived);
        }

        StartCoroutine(ReturnToMainMenu());
    }    

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene("Main Menu"); 
    }

    private IEnumerator BloodyScreenEffect()
    {
        if (!bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(true);
        }

        Image image = bloodyScreen.GetComponent<Image>();
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1f);
        
        float duration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            image.color = Color.Lerp(new Color(image.color.r, image.color.g, image.color.b, 1f), new Color(image.color.r, image.color.g, image.color.b, 0f), elapsedTime / duration);
            yield return null;
        }

        if (bloodyScreen.activeInHierarchy)
        {
            bloodyScreen.SetActive(false);
        }
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ZombieHand"))
        {
            if (!isDead)
            {
                int damage = other.gameObject.GetComponent<ZombieHand>().damage;
                TakeDamage(damage);
            }
        }
    }
}