using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
	private int parentID;
    // Start is called before the first frame update
    void Start()
    {

	}

	// Update is called once per frame
	void Update()
    {
        
    }

	public void move(MoveType type)
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
