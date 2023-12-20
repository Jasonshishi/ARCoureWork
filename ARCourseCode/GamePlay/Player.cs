using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.SceneManagement;


public class Player : MonoBehaviour
{


	//bug解决：
	/*
	 * 1.现在的问题就是当玩家处在三个连续的块的时候 还是会反复横跳，isStepped没起作用，但是只有两个块就没事
	 * 解决：//moduleJiaoXiaList[0].isStepped = false; 这一行的本意是让原来的脚下的step变成false，但是会导致update里变成false的同时走回去。
				moduleJiaoXiaList.RemoveAt(0);
	        这个方法行不通：这里就要再加一个新的更改stepped状态的方法，就是所有不在moduleJiaoXiaList的module的stepped值变为false。
	        再想一个，创建一个3位的列表，让列表的第一个是专门给改成false用的(有用)

	 * 
	 * 2.当玩家走楼梯时，会在登顶后返回到楼梯中央。
	 * 解决：将probuider的楼梯替换，之前会有碰撞问题
	 * 
	 * 
	 * 3.当玩家走到下一块时，如果三面没有块且背后是楼梯，会返回楼梯那格子
	 * 解决：暂时bug没复现
	 * 
	 * 
	 * 4.当玩家跌落后，按照一开始的逻辑回到【3】，可能会遇到3是clonemodule并且已经被销毁的情况。
	 * 解决：试图遍历一下所有的列表值，直到遇到最近的元老。
	 * 
	 * 
	 * 5.在获取module的rotation来做楼梯朝向时，会发现获取不到特定角度，会有问题。
	 * 解决：直接获取的rotation是radian的形式，必须先转一步270 * Mathf.Deg2Rad
	 * 
	 * 
	 * 6.如果一个module出局，就影响大家。
	 * 
	 * 
	 * 
	 * 7.到达不了终点target
	 * 
	 * 8.无限生成外面框bug
	 * 解决：解决inbound的上限
	 * 
	 * 9.生成不了楼梯 
	 * 解决：朝向问题，如果单纯让方向改为90度不好，要给订一个范围。Mathf.Abs( forward_module.transform.rotation.eulerAngles.y ) < 0.1f
	 */

	public static Player instance;

	public GirdGenerate grid;

	
	[HideInInspector]
	public LayerMask ModuleMask;

	Collider player_collider;
	
	private Module module_hit;

	[SerializeField]
	public List<Module> moduleJiaoXiaList = new List<Module>();

	public int levelNum ;

	//public Module yuanZhong1;

	//public Module yuanZhong2;

	private void Start()
    {
		instance = this;
		player_collider = GetComponent<Collider>();
		//moduleJiaoXiaList.Add(grid.GetModule(Vector3.one));
		//moduleJiaoXiaList.Add(yuanZhong1);
		//moduleJiaoXiaList.Add(yuanZhong2);
		TextAnimation.instance.fullText = "Shaking, floating, disorganized, space here seems unstable.";
		Debug.Log("碰到了" + TextAnimation.instance.fullText);
		TextAnimation.instance.TypeInn();


	}

    private void Update()
    {
		/*
		//因为射线检测的原因，必须让player一直看向前方且前方是刚刚产生的module,暂时放弃这个方法

		//Ray ray = new Ray(transform.position + vector.normalized * 0.5f + Vector3.up * 2f, Vector3.down);    //做了射线检测
		RaycastHit raycastHit ;
		if (Physics.SphereCast(transform.position, 0.3f,Vector3.forward, out raycastHit, 1f, ModuleMask)) // 当在球形射线检测范围内有module时
		{
			 module_hit = raycastHit.collider.GetComponent<Module>();
			if ((module_hit == null || module_hit.grid != null) && !module_hit.isJiaoXia )
			{
				Debug.Log(module_hit.transform.tag);
				//用协程实现一点点动画 因为每个module的size是1
				StartCoroutine(AnimateGetUp(new Vector3(module_hit.transform.position.x, module_hit.transform.position.y+1f, module_hit.transform.position.z)  ));
				module_hit.isJiaoXia = true;
			}
			return;
		}
		*/
		

		PlayerMove();
	}

	/*
    private void OnCollisionEnter(Collision collision)
    {
		Module hit = collision.collider.GetComponent<Module>();
        if ((hit == null || hit.grid != null) && !hit.isJiaoXia)
        {
			Debug.Log(hit.transform.tag);
			//用协程实现一点点动画 因为每个module的size是1
			StartCoroutine(AnimateGetUp(new Vector3(hit.transform.position.x, hit.transform.position.y + 1f, hit.transform.position.z)));
			hit.isJiaoXia = true;
		}
	}
	*/
	public void PlayerMove()
    {


		//在player正前方的module
		Module forward_module = (grid.IsBeyondBoundary(transform.position + Vector3.forward)) ? null : grid.GetModule(transform.position + Vector3.forward);
		Module back_module = (grid.IsBeyondBoundary(transform.position - Vector3.forward)) ? null : grid.GetModule(transform.position - Vector3.forward);
		Module left_module = (grid.IsBeyondBoundary(transform.position + Vector3.left)) ? null : grid.GetModule(transform.position + Vector3.left);
		Module right_module = (grid.IsBeyondBoundary(transform.position + Vector3.right)) ? null:grid.GetModule(transform.position + Vector3.right);

		//脚下的块
		Module d_module = (grid.IsBeyondBoundary(transform.position + Vector3.down)) ? null : grid.GetModule(transform.position + Vector3.down);


		//所有底下一层的module的4个方位
		Module dforward_module = (grid.IsBeyondBoundary(transform.position + Vector3.forward + Vector3.down)) ? null : grid.GetModule(transform.position + Vector3.forward +Vector3.down);
		Module dback_module = (grid.IsBeyondBoundary(transform.position - Vector3.forward + Vector3.down) ? null : grid.GetModule(transform.position - Vector3.forward + Vector3.down));
		Module dleft_module = (grid.IsBeyondBoundary(transform.position + Vector3.left + Vector3.down)) ? null : grid.GetModule(transform.position + Vector3.left + Vector3.down);
		Module dright_module = (grid.IsBeyondBoundary(transform.position + Vector3.right + Vector3.down)) ? null : grid.GetModule(transform.position + Vector3.right + Vector3.down);

		if (forward_module != null || back_module != null || left_module != null || right_module != null )
        {
            if (forward_module != null && forward_module.transform.tag == "Stair_Module" &&   Mathf.Abs( forward_module.transform.rotation.eulerAngles.y ) < 0.1f && forward_module.isTimeBiggerThanOne == true)
            {
				//执行人物移动，将人物移动至该module上面一层，完成上楼
				
				StartCoroutine(AnimateGetUp(new Vector3(forward_module.transform.position.x, forward_module.transform.position.y + 1f, forward_module.transform.position.z)));
			}
			if (left_module != null && left_module.transform.tag == "Stair_Module"  && Mathf.Abs(left_module.transform.rotation.eulerAngles.y -270) < 0.1f && left_module.isTimeBiggerThanOne)
			{
				//人物转向

				//执行人物移动，将人物移动至该module上面一层，完成上楼
				StartCoroutine(AnimateGetUp(new Vector3(left_module.transform.position.x, left_module.transform.position.y + 1f, left_module.transform.position.z)));
			}
			if (right_module != null && right_module.transform.tag == "Stair_Module" && Mathf.Abs(right_module.transform.rotation.eulerAngles.y -90 )< 0.1f && right_module.isTimeBiggerThanOne)
			{
				//人物转向
				transform.rotation = Quaternion.Euler(0, 90, 0);

				//执行人物移动，将人物移动至该module上面一层，完成上楼
				StartCoroutine(AnimateGetUp(new Vector3(right_module.transform.position.x, right_module.transform.position.y + 1f, right_module.transform.position.z)));
			}
			if (back_module != null && back_module.transform.tag == "Stair_Module"  && Mathf.Abs(back_module.transform.rotation.eulerAngles.y - 180) < 0.1f && back_module.isTimeBiggerThanOne)
			{
				//执行人物移动，将人物移动至该module上面一层，完成上楼
				StartCoroutine(AnimateGetUp(new Vector3(back_module.transform.position.x, back_module.transform.position.y + 1f, back_module.transform.position.z)));
			}

			/*
			//这里back的判定有问题，还应该判断这个stair的朝向
			if (back_module != null && back_module.transform.tag == "Stair_Module")
			{
				//执行人物移动，将人物移动至该module上面一层，完成上楼
				StartCoroutine(AnimateGetUp(new Vector3(back_module.transform.position.x, back_module.transform.position.y + 1f, back_module.transform.position.z)));
			}
			*/
		}

		//这里我们先写简易版demo需要，咱不考虑平层和底层同时发生的情况。也咱不考虑stair朝向问题。

		/*
		 * 前进判定
		 */

		//下面一层还需要考虑，人物在两个module之间反复横跳的问题。
		if (dforward_module != null && dforward_module.transform.tag == "Platform_Module" && !dforward_module.isStepped && dforward_module.isTimeBiggerThanOne)
		{
			Debug.Log(dforward_module.name + "是否踩了" + dforward_module.isStepped);
			//把当前的module打上刚刚被踩过的标签。拒绝反复横跳

			//这里有个问题是需要先确定第一个元素时存在的，只不为null不行，必须先检测count
			//!为什么大于2呢，是因为我要列表始终保持2个数，一个是之前到了以后的module，一个是现在到了以后的module，isstepped都为true
			if (moduleJiaoXiaList.Count > 2)
			{
				if (moduleJiaoXiaList[0] == null)
				{
					return;
				}
				moduleJiaoXiaList[0].isStepped = false;
				moduleJiaoXiaList.RemoveAt(0);
				//Debug.Log(moduleJiaoXiaList.Count);
			}

			transform.rotation = Quaternion.Euler(0, 0, 0);
			//因为踩的是下面一层的module，所以同样移动到该module上面一层。完成平移
			StartCoroutine(AnimateGetUp(new Vector3(dforward_module.transform.position.x, dforward_module.transform.position.y + 1, dforward_module.transform.position.z)));

			//！！！这里出了一个bug就是，JiaoXia的module永远比动画快一步。用了一个不是办法的办法就是朝移动方向加一格获取Jiaoxia
			Module ModuleJiaoXia = dforward_module;
			ModuleJiaoXia.isStepped = true;
			moduleJiaoXiaList.Add(ModuleJiaoXia);


		}

		//下面一层还需要考虑，人物在两个module之间反复横跳的问题。
		if (dright_module != null && dright_module.transform.tag == "Target_Module" && !dright_module.isStepped && dright_module.isTimeBiggerThanOne)
		{
			Debug.Log(dright_module.name + "是否踩了" + dright_module.isStepped);
			//把当前的module打上刚刚被踩过的标签。拒绝反复横跳

			//这里有个问题是需要先确定第一个元素时存在的，只不为null不行，必须先检测count
			//!为什么大于2呢，是因为我要列表始终保持2个数，一个是之前到了以后的module，一个是现在到了以后的module，isstepped都为true
			if (moduleJiaoXiaList.Count > 2)
			{
				if (moduleJiaoXiaList[0] == null)
				{
					return;
				}
				moduleJiaoXiaList[0].isStepped = false;
				moduleJiaoXiaList.RemoveAt(0);
				//Debug.Log(moduleJiaoXiaList.Count);
			}

			transform.rotation = Quaternion.Euler(0, 90, 0);
			//因为踩的是下面一层的module，所以同样移动到该module上面一层。完成平移
			StartCoroutine(AnimateGetUp(new Vector3(dright_module.transform.position.x, dright_module.transform.position.y + 1, dright_module.transform.position.z)));

			//！！！这里出了一个bug就是，JiaoXia的module永远比动画快一步。用了一个不是办法的办法就是朝移动方向加一格获取Jiaoxia
			Module ModuleJiaoXia = dright_module;
			ModuleJiaoXia.isStepped = true;
			moduleJiaoXiaList.Add(ModuleJiaoXia);


		}


		if (dback_module != null && dback_module.transform.tag == "Platform_Module" && !dback_module.isStepped && dback_module.isTimeBiggerThanOne)
		{

			//把当前的module打上刚刚被踩过的标签。拒绝反复横跳
			if (moduleJiaoXiaList.Count > 2)
			{
				if (moduleJiaoXiaList[0] == null)
				{
					return;
				}
				//移除上一个的脚下物体。
				moduleJiaoXiaList[0].isStepped = false;
				moduleJiaoXiaList.RemoveAt(0);
			}

			transform.rotation = Quaternion.Euler(0, 180, 0);
			//因为踩的是下面一层的module，所以同样移动到该module上面一层。完成平移
			StartCoroutine(AnimateGetUp(new Vector3(dback_module.transform.position.x, dback_module.transform.position.y + 1, dback_module.transform.position.z)));
			Module ModuleJiaoXia = dback_module;
			ModuleJiaoXia.isStepped = true;
			moduleJiaoXiaList.Add(ModuleJiaoXia);



		}

		if (dleft_module != null && dleft_module.transform.tag == "Platform_Module" && !dleft_module.isStepped && dleft_module.isTimeBiggerThanOne) 
		{
			//把当前的module打上刚刚被踩过的标签。拒绝反复横跳
			if (moduleJiaoXiaList.Count > 2)
			{
				if (moduleJiaoXiaList[0] == null)
				{
					return;
				}
				//移除上一个的脚下物体。
				moduleJiaoXiaList[0].isStepped = false;
				moduleJiaoXiaList.RemoveAt(0);
			}

			transform.rotation = Quaternion.Euler(0, 270, 0);
			//因为踩的是下面一层的module，所以同样移动到该module上面一层。完成平移
			StartCoroutine(AnimateGetUp(new Vector3(dleft_module.transform.position.x, dleft_module.transform.position.y + 1, dleft_module.transform.position.z)));
			Module ModuleJiaoXia = dleft_module;
			ModuleJiaoXia.isStepped = true;
			moduleJiaoXiaList.Add(ModuleJiaoXia);



		}
		if (dright_module != null && dright_module.transform.tag == "Platform_Module" && !dright_module.isStepped && dright_module.isTimeBiggerThanOne)
		{
			//把当前的module打上刚刚被踩过的标签。拒绝反复横跳
			if (moduleJiaoXiaList.Count > 2)
			{
				if (moduleJiaoXiaList[0] == null)
				{
					return;
				}
				//移除上一个的脚下物体。
				moduleJiaoXiaList[0].isStepped = false;
				moduleJiaoXiaList.RemoveAt(0);
			}

			//人物转向
			transform.rotation = Quaternion.Euler(0, 90, 0);
			//因为踩的是下面一层的module，所以同样移动到该module上面一层。完成平移
			StartCoroutine(AnimateGetUp(new Vector3(dright_module.transform.position.x, dright_module.transform.position.y + 1, dright_module.transform.position.z)));

			Module ModuleJiaoXia = dright_module;
			ModuleJiaoXia.isStepped = true;
			moduleJiaoXiaList.Add(ModuleJiaoXia);

		}

		/*
		//当脚下不为椅子，甚至没有脚下时。
		if ( d_module.transform.tag != "Chair_Module" )
		{

			

		}
        else if(dback_module!= null && d_module.transform.tag == "Chair_Module" )
        {
            if (d_module.transform.rotation.eulerAngles.y == 0)
            {
				
            }
			if (d_module.transform.rotation.eulerAngles.y == 90)
			{

			}
			if (d_module.transform.rotation.eulerAngles.y == 180)
			{

			}
			if (d_module.transform.rotation.eulerAngles.y == 270)
			{

			}
		}
		*/

		

	}




	//角色重生
	private void OnCollisionEnter(Collision collision)
	{
		// 检查碰撞的对象是否有"Ground"的tag
		if (collision.gameObject.tag == "Ground")
		{
			Debug.Log("触碰地面");
			// 调用玩家死亡的方法
			PlayerFallDownReset();
			TextAnimation.instance.fullText = "Fall, fall, there's still a chance, there's still a chance.";
			Debug.Log("碰到了" + TextAnimation.instance.fullText);
			TextAnimation.instance.TypeInn();
		}

		
        //文字触发
       
	}

    private void OnTriggerEnter(Collider collision)
    {
		if (collision.gameObject != null)
		{
			if (collision.gameObject.tag == "Text1")
			{

				TextAnimation.instance.fullText = "Somnus, it's a cabinet, and the drawers of the cabinet act like stairs.\n What's inside the drawers? ";
				Debug.Log("碰到了" + TextAnimation.instance.fullText);
				TextAnimation.instance.TypeInn();
				collision.gameObject.SetActive(false);

			}
			if (collision.gameObject.tag == "Text2")
			{

				TextAnimation.instance.fullText = "Somnus doesn't know where it should go... It's a candlestick, just a candlestick, but the light is warm.";
				Debug.Log("碰到了" + TextAnimation.instance.fullText);
				TextAnimation.instance.TypeInn();
				collision.gameObject.SetActive(false);

			}
			if (collision.gameObject.tag == "Text3")
			{

				TextAnimation.instance.fullText = "Radiance, you have gained radiance. \n Dear Somnus, a brief farewell.";
				Debug.Log("碰到了" + TextAnimation.instance.fullText);
				TextAnimation.instance.TypeInn();

				collision.gameObject.SetActive(false);

				// 在第一关通关后调用此方法来解锁第二关
				PlayerPrefs.SetInt("Level2Unlocked", 1);
				PlayerPrefs.Save();


				StartCoroutine(UnlockNextLevel());
				Level1Button.instance.FadEOut();



			}
			if (collision.gameObject.tag == "Text4")
			{

				TextAnimation.instance.fullText = "It's amazing! For the first time (in a dream) Somnus realizes Itself.";
				Debug.Log("碰到了" + TextAnimation.instance.fullText);
				TextAnimation.instance.TypeInn();

				collision.gameObject.SetActive(false);

				// 在第一关通关后调用此方法来解锁第二关
				PlayerPrefs.SetInt("Level2Unlocked", 1);
				PlayerPrefs.Save();


				StartCoroutine(UnlockNextLevel());
				Level1Button.instance.FadEOut();



			}
		}
	}

	public IEnumerator UnlockNextLevel()
	{
		yield return new WaitForSeconds(5);

		int currentLevel = levelNum; // 当前关卡号，这里假设为第二关
		int nextLevel = currentLevel + 1; // 下一关

		// 将下一关的解锁状态设置为已解锁（状态值为1）
		PlayerPrefs.SetInt("Level" + nextLevel.ToString(), 1);
		PlayerPrefs.Save(); // 保存数据

		// 返回选关页面
		SceneManager.LoadScene(1); // 实现此函数来加载选关页面
	}


	public void PlayerFallDownReset()
    {
        //这个方法会有bug
        /*
        if (moduleJiaoXiaList.Count > 2)
        {
			//把脚下列表前两个清空，保留最后一位也就是玩家最后站着的module
			moduleJiaoXiaList.RemoveRange(0, 2);
			Module moduleReset = moduleJiaoXiaList[0];
			StartCoroutine(AnimateGetUp(new Vector3(moduleReset.transform.position.x, moduleReset.transform.position.y + 1, moduleReset.transform.position.z)));
		}
		else if (moduleJiaoXiaList.Count == 2)
		{
		    //当现在列表只有2位时，把第一位删掉
			moduleJiaoXiaList.RemoveAt(0);
			Module moduleReset = moduleJiaoXiaList[0];
			StartCoroutine(AnimateGetUp(new Vector3(moduleReset.transform.position.x, moduleReset.transform.position.y + 1, moduleReset.transform.position.z)));
		}
		else if (moduleJiaoXiaList.Count == 1)
		{
			//当现在列表只有1位时，直接选用
			Module moduleReset = moduleJiaoXiaList[0];
			StartCoroutine(AnimateGetUp(new Vector3(moduleReset.transform.position.x, moduleReset.transform.position.y + 1, moduleReset.transform.position.z)));
		}
		*/
        for (int i = moduleJiaoXiaList.Count; i > 0; i--)
        {
			Debug.Log(i);
            if (moduleJiaoXiaList[i-1] != null && moduleJiaoXiaList[i-1].isYuanLao)
            {
				Module moduleReset = moduleJiaoXiaList[i-1];
				StartCoroutine(AnimateGetUp(new Vector3(moduleReset.transform.position.x, moduleReset.transform.position.y + 1, moduleReset.transform.position.z)));
				break;
			}
        }
	}

	//楼梯方向判定
	public bool stairDirectionDetectcorrect(Module stairModule)
    {
        if (stairModule.transform.tag == "Stair_Module")
        {
			//先判断玩家相对于楼梯的哪个位置
			Vector3 player_to_stair = transform.position - stairModule.transform.position;
			Debug.Log(player_to_stair);
			Debug.Log(stairModule.transform.rotation.y);
			Debug.Log(270 * Mathf.Deg2Rad);
			//根据4个方向的位置给楼梯的唯一解

			//
			if (player_to_stair.x > 0 && stairModule.transform.rotation.eulerAngles.y  ==  90) //270 * Mathf.Deg2Rad

			{
				Debug.Log("右面");
				return true;
				

            }
            else if (player_to_stair.z > 0 && stairModule.transform.rotation.eulerAngles.y == 0)  //180
            {
				return true;
			}
			else if (player_to_stair.x < 0 && stairModule.transform.rotation.eulerAngles.y == 270)//90
			{
				return true;
			}
			else if (player_to_stair.z< 0 && stairModule.transform.rotation.eulerAngles.y == 180)
			{
				Debug.Log("正面");
				return true;
				
			}
            else
            {
				return false;
            }
		}
        else
        {
			return false;
        }
    }


    
   



    //角色移动动画
    private IEnumerator AnimateArc(Transform transform, Vector3 destination, float duration )
	{
		Vector3 start = transform.position;
		float startTime = Time.time;

		//duration时间越长，lerp的num值越小，lerp的速度越慢
		while (Time.time - startTime <= duration)
		{
			if (transform == null)
			{
				yield break;
			}
			float num = (Time.time - startTime) / duration;
			transform.position = Vector3.Lerp(start, destination, num); 
			
			yield return null;
		}
		//将player移动到目标地点
		transform.position = destination;
		yield break;
	}

	//执行位置移动
	private IEnumerator AnimateGetUp(Vector3 destination)
	{
		//yield return new WaitForSeconds(1f);
		yield return AnimateArc(transform, destination, 1f);
		
		yield break;
	}



}
