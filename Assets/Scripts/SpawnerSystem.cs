using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerSystem : MonoBehaviour
{
    public Vector3 SpawnLoc;
    public GameObject[] Enemies;
    public HealthBarHandler EnemyHealthBar;
    public HealthBarHandler EnemyGauge;

    private bool isFirstEnemy = true;
    private int lastSpawnedIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public GameObject SpawnEnemy()
    {
        int index;
        if (isFirstEnemy)
        {
            index = 0;
        }
        else
        {
            index = Random.Range(0, Enemies.Length);
            if (index == lastSpawnedIdx)
            {
                index += 1;
                index %= Enemies.Length;
            }
        }

        GameObject newEnemy = Instantiate(Enemies[index], SpawnLoc, Quaternion.identity);
        CombatHandler enemyCombatHandler = newEnemy.GetComponent<CombatHandler>();
        enemyCombatHandler.HealthBar = EnemyHealthBar;
        enemyCombatHandler.GaucheBar = EnemyGauge;
        //enemyCombatHandler.UpdateUI();

        return newEnemy;
    }
}
