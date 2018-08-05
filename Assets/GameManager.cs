using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Weekly
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager manager;

        public int enemyCount;
        [HideInInspector]
        public EnemyController enemyController;
        [HideInInspector]
        public TiledWorldGenerator tileManager;

        private void Awake()
        {
            manager = this;
        }
        // Use this for initialization
        void Start()
        {
            StartCoroutine(Setup());
        }

        IEnumerator Setup()
        {
            tileManager = GetComponent<TiledWorldGenerator>();
            tileManager.Setup();
            GetComponent<PlayerController>().SpawnCharacter(tileManager.GetAvailableTile());
            enemyController = GetComponent<EnemyController>();
            for (int i = 0; i < enemyCount; i++)
            {
                enemyController.SpawnEnemy(tileManager.GetAvailableTile());
            }
            enemyController.SetupEnemyDetection();
            enemyController.StartCoroutine(enemyController.LogicUpdate());
            yield break;
        }
    }
}

