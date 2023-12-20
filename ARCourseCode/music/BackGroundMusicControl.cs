using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundMusicControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 查找所有带有“music”标签的游戏物体
        GameObject[] musicObjects = GameObject.FindGameObjectsWithTag("BGmusic");

        // 遍历每一个找到的物体，并销毁它们
        foreach (GameObject musicObject in musicObjects)
        {
            Destroy(musicObject);
        }
    }

    
}
