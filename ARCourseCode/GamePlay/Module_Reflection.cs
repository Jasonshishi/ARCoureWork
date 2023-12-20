using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_Reflection : MonoBehaviour
{

    public static Module_Reflection instant;

    public GirdGenerate grid;

    public GameObject selectionBoxA;

    public GameObject selectionBoxB;

    public GameObject selectionBoxUI_A;

    public GameObject selectionBoxUI_B;

    public int selectANum = 0;

    public int selectBNum = 0;


    public Module moduleInA;

    public Module moduleInB;

    public GameObject testGrid;

    public bool isBeyond;

    //public AudioClip selectYuanLaoAudio;

    
    //public GameObject BCubeColor;


    //有一个需求，我需要知道grid格子上是否原来就有障碍物，这样生出来的才能被识别为新生儿。

    private void Start()
    {
        instant = this;
        
    }

    // Update is called once per frame
    void Update()
    {
        SelectboxOutGridDetection();

        SelectionboxAUI();

        SelectionboxBUI();

        moduleInA = grid.GetModule(selectionBoxA.transform.position);
        moduleInB = grid.GetModule(selectionBoxB.transform.position);

        //HK到此一游

        if ((moduleInA != null && moduleInA.transform.tag == "Target_Module") || (moduleInB != null && moduleInB.transform.tag == "Target_Module"))
        {
            return;
        }
        else
        {
            if (moduleInA != null && moduleInA.isYuanLao)
            {
                for (int i = 0; i < moduleInA.gameObject.transform.childCount; i++)
                {
                    moduleInA.gameObject.transform.GetChild(i).gameObject.layer = 6;
                }
            }
            if (moduleInB != null && moduleInB.isYuanLao)
            {

                for (int i = 0; i < moduleInB.gameObject.transform.childCount; i++)
                {
                    moduleInB.gameObject.transform.GetChild(i).gameObject.layer = 6;
                }
            }

            //第一种情况，当A中有元老但B中无，直接在B的grid位置上生成一个新的A中的module
            if (moduleInA != null && moduleInA.isYuanLao && moduleInB == null)
            {
                grid.SetModule(selectionBoxB.transform.position, moduleInA);



            }
            //第二种，就是一的翻版
            else if (moduleInB != null && moduleInB.isYuanLao && moduleInA == null)
            {
                grid.SetModule(selectionBoxA.transform.position, moduleInB);

            }
            //第三种，当A中有元老，且已经映射到B了（inB不是元老），这时移动A到另一个元老，在B中产生新的映射
            else if (moduleInA != null && moduleInA.isYuanLao && moduleInB != null && !moduleInB.isYuanLao && moduleInA.tag != moduleInB.tag)
            {
                //这里产生一个问题，就是当A和B都不动的时候，会一直删除重建Module。所以要在条件里再加一个 moduleinA不等于moduleinB
                grid.ChangeModule(selectionBoxB.transform.position, moduleInA);

            }
            //第4种，反过来
            else if (moduleInB != null && moduleInB.isYuanLao && moduleInA != null && !moduleInA.isYuanLao && moduleInA.tag != moduleInB.tag)
            {
                grid.ChangeModule(selectionBoxA.transform.position, moduleInB);

            }

            //第五种情况，当A中没有，B中的不是元老，说明A已经移走，所以要清除B中的复制体
            else if (moduleInA == null && moduleInB != null && !moduleInB.isYuanLao)
            {
                grid.DeleteModule(selectionBoxB.transform.position);
            }
            //第6 反过来
            else if (moduleInB == null && moduleInA != null && !moduleInA.isYuanLao)
            {
                grid.DeleteModule(selectionBoxA.transform.position);
            }
        }

    }

    /*
    public void ModuleHightLIght()
    {
        if (moduleInA != null && moduleInA.isYuanLao)
        {
            // moduleInA.transform.GetChild(0).gameObject.SetActive(false);

            
            for (int i = 0; i < moduleInA.GetComponent<MeshRenderer>().materials.Length; i++)
            {
                moduleInA.gameObject.GetComponent<MeshRenderer>().materials[i] = highLight; //将所有材质换成指定高亮材质
                Debug.Log(moduleInA.GetComponent<MeshRenderer>().materials);
            }
            
            if (moduleInA.gameObject.transform.Find("HighLight") != null)
            {
                moduleInA.gameObject.transform.Find("HighLight").gameObject.SetActive(true);

            }


        }

        if (moduleInB == null )
        {
            return;
        }

        if (moduleInB != null && moduleInB.isYuanLao)
        {
            
          for (int i = 0; i < moduleInB.GetComponent<MeshRenderer>().materials.Length; i++)
          {
              moduleInB.gameObject.GetComponent<MeshRenderer>().materials[i] = highLight; //将所有材质换成指定高亮材质
          }
         
            if (moduleInB.gameObject.transform.Find("HighLight") != null)
            {
                moduleInB.gameObject.transform.Find("HighLight").gameObject.SetActive(true);

            }

        }

    }
      */

    public void SelectboxOutGridDetection()
    {
        if (grid.IsBeyondBoundary(selectionBoxA.transform.position) || grid.IsBeyondBoundary(selectionBoxB.transform.position))
        {
            if (testGrid != null)
            {
                testGrid.SetActive(true);
            }
             grid.bloomGridMaterial.SetFloat("_threshold",0);
        }
        else
        {
            if (testGrid != null)
            {
                testGrid.SetActive(false);
            }
            
             grid.bloomGridMaterial.SetFloat("_threshold", 1);
        }
    }

    public void SelectionboxAUI()
    {
        //首先将输入position世界坐标转换为自身坐标存进p

        Vector3Int pA = grid.WorldToGrid(selectionBoxA.transform.position);
       
        if (!grid.InBounds(pA) )
        {
            //判断一下p的位置是否在规定范围内
            return;
        }

        if (selectANum < 1)
        {
            selectANum = 1;
            Instantiate(selectionBoxUI_A, new Vector3(Mathf.Round(selectionBoxA.transform.position.x), Mathf.Round(selectionBoxA.transform.position.y), Mathf.Round(selectionBoxA.transform.position.z)), selectionBoxUI_A.transform.rotation);
            
        }

        
    }


    public void SelectionboxBUI()
    {
        //首先将输入position世界坐标转换为自身坐标存进p

        Vector3Int pB = grid.WorldToGrid(selectionBoxB.transform.position);

        if (!grid.InBounds(pB))
        {
            //判断一下p的位置是否在规定范围内
            return;
        }

        if (selectBNum < 1)
        {
            selectBNum = 1;
            Instantiate(selectionBoxUI_B , new Vector3(Mathf.Round(selectionBoxB.transform.position.x), Mathf.Round(selectionBoxB.transform.position.y), Mathf.Round(selectionBoxB.transform.position.z)), selectionBoxUI_B.transform.rotation);
            
        }
       
    }


}