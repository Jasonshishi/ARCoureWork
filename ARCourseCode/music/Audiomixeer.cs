using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Audiomixeer : MonoBehaviour
{
    public AudioMixer audioMixer; // 引用你的AudioMixer
    public Slider volumeSlider;   // 引用你的Slider

    void Start()
    {
        // 设置滑动条的初始值
        volumeSlider.value = PlayerPrefs.GetFloat("masterVolume", 0.75f);
    }

    public void SetLevel(float sliderValue)
    {
        // 将Slider的值转换为音量值
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue + 0.01f) * 20);
        PlayerPrefs.SetFloat("masterVolume", sliderValue);
    }
}
