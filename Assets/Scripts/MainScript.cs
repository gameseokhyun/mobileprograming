using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{
	//public List<GameObject> blocks = new List<GameObject>();
	public GameObject blockPerfap;
	BlockHandler nowBlockHandler;
	public static Dictionary<int, List<GameObject>> installBlocks = new Dictionary<int, List<GameObject>>();
	private int score;
	private int comboCount = 0;
	private float comboTimer = 5;
	Transform gamePanel;
	Transform scoreText;
	GameObject comboText;
	public static int MAX_WIDTH = 810;
	public static int MIN_WIDTH = 25;
	public static int MAX_HEIGHT = 575;
	public static int MIN_HEIGHT = 25;
	public static int BLOCK_SIZE = 50;
	float tim = 0;

	void Start()
    {
		gamePanel = GameObject.Find("GamePanel").transform;
		scoreText = GameObject.Find("ScoreText").transform;
		comboText = GameObject.Find("ComboText");
		comboText.SetActive(false);
		Debug.Log(gamePanel.name);
		nowBlockHandler = new BlockHandler(gamePanel, blockPerfap);
	}

	// Update is called once per frame
	void Update()
	{
		tim += Time.deltaTime;
		if (comboTimer < 0)
		{
			comboText.SetActive(false);
			comboCount = 0;
		}
		comboTimer -= Time.deltaTime;

		if (nowBlockHandler.isEnd())
		{
			while (renewBlock(checkDestoryBlock()))
			{
			}
			nowBlockHandler = new BlockHandler(gamePanel, blockPerfap);
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

			}
		}
	}

	public int checkDestoryBlock()
	{
		int destroyLine = -1;
		foreach (KeyValuePair<int, List<GameObject>> installBlocks in MainScript.installBlocks)
		{
			Debug.Log(tim + "한줄 수 " + installBlocks.Value.Count);
			if (installBlocks.Value.Count == 16)
			{
				foreach (GameObject destoryBlock in installBlocks.Value)
				{
					Destroy(destoryBlock);
				}
				comboCount++;
				comboTimer = 5;
				installBlocks.Value.Clear();
				destroyLine = installBlocks.Key;
				score += 50;
				scoreText.GetComponent<Text>().text = score.ToString();
				comboText.SetActive(true);
				comboText.transform.GetComponent<Text>().text = comboCount.ToString() + "COMBO !";
				break;
			}
		}
		return destroyLine;
	}

	public bool renewBlock(int line)
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

	public void addScore(int amount)
	{
		score += amount;
	}

	public int getScore()
	{
		return score;
	}
}
