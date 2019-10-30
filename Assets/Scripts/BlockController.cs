using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
	REMOVE_RANDOM_LINE,
	SCORE_UP,
	None,
}
public class BlockController : MonoBehaviour
{
	private int parentID;
	private ItemType item = ItemType.None;
	// Start is called before the first frame update
	void Start()
    {
		//아이템 설정
		float rand = Random.Range(0, 100);
		if (rand < 5)
		{
			item = ItemType.REMOVE_RANDOM_LINE;
		}
		else if (rand < 10)
		{
			item = ItemType.SCORE_UP;
		}
	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public ItemType getItem()
	{
		return item;
	}

	public void move(MoveType type) //블록 각자 이동
	{
		switch (type)
		{
			case 0:
				transform.Translate(Vector3.up * MainScript.BLOCK_SIZE, Space.World);
				break;
			case MoveType.Move_Right:
				transform.Translate(Vector3.right * MainScript.BLOCK_SIZE, Space.World);
				break;
			case MoveType.Move_Down:
				transform.Translate(Vector3.down * MainScript.BLOCK_SIZE, Space.World);
				break;
			case MoveType.Move_Left:
				transform.Translate(Vector3.left * MainScript.BLOCK_SIZE, Space.World);
				break;
		}
	}
}
