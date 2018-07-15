using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    public GameObject enemyPrefab;
    List<Enemy> enemies = new List<Enemy>();
	// Use this for initialization
	public IEnumerator VisionUpdate ()
    {
        while (true)
        {
            foreach (Enemy enemy in enemies)
            {
                EnemyVisionUpdate(enemy);
                yield return null;
            }
        }
        
        yield break;
    }

    public void SpawnEnemy (Tile t)
    {
        t.occupant = Instantiate(enemyPrefab, t.transform.position + Vector3.up * 1.5f, Quaternion.identity, this.transform);
        enemies.Add(t.occupant.GetComponent<Enemy>());
    }

    public void SetupEnemyDetection ()
    {
        int counter = 0;
        foreach (Enemy e in enemies)
        {
            e.gameObject.name = "Enemy " + counter;
            counter++;
            foreach (Enemy ee in enemies)
            {
                if (e != ee)
                {
                    e.detectionDictionary.Add(ee, 0);
                }
            }
        }
    }
    void EnemyVisionUpdate(Enemy enemy)
    {

        //Foreach enemy, check the dot product of every other enemy. 
        //if the dot is greater than 0.5 (60 degree field), do a distance check, then a raycast.
        //If the enemy passes all checks, they are "detected". 
        //Each enemy has a dictionary of all other units, as well as how detected they are.
        //If a unit is "detected" during this pass, this adds onto their detection float. if this float is over 1, they are suspicious, and above 2 they are truly detected. 
        foreach (Enemy e in enemies)
        {
            bool add = false;
            if (e != enemy)
            {
                Vector3 heading = (e.transform.position - enemy.transform.position).normalized;
                Vector3 forward = transform.forward;
                float dot = Vector3.Dot(heading, forward);

                if (dot > 0.5f)
                {
                    float dist = Vector3.Distance(enemy.transform.position, e.transform.position);

                    if (dist < enemy.detectionDistance)
                    {
                        Ray ray = new Ray(enemy.transform.position + Vector3.up * 0.5f, (e.transform.position - enemy.transform.position).normalized);
                        RaycastHit hit;

                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.transform.gameObject == e.gameObject)
                            {
                                add = true;
                            }
                            //print(enemy.name + " Ray blocked: " + hit.transform.name);
                            //hit.transform.GetComponent<Renderer>().material.color = Color.cyan;
                        }
                        else
                        {
                            add = true;
                        }
                    }
                }
                //float mod = 0;
                if (add)
                {
                    enemy.detectionDictionary[e] += 1 * Time.deltaTime * enemies.Count;
                }
                else
                {
                    enemy.detectionDictionary[e] -= 0.5f * Time.deltaTime * enemies.Count;
                }

                enemy.detectionDictionary[e] = Mathf.Clamp(enemy.detectionDictionary[e], -1, 3);
            }
        }

        //Just for debug, but if this enemy has one fully detected unit, it will turn its marker true red. 
        bool detected = false;
        foreach (KeyValuePair<Character, float> pair in enemy.detectionDictionary)
        {
            if (pair.Value > 2)
            {
                detected = true;
                break;
            }
        }
        if (detected)
        {
            LineRenderer l = enemy.GetComponent<LineRenderer>();
            l.startColor = Color.red;
            l.endColor = Color.red;
        }
        

    }
}
