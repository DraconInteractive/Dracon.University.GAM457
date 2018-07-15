using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public GameObject playerPrefab;
    public Player player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SpawnCharacter (Tile t)
    {
        t.occupant = Instantiate(playerPrefab, t.transform.position + Vector3.up * 1.5f, Quaternion.identity, this.transform);
        player = t.occupant.GetComponent<Player>();
    }
}
