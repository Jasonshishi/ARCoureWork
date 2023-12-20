using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Level1Button : MonoBehaviour
{
    public static Level1Button instance;
    public AudioClip buttonAudio;
    public AudioSource ButtonSource;


    public Image fadeImage;

    public float fadeOutTime;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayButtonAudio()
    {

        ButtonSource.PlayOneShot(buttonAudio);

    }

    public void ResetLevel()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    public void LoadLevel1()
    {
        StartCoroutine( LoadNext());
    }


    public IEnumerator LoadNext()
    {
        yield return new WaitForSeconds(1);

        if (SceneManager.GetActiveScene().buildIndex > 3)
        {
            PlayerPrefs.SetInt("MusicX", 1);
            PlayerPrefs.Save();
        }
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    public void Quit()
    {
        // 关闭游戏
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        // 在应用程序关闭时清除 PlayerPrefs 数据
        PlayerPrefs.DeleteAll();
    }

    public void FadEOut()
    {
        StartCoroutine(Fadeout());
    }

    IEnumerator Fadeout()
    {

        Color originalColor = fadeImage.color;
        float elapsedTime = 0.0f;
   
        while (elapsedTime < fadeOutTime)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeOutTime);
            fadeImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
       
    }

}
