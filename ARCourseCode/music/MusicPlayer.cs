using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{

    public AudioSource buttonAudio;

    public AudioClip buttonClip;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex < 3)
        {
            DontDestroyOnLoad(gameObject);
        }

        /*
        while (FindObjectsOfType<MusicPlayer>().Length > 0)
        {
            Destroy(FindObjectOfType<MusicPlayer>().gameObject);
        }
        */
        
        // 可选：检查场景中是否已存在另一个相同的音乐播放器并销毁它
        if ( FindObjectsOfType<MusicPlayer>().Length > 0 && PlayerPrefs.GetInt("MusicX", 0) == 1)
        {
                Debug.Log(6);
                Destroy(gameObject);
           
        }

        if (FindObjectsOfType<MusicPlayer>().Length > 1)
        {
            Destroy(gameObject);
        }

        
    }

    public void StartButtonAudio()
    {

        buttonAudio.PlayOneShot(buttonClip);

    }

   
}
