using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
	//블럭
	public GameObject blockPerfap;
	BlockHandler nowBlockHandler;
	public static Dictionary<int, List<GameObject>> installBlocks = new Dictionary<int, List<GameObject>>();

	//점수, 봄보
	private int score;
	private int comboCount = 0;
	private float comboTimer = 5;

	//UI
	Transform gamePanel;
	Transform scoreText;
	Transform levelText;
	GameObject comboText;
	GameObject itemText;

	//사이즈 
	public static int MAX_WIDTH = 810;
	public static int MIN_WIDTH = 25;
	public static int MAX_HEIGHT = 575;
	public static int MIN_HEIGHT = 25;
	public static int BLOCK_SIZE = 50;

	//난이도 및 레벨
	private int level = 1;
	private float timer = 0.0f;
	private float timeDelay = 0.5f;
	private float itemTimer = 0.0f;

	private bool isGameEnd;

	void Start()
    {
		gamePanel = GameObject.Find("GamePanel").transform;
		scoreText = GameObject.Find("ScoreText").transform;
		levelText = GameObject.Find("LevelText").transform;
		comboText = GameObject.Find("ComboText");
		comboText.SetActive(false);
		itemText = GameObject.Find("ItemText");
		itemText.SetActive(false);
		timeDelay -= (float)(level * 0.15);
		nowBlockHandler = new BlockHandler(gamePanel, blockPerfap);
	}

	// Update is called once per frame
	void Update()
	{
		if (isGameEnd) return;
		//콤보 시간과 아이템 시간 체크 후 안내메시지 사라짐
		if (comboTimer < 0)
		{
			comboText.SetActive(false);
			comboCount = 0;
		}
		comboTimer -= Time.deltaTime;

		if (itemTimer < 0)
		{
			itemText.SetActive(false);
			itemTimer = 0;
		}
		itemTimer -= Time.deltaTime;
		
		if (nowBlockHandler.isEnd()) //블록 이동이 끝났다면
		{
			if (!checkGameEnd()) //게임이 끝난 지 체크
			{
				while (renewBlock(checkDestoryBlock())) //줄을 사라지게 한 뒤 터진 줄부터 안터진 줄까지 줄수를 -1 해줌
				{
				}
				nowBlockHandler = new BlockHandler(gamePanel, blockPerfap); //새로운 블록생성
			}
		}
		else
		{
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				nowBlockHandler.move(0);
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				nowBlockHandler.move(MoveType.Move_Right);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				nowBlockHandler.move(MoveType.Move_Down);
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				nowBlockHandler.move(MoveType.Move_Left);
			}
			else if (Input.GetKeyDown(KeyCode.Space))
			{
				nowBlockHandler.move(MoveType.Set_Down);
			}
			timer += Time.deltaTime; //내려가는 속도 조절
			if (timer < timeDelay)
			{
				return;
			}
			timer = 0;
			nowBlockHandler.move(MoveType.Move_Down);
		}
	}

	public int checkDestoryBlock() //사라져야할 줄이 있는 지 체크
	{
		int destroyLine = -1;
		foreach (KeyValuePair<int, List<GameObject>> installBlocks in MainScript.installBlocks)
		{
			if (installBlocks.Value.Count == 16) //key 값은 y축이고 밸류 값은 블록들로 형성돼있는데 한줄에 최대 16블록이 들어갈 수 있으므로 16개의 블록이 있으면 사라지게함
			{
				destroyLine = installBlocks.Key; //사라진 줄부터 윗 줄의 key값을 정리해주기 위한 변수
				doDestroyLine(destroyLine); //사라져야할 줄을 사라지게함
				break;
			}
		}
		return destroyLine;
	}

	private void doDestroyLine(int line)
	{
		foreach (GameObject destoryBlock in MainScript.installBlocks[line])
		{
			//낮은 확률로 블록에 아이템이 있는데 그 블록을 깰 경우 아이템 효과 발동
			ItemType item = destoryBlock.GetComponent<BlockController>().getItem();
			if (item == ItemType.SCORE_UP)
			{
				addScore(100);
				comboText.SetActive(true);
				comboText.transform.GetComponent<Text>().text = "아이템 보너스 스코어 사용!";
			}
			else if (item == ItemType.REMOVE_RANDOM_LINE) //랜덤으로 다른 라인을 삭제시킴
			{
				if (MainScript.installBlocks.Count > 1) //존재하는 라인이 자기라인밖에 없을경우 패스
				{
					int randLine = (int)Random.Range(0, MainScript.installBlocks.Count);
					while (true)
					{
						//조건 : 다른 라인이여야하며 그 라인에 블록이 존재해야함
						if (randLine != line && MainScript.installBlocks.ContainsKey(randLine) && MainScript.installBlocks[randLine].Count > 0)
							break;
						randLine = (int)Random.Range(0, MainScript.installBlocks.Count);
					}
					comboText.SetActive(true);
					comboText.transform.GetComponent<Text>().text = "아이템 랜덤 줄 파괴 사용!";
				}
			}
			Destroy(destoryBlock);
		}
		itemTimer = 5;
		addCombo(); //콤보 증가
		MainScript.installBlocks[line].Clear();
		addScore(50); //점수증가
	}

	private bool checkGameEnd() //8번째 줄부터 설치된 블록이 정해진 y좌표를 넘어갈 경우 게임 종료
	{
		for (int i = 8; i < MainScript.installBlocks.Count; i++)
		{
			foreach (GameObject block in MainScript.installBlocks[i])
			{
				if (block.transform.position.y > 550)
				{
					comboText.SetActive(true);
					comboText.transform.GetComponent<Text>().text = "GAME END";
					isGameEnd = true;
					return true;
				}
			}
		}
		return false;
	}

	private bool renewBlock(int line) //줄이 사라졌을때 그 줄과 위에 줄에 있는 라인들의 값을 -1 시켜줌
	{
		if (line < 0) return false;
		for (int i = line + 1; i < MainScript.installBlocks.Count; i++)
		{
			foreach (GameObject renewBlock in MainScript.installBlocks[i])
			{
				renewBlock.GetComponent<BlockController>().move(MoveType.Move_Down);
			}
			MainScript.installBlocks[i - 1].AddRange(MainScript.installBlocks[i]);
		}
		MainScript.installBlocks[MainScript.installBlocks.Count - 1].Clear();
		return true;
	}

	private void addScore(int amount) //점수 증가 level * 300 이상의 경험치 획득시 1단계 레벨업
	{
		score += amount;
		if (score >= level * 300)
		{
			score = 0;
			level++;
			timeDelay -= (float)(level * 0.15); //레벨업 시 내려가는 속도 빨라짐
			comboText.SetActive(true);
			comboText.transform.GetComponent<Text>().text = "레벨 업 ! ";
			levelText.GetComponent<Text>().text = level.ToString();
		}
		scoreText.GetComponent<Text>().text = score.ToString();
	}

	private void addCombo()
	{
		comboCount++;
		comboTimer = 5;
		comboText.SetActive(true);
		comboText.transform.GetComponent<Text>().text = comboCount.ToString() + "COMBO !";
	}


	private int getScore()
	{
		return score;
	}
}
