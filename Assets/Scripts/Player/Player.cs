using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float HP = 100f;

    public TextMeshProUGUI playerHPUI;

    public GameObject gameOverUI;

    public bool isDead;

    private void Start()
    {
        
        playerHPUI.text = $"HP:{HP}";
        isDead = false ;
    }

    public void TakeDamage(float damageAmount)
    {
        HP -= damageAmount;

        if (HP <= 0)
        {
            print("Player die");
            PlayerDead();
            playerHPUI.gameObject.SetActive(false);

            GetComponent<ScreenFade>().StartFade();
            StartCoroutine(ShowGameOverUI());
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerDeath);
        }
        else
        {
            print("Player hit");
            StartCoroutine(BloodyScreenEffect());
            SoundManager.Instance.playerChannel.PlayOneShot(SoundManager.Instance.playerHurt);
            playerHPUI.text = $"HP:{HP}";
        }
    }

    private void PlayerDead()
    {
        isDead = true;
        GetComponentInChildren<MouserMovement>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        int indexPlayerDeath = UnityEngine.Random.Range(0, 2);
        Animator animator = GetComponentInChildren<Animator>();
        animator.enabled = true;
        animator.SetInteger("indexPlayerDeath", indexPlayerDeath);
    }

    private IEnumerator BloodyScreenEffect()
    {
        
        GameObject bloodyScreen = ObjectPool.instance.GetBloodyScreen();
        //GameObject bloodyScreen = GetComponent<BloodyScreenInstantiate>().InstantiateBloodyScreen();
        
        //if (!bloodyScreen.activeInHierarchy)
        //{
        //    bloodyScreen.SetActive(true);
        //}

        var image = bloodyScreen.GetComponentInChildren<Image>();

        // Set the initial alpha value to 1 (fully visible).
        Color startColor = image.color;
        startColor.a = 1f;
        image.color = startColor;

        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Calculate the new alpha value using Lerp.
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);

            // Update the color with the new alpha value.
            Color newColor = image.color;
            newColor.a = alpha;
            image.color = newColor;

            // Increment the elapsed time.
            elapsedTime += Time.deltaTime;

            yield return null; ; // Wait for the next frame.
        }

        ObjectPool.instance.ReturnBloodyScreen(bloodyScreen);

        //if (bloodyScreen.activeInHierarchy)
        //{
        //    bloodyScreen.SetActive(false);
        //}
    }

    private IEnumerator ShowGameOverUI()
    {
        yield return new WaitForSeconds(1f);
        gameOverUI.gameObject.SetActive(true);

        int score = GlobalReferences.Instance.bestWaveToSave - 1;
        
        if (score > PlayerPrefs.GetInt(SaveLoadManager.Instance.highScoreKey))
        {
            SaveLoadManager.Instance.SaveHighScore(score);
        }

        StartCoroutine(ReturnToMainMenu());
    }

    private IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene("MainMenu");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (other.CompareTag("ZombieHand"))
            {
                TakeDamage(other.gameObject.GetComponent<ZombieHand>().damage);
            }
        }
    }
}
