using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum MoveType
{
	Tr,
	Move_Right,
	Move_Down,
	Move_Left,
}
public class BlockHandler : MonoBehaviour
{
	public List<GameObject> blocks = new List<GameObject>();
	private GameObject BlockPerfap;
	private static Mutex installLock = new Mutex();
	private bool isend;
	private float tim;
	public BlockHandler(Transform GamePanel, GameObject BlockPerfap)
	{
		int startX = (MainScript.BLOCK_SIZE / 2) + (MainScript.BLOCK_SIZE * Random.Range(1,9));
		for (int i = 0; i < 4; i++)
		{
			GameObject block = Instantiate(BlockPerfap, new Vector3(startX, 525, 0), Quaternion.identity);
			block.transform.parent = GamePanel;
			block.GetComponent<MeshRenderer>().material.color = Color.red;
			startX += MainScript.BLOCK_SIZE;
			blocks.Add(block);
		}
	}
    // Start is called before the first frame update
    void Start()
    {
        
    }

	
    // Update is called once per frame
    void Update()
    {
		tim += Time.deltaTime;
	}

	public bool isEnd()
	{
		return isend;
	}

	public void move(MoveType type)
	{
		if (checkAvailableMove(type))
		{
			foreach (GameObject block in blocks)
			{
				block.GetComponent<BlockController>().move(type);
			}
		}
	}


	public bool checkAvailableMove(MoveType type)
	{
		int moveX = type == MoveType.Move_Left ? -(MainScript.BLOCK_SIZE) : type == MoveType.Move_Right ? MainScript.BLOCK_SIZE : 0;
		int moveY = MainScript.BLOCK_SIZE;
		bool isCanMove = true;
		bool isAddInstall = false;
		List<GameObject> addBlockTemp = new List<GameObject>();
		Debug.Log(tim+"addBlockTemp" + addBlockTemp.Count);
		foreach (GameObject block in blocks)
		{
			Debug.Log(tim + "block.transform.position.y" + block.transform.position.y);
			if (block.transform.position.x + moveX < MainScript.MIN_WIDTH || block.transform.position.x + moveX > MainScript.MAX_WIDTH)
			{
				isCanMove = false;
				continue;
			}
			else if (block.transform.position.y - moveY < MainScript.MIN_HEIGHT)
			{
				Debug.Log(tim + "여기서추가1");
				isCanMove = false;
				isAddInstall = true;
				continue;
			}
			foreach (List<GameObject> installBlocks in MainScript.installBlocks.Values)
			{
				foreach (GameObject installBlock in installBlocks)
				{
					Debug.Log(tim + "block.transform.position.y - moveY" + (int)(block.transform.position.y - moveY));
					Debug.Log(tim + "installBlock.transform.position.y" + (int)(installBlock.transform.position.y));
					if ((int) block.transform.position.x + moveX == (int)installBlock.transform.position.x && (int) block.transform.position.y - moveY == (int) installBlock.transform.position.y)
					{
						Debug.Log(tim + "여기서추가2");
						isCanMove = false;
						isAddInstall = true;
						continue;
					}
				}
			}
		}
		if (isAddInstall)
			addInstallBlock();
		return isCanMove;
	}

	public void addInstallBlock()
	{
		foreach (GameObject block in blocks)
		{
			int line = (int)(block.transform.position.y / 50);
			Debug.Log(tim + "line" + line);
			if (!MainScript.installBlocks.ContainsKey(line))
			{
				MainScript.installBlocks.Add(line, new List<GameObject>());
			}
			MainScript.installBlocks[line].Add(block);
			Debug.Log(tim + "lineCount" + MainScript.installBlocks[line].Count);
			isend = true;
		}
	}
}
