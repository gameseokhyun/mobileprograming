using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
	public List<GameObject> enemys = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
		float distance = 99999;
		int closerEnemyIndex = -1;
		GameObject Player = GameObject.Find("PlayerCube");
		if (Player != null)
		{
			Debug.Log("플레이어를 찾았습니다.");
			Debug.Log("적들을 탐색하겠습니다.");
		} else
		{
			return;
		}
		for (int i = 1; i <= 5; i++)
		{
			GameObject Enemy = GameObject.Find("EnemyCube" + i);
			if (Enemy != null)
			{
				Debug.Log("적" + i +"을 찾았습니다.");
				Debug.Log(string.Format("적{0}의 위치 x : {1}, y : {2} z : {3}", i, Enemy.transform.position.x, Enemy.transform.position.y, Enemy.transform.position.z));
				float distanceEnemyToPlayer = Vector3.Distance(Player.transform.position, Enemy.transform.position);
				if (distance > distanceEnemyToPlayer)
				{
					distance = distanceEnemyToPlayer;
					closerEnemyIndex = i;
				}
				enemys.Add(Enemy);
			}
		}
		Debug.Log("가장 가까운 적 인덱스 : " + closerEnemyIndex);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
