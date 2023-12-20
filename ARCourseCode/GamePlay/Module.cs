using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public GirdGenerate grid;

    public bool isYuanLao;

    public bool isStepped;

    public bool isTimeBiggerThanOne;

    [SerializeField]
    public GameObject player;

    private float timer = 0;

   
    public bool isPlayAudio;

    private AudioSource selectAudio;

    // Start is called before the first frame update
    void Start()
    {
        
        isStepped = false;
        player = GameObject.FindGameObjectWithTag("Player"); //直接用tag名直接索取。
        selectAudio = GetComponent<AudioSource>();

       
    }

    private void Update()
    {
        
        timer += Time.deltaTime;
        //这里是所有选择框没有选到的情况

        if (!isYuanLao)
        {
            //当这个module是后生成的，并且两个选取框都没选取他时
            if (Module_Reflection.instant.moduleInA != this && Module_Reflection.instant.moduleInB != this)
            {
                Destroy(this.gameObject);
            }
        }

        //highLight更改
        if (Module_Reflection.instant.moduleInA != this && Module_Reflection.instant.moduleInB != this)
        {
            for (int i = 0; i < this.gameObject.transform.childCount; i++)
            {
                this.gameObject.transform.GetChild(i).gameObject.layer = 0;
            }
        }


        /*
        //只要不在脚下list里，就自动取消isStepped
        if (Player.instance.moduleJiaoXiaList[0] != this && Player.instance.moduleJiaoXiaList[1] != this)
        {
            this.isStepped = false;
        }

        */

        /*
        //实时计算模块与玩家的距离，如果大于n，则不在玩家脚下。

        if (Vector3.Distance(transform.position , player.transform.position) > 1.2)
        {
            isJiaoXia = false;
        }
        */


        //计时器。
        if (!isYuanLao)
        {
            

            isTimeBiggerThanOne = timer > 1f ? true : false;
        }

        //当计时器大于1秒，小人才可以移动上楼。
        if (isYuanLao)
        {
            isTimeBiggerThanOne = true;
        }

        //当A接触到选取元老并且B中不是元老，播放音效。同理BA
        if ( isYuanLao && !isPlayAudio && Module_Reflection.instant.moduleInA == this && Module_Reflection.instant.moduleInB != this)
        {
            selectAudio.Play();
            isPlayAudio = true;
        }
        else if (isYuanLao && !isPlayAudio && Module_Reflection.instant.moduleInB == this && Module_Reflection.instant.moduleInA != this)
        {
            selectAudio.Play();
            isPlayAudio = true;

        }

        if (Module_Reflection.instant.moduleInA != this && Module_Reflection.instant.moduleInB != this)
        {
            isPlayAudio = false;
        }
    }


}