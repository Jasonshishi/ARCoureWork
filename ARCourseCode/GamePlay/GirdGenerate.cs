using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirdGenerate : MonoBehaviour
{

    public Vector3Int Size = new Vector3Int(7,7,7);


    public Module[,,] grid;

    public Material gridMaterial;

    public Material bloomGridMaterial;

    private Vector3 offset;

    public Module[,,] Grid
    {
        get
        {
            return grid;
        }
    }

    public Module[] drawList = new Module[16];

    private void Start()
    {
        //确定偏移位置 转坐标用
        offset = new Vector3((-Size.x / 2) , -Size.y / 2, -Size.z / 2);


        grid = new Module[Size.x, Size.y, Size.z];
        Debug.Log(grid.Length);

        //开局生成指定模块
        foreach (Module module in GetComponentsInChildren<Module>())
        {

            WorldToGrid(module.transform.position);
            InitialModule(module.transform.position, module);
            //   Debug.Log(module.transform.position);

        }
    }

    //判断是否在4x4的范围内
    public bool InBounds(Vector3Int p)

    {
        //这里设成》=会有一个bug，就是当module没越界，getmodule返回不了null 所以有问题。
        return p.x >= 0 && p.x <= grid.GetLength(0)-1 && p.y >= 0 && p.y <= grid.GetLength(1)-1 && p.z >= 0 && p.z <= grid.GetLength(2)-1;
    }


    public Vector3Int WorldToGrid(Vector3 position)
    {
        return new Vector3Int(Mathf.RoundToInt(position.x - offset.x), Mathf.RoundToInt(position.y - offset.y), Mathf.RoundToInt(position.z - offset.z));
    }


    //判断给定的位置在不在边界内，如果在，则返回该位置在grid中的位置，方便后面在指定位置生成。
    public Module GetModule(Vector3 position)
    {
        Vector3Int p = WorldToGrid(position);
        if (!InBounds(p))
        {
            return null;
        }
        if (grid[p.x, p.y, p.z] != null)
        {
            return grid[p.x, p.y, p.z];
        }
        else
        {
            return null;
        }
        
    }

    //专门给人物移动设定的一个方法，用来检测人物四周是否过了界 虽然很不优雅，但是能用。
    public bool IsBeyondBoundary(Vector3 position)
    {
        Vector3Int p = WorldToGrid(position);

        return (p.x < 0 || p.x > grid.GetLength(0) - 1 || p.y < 0 || p.y > grid.GetLength(1) - 1 || p.z < 0 || p.z > grid.GetLength(2) - 1);
    }

    public void InitialModule(Vector3 position, Module Module)
    {
        //首先将输入position世界坐标转换为自身坐标存进p
        Vector3Int p = WorldToGrid(position);
        if (!InBounds(p))
        {
            //判断一下p的位置是否在规定范围内
            return;
        }

        Module.transform.position = new Vector3(Mathf.Round(Module.transform.position.x), Mathf.Round(Module.transform.position.y), Mathf.Round(Module.transform.position.z));

        Module.grid = this;
        Module.transform.SetParent(transform);
        grid[p.x, p.y, p.z] = Module;

        //只有一开始就在子级里的bro才有幸被选为元老
        Module.isYuanLao = true;


    }


    //在指定格子上生成碎片
    //追加：这个函数还要承担 如果已有module并且不是yuanlao则删除本来module 这个功能适用于 A移动到另一个元老module，B不动。就要先destroy再新建。
    public void SetModule(Vector3 position, Module Module)
    {
        //首先将输入position世界坐标转换为自身坐标存进p
        Vector3Int p = WorldToGrid(position);
        if (!InBounds(p))
        {
            //判断一下p的位置是否在规定范围内
            return;
        }



        if (grid[p.x, p.y, p.z] == null)
        {
            //Module.transform.position = new Vector3(Mathf.Round(Module.transform.position.x), 0f, Mathf.Round(Module.transform.position.z));
            Module newModule;
            newModule = Instantiate(Module, new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z)), Module.transform.rotation, transform);

            //Module.grid = this;
            //Module.transform.SetParent(transform);
            //在指定位置生成函数里指定的Module 并且链接到父系
            grid[p.x, p.y, p.z] = newModule;
            grid[p.x, p.y, p.z].isYuanLao = false;
            grid[p.x, p.y, p.z].isTimeBiggerThanOne = false;

            /*
            //调整玩家的面朝方向
            Vector3 pos = Vector3.Lerp(Player.instance.transform.position, newModule.transform.position, 0.03f);
            Player.instance.transform.GetChild(0).LookAt(pos);
            */
        }




    }
    public void ChangeModule(Vector3 position, Module newModule)
    {

        Vector3Int p = WorldToGrid(position);
        if (!InBounds(p))
        {
            //判断一下p的位置是否在规定范围内
            return;
        }

        if (grid[p.x, p.y, p.z] != null)
        {
            DeleteModule(position);
            Module replaceModule;
            replaceModule = Instantiate(newModule, new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z)), newModule.transform.rotation, transform);

            //Module.grid = this;
            //Module.transform.SetParent(transform);
            //在指定位置生成函数里指定的Module 并且链接到父系
            grid[p.x, p.y, p.z] = replaceModule;
            grid[p.x, p.y, p.z].isYuanLao = false;
            grid[p.x, p.y, p.z].isTimeBiggerThanOne = false;


            /*
            //调整玩家的面朝方向
            Vector3 pos = Vector3.Lerp(Player.instance.transform.position, newModule.transform.position, 0.3f);
            Player.instance.transform.GetChild(0).LookAt(pos);
            */
        }

    }


    public Module[] ModuleNearby(Module targetMoudle)
    {

        //所有正面一层的8视图
        Module forward_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.forward);

        Module back_module = (IsBeyondBoundary(targetMoudle.transform.position - Vector3.forward)) ? null : GetModule(targetMoudle.transform.position - Vector3.forward);
        Module left_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.left)) ? null : GetModule(targetMoudle.transform.position + Vector3.left);
        Module right_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.right)) ? null : GetModule(targetMoudle.transform.position + Vector3.right);
        Module fright_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.right + Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.right + Vector3.forward);
        Module bright_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.right - Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.right - Vector3.forward);
        Module fleft_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.left + Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.left + Vector3.forward);
        Module bleft_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.left - Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.left - Vector3.forward);





        //所有底下一层的module的9个方位
        Module dforward_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.forward + Vector3.down)) ? null : GetModule(targetMoudle.transform.position + Vector3.forward + Vector3.down);
        Module dback_module = (IsBeyondBoundary(targetMoudle.transform.position - Vector3.forward + Vector3.down) ? null : GetModule(targetMoudle.transform.position - Vector3.forward + Vector3.down));
        Module dleft_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.left + Vector3.down)) ? null : GetModule(targetMoudle.transform.position + Vector3.left + Vector3.down);
        Module dright_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.right + Vector3.down)) ? null : GetModule(targetMoudle.transform.position + Vector3.right + Vector3.down);
        Module dfright_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.right + Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.right + Vector3.forward);
        Module dbright_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.right - Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.right - Vector3.forward);
        Module dfleft_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.left + Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.left + Vector3.forward);
        Module dbleft_module = (IsBeyondBoundary(targetMoudle.transform.position + Vector3.left - Vector3.forward)) ? null : GetModule(targetMoudle.transform.position + Vector3.left - Vector3.forward);


        drawList[0] = forward_module;
        drawList[1] = back_module;

        return drawList;

    }


    //删除操作
    public void DeleteModule(Vector3 position)
    {
        Vector3Int p = WorldToGrid(position);
        if (!InBounds(p))
        {
            //判断一下p的位置是否在规定范围内
            return;
        }
        if (grid[p.x, p.y, p.z] != null)
        {
            
            Destroy(grid[p.x, p.y, p.z].gameObject);
        }
        grid[p.x, p.y, p.z] = null;

    }



  /*
    //格子可视化
    public void OnDrawGizmos()
    {
     if (grid == null)
     {
      return;
     }
           for (int k = 0; k < grid.GetLength(2); k++)
           {
      for (int i = 0; i < grid.GetLength(1); i++)
      {
       for (int j = 0; j < grid.GetLength(0); j++)
       {
        Gizmos.color = ((grid[j, i , k] == null) ? Color.black : Color.white);
        Gizmos.DrawWireCube(new Vector3(j + offset.x, i + offset.y , k + offset.z), Vector3.one * 0.9f);
       }

      }
     }

    }
  */

    /*

    //
    private void OnRenderObject()
    {
     ModuleNearby(Module_Reflection.instant.moduleInA);
     ModuleNearby(Module_Reflection.instant.moduleInB);


     int gridSize = 4;
     //和gizmo对齐
     float cellSize = 1.05f;


     if (gridMaterial == null)
     {
      Debug.LogError("还没材质");
      return;
     }


     gridMaterial.SetPass(0);

     //画线行
     GL.PushMatrix();
     GL.Begin(GL.LINES);

     Color gridColor = Color.black;
     GL.Color(gridColor);

           //和gizmo的生成方法不一样的是，这个要每一个格子都要画出具体线段。
           for (int i = 0; i < drawList.Length; i++)
           {
               if (drawList[i] != null)
               {
       Debug.Log(drawList[i]);
       Vector3 center = drawList[i].transform.position;
       Debug.Log(center);
       Vector3 gameGridOffset = new Vector3(offset.x - 0.5f, offset.y - 0.5f, offset.z - 0.5f);

       // 竖线
       GL.Vertex(new Vector3(0,0,0));
       GL.Vertex(new Vector3(1,1,1));

       // 横线
       GL.Vertex(new Vector3((gameGridOffset.x) * cellSize, center.y + gameGridOffset.y, (center.z + gameGridOffset.z) * cellSize));
       GL.Vertex(new Vector3((gridSize + gameGridOffset.x) * cellSize, center.y + gameGridOffset.y, (center.z + gameGridOffset.z) * cellSize));

       //立线
       //GL.Vertex(new Vector3(0 * cellSize, 0, k * cellSize));
       //GL.Vertex(new Vector3(gridSize * cellSize, 0, k * cellSize));
      }


     }



     GL.End();
     GL.PopMatrix();
    }

  



    //格子游戏内可视化，用GL来做
    private void OnRenderObject()
   {

        int gridSize = 5;
        //和gizmo对齐
        float cellSize = 1.05f;


        if (gridMaterial == null)
        {
            Debug.LogError("还没材质");
            return;
        }


        gridMaterial.SetPass(0);

        //画线行
        GL.PushMatrix();
        GL.Begin(GL.LINES);

        Color gridColor = Color.green;
        GL.Color(gridColor);

        //和gizmo的生成方法不一样的是，这个要每一个格子都要画出具体线段。
        for (int k = 0; k < grid.GetLength(2) + 1; k++)
        {
            for (int i = 0; i < grid.GetLength(1) + 1; i++)
            {
                for (int j = 0; j < grid.GetLength(0) + 1; j++)
                {
                    Vector3 gameGridOffset = new Vector3(offset.x - 0.5f, offset.y - 0.5f, offset.z - 0.5f);
                    // 竖线
                    GL.Vertex(new Vector3((j + gameGridOffset.x) * cellSize, i + gameGridOffset.y, (gameGridOffset.z) * cellSize));
                    GL.Vertex(new Vector3((j + gameGridOffset.x) * cellSize, i + gameGridOffset.y, (gridSize + gameGridOffset.z) * cellSize));

                    // 横线
                    GL.Vertex(new Vector3((gameGridOffset.x) * cellSize, i + gameGridOffset.y, (k + gameGridOffset.z) * cellSize));
                    GL.Vertex(new Vector3((gridSize + gameGridOffset.x) * cellSize, i + gameGridOffset.y, (k + gameGridOffset.z) * cellSize));

                    //立线
                    //GL.Vertex(new Vector3(0 * cellSize, 0, k * cellSize));
                    //GL.Vertex(new Vector3(gridSize * cellSize, 0, k * cellSize));
                }

            }
        }



        GL.End();
        GL.PopMatrix();
        if (IsBeyondBoundary(Module_Reflection.instant.selectionBoxA.transform.position) || IsBeyondBoundary(Module_Reflection.instant.selectionBoxB.transform.position))
        {
          
        }
       
    }
      */



}