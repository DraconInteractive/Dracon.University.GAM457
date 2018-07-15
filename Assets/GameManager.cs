using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public int enemyCount;
	// Use this for initialization
	void Start () {
        StartCoroutine(Setup());
	}
	
	IEnumerator Setup ()
    {
        TiledWorldGenerator w = GetComponent<TiledWorldGenerator>();
        w.Setup();
        GetComponent<PlayerController>().SpawnCharacter(w.GetAvailableTile());
        EnemyController e = GetComponent<EnemyController>();
        for (int i = 0; i < enemyCount; i++)
        {
            e.SpawnEnemy(w.GetAvailableTile());
        }
        e.SetupEnemyDetection();
        e.StartCoroutine(e.VisionUpdate());
        yield break;
    }
}
