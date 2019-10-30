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
	Set_Down,
}

public enum Shape
{
	LONG_RECTANGLE,
	SQUARE,
	SHAPE_L,
	SHAPE_N,
	SHAPE_P,
	SHAPE_T,
	SHAPE_U,
	SHAPE_V,
	SHAPE_X,
}
public class BlockHandler : MonoBehaviour
{
	public List<GameObject> blocks = new List<GameObject>();
	private GameObject BlockPerfap;
	private static Mutex installLock = new Mutex();
	private bool isend;
	private int downTime = 0;
	public BlockHandler(Transform GamePanel, GameObject BlockPerfap) 
	{
		this.BlockPerfap = BlockPerfap;
		int startX = (MainScript.BLOCK_SIZE / 2) + (MainScript.BLOCK_SIZE * Random.Range(1,9)); //생성 위치 랜덤
		createBlock((Shape) Random.Range(0,9), startX, 550); //각 블록 생성
	}

	private void addBlock(Shape type, float x, float y)
	{
		GameObject block = null;
		block = Instantiate(BlockPerfap, new Vector3(x, y, 0), Quaternion.identity);
		//block.GetComponent<MeshRenderer>().material.color;
		blocks.Add(block);
	}

	private void createBlock(Shape type, float x, float y)
	{
		//블록을 모양대로 생성
		switch (type)
		{
			case Shape.LONG_RECTANGLE:
				{
					addBlock(type, x, y);
					addBlock(type, x + MainScript.BLOCK_SIZE, y);
					addBlock(type, x + MainScript.BLOCK_SIZE * 2, y);
					addBlock(type, x + MainScript.BLOCK_SIZE * 3, y);
					addBlock(type, x + MainScript.BLOCK_SIZE * 4, y);
					break;
				}
			case Shape.SQUARE:
				{
					addBlock(type, x, y);
					addBlock(type, x + MainScript.BLOCK_SIZE, y);
					addBlock(type, x, y + MainScript.BLOCK_SIZE);
					addBlock(type, x + MainScript.BLOCK_SIZE, y + MainScript.BLOCK_SIZE);
					break;
				}
			case Shape.SHAPE_L:
				{
					addBlock(type, x, y);
					addBlock(type, x + MainScript.BLOCK_SIZE, y);
					addBlock(type, x, y + MainScript.BLOCK_SIZE);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 2);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 3);
					break;
				}
			case Shape.SHAPE_N:
				{
					addBlock(type, x, y);
					addBlock(type, x + MainScript.BLOCK_SIZE, y + MainScript.BLOCK_SIZE * 2);
					addBlock(type, x + MainScript.BLOCK_SIZE, y + MainScript.BLOCK_SIZE * 3);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 1);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 2);
					break;
				}
			case Shape.SHAPE_P:
				{
					addBlock(type, x, y);
					addBlock(type, x + MainScript.BLOCK_SIZE * 1, y + MainScript.BLOCK_SIZE * 1);
					addBlock(type, x + MainScript.BLOCK_SIZE * 1, y + MainScript.BLOCK_SIZE * 2);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 1);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 2);
					break;
				}
			case Shape.SHAPE_T:
				{
					addBlock(type, x, y);
					addBlock(type, x - MainScript.BLOCK_SIZE * 1, y + MainScript.BLOCK_SIZE * 2);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 1);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 2);
					addBlock(type, x + MainScript.BLOCK_SIZE * 1, y + MainScript.BLOCK_SIZE * 2);
					break;
				}
			case Shape.SHAPE_U:
				{
					addBlock(type, x, y);
					addBlock(type, x - MainScript.BLOCK_SIZE * 1, y + MainScript.BLOCK_SIZE * 1);
					addBlock(type, x - MainScript.BLOCK_SIZE * 1, y);
					addBlock(type, x + MainScript.BLOCK_SIZE * 1, y + MainScript.BLOCK_SIZE * 1);
					addBlock(type, x + MainScript.BLOCK_SIZE * 1, y);
					break;
				}
			case Shape.SHAPE_V:
				{
					addBlock(type, x, y);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 1);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 2);
					addBlock(type, x + MainScript.BLOCK_SIZE * 1, y);
					addBlock(type, x + MainScript.BLOCK_SIZE * 2, y);
					break;
				}
			case Shape.SHAPE_X:
				{
					addBlock(type, x, y);
					addBlock(type, x, y + MainScript.BLOCK_SIZE * 1);
					addBlock(type, x, y - MainScript.BLOCK_SIZE * 1);
					addBlock(type, x + MainScript.BLOCK_SIZE * 1, y);
					addBlock(type, x - MainScript.BLOCK_SIZE * 1, y);
					break;
				}
		}
		
	}

    // Start is called before the first frame update
    void Start()
    {
        
    }

	
    // Update is called once per frame
    void Update()
    {
	}

	public bool isEnd()
	{
		return isend;
	}

	public void move(MoveType type)
	{
		if (type == MoveType.Set_Down) //스페이스바 누를시 맨밑으로 내려가는데 이 경우만 특별히 맨밑에 아무것도 없을때까지 체크하기 위해 따로 빼줌
		{
			while (checkAvailableMove(MoveType.Move_Down)) //아무것도 없을때 확인 -> 내림 반복
			{
				foreach (GameObject block in blocks)
				{
					block.GetComponent<BlockController>().move(MoveType.Move_Down);
				}
			}
		}
		else
		{
			if (checkAvailableMove(type))
			{
				foreach (GameObject block in blocks)
				{
					block.GetComponent<BlockController>().move(type);
				}
			}
			else if (downTime > 3) //블록이 더이상 밑으로 가지 못할 때 바로  끝내지않고 옆으로는 이동할 수 있게 해주기 위한 타이머
			{
				addInstallBlock();
			}
		}
	}


	public bool checkAvailableMove(MoveType type) //이동 가능 여부 체크
	{
		int moveX = type == MoveType.Move_Left ? -(MainScript.BLOCK_SIZE) : type == MoveType.Move_Right ? MainScript.BLOCK_SIZE : 0;
		int moveY = MainScript.BLOCK_SIZE;
		bool isCanMove = true;
		List<GameObject> addBlockTemp = new List<GameObject>();
		foreach (GameObject block in blocks)
		{
			if (block.transform.position.x + moveX < MainScript.MIN_WIDTH || block.transform.position.x + moveX > MainScript.MAX_WIDTH) //x값이 화면 밖일때 못나가게끔
			{
				isCanMove = false;
				continue;
			}
			else if (block.transform.position.y - moveY < MainScript.MIN_HEIGHT) //y값이 화면 밖일때 못나가게끔
			{
				isCanMove = downTime > 0 && (type == MoveType.Move_Left || type == MoveType.Move_Right); //옆으로는 이동할 수 있게 해주는 if문
				downTime++;
				continue;
			}
			foreach (List<GameObject> installBlocks in MainScript.installBlocks.Values)
			{
				foreach (GameObject installBlock in installBlocks)
				{
					if (((int) block.transform.position.y - moveY == (int) installBlock.transform.position.y) && (int)block.transform.position.x + moveX == (int)installBlock.transform.position.x)
					{
						isCanMove = false;
						downTime++;
						continue;

					}
				}
			}
		}
		return isCanMove;
	}

	public void addInstallBlock() //더이상 움직이지 못하면 설치된 블록들 추가
	{
		foreach (GameObject block in blocks)
		{
			int line = (int)(block.transform.position.y / 50);
			if (!MainScript.installBlocks.ContainsKey(line))
			{
				MainScript.installBlocks.Add(line, new List<GameObject>());
			}
			MainScript.installBlocks[line].Add(block);
			isend = true;
		}
	}
}
