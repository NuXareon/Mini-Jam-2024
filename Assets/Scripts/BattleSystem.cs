using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    public GameObject Player;
    private CombatHandler PlayerStats;
    // TODO this needs to come from the spawner system
    public GameObject Enemy;
    private CombatHandler EnemyStats;

    public SlotHandler Slot1;
    public SlotHandler Slot2;

    void Start()
    {
        PlayerStats = Player.GetComponent<CombatHandler>();
        EnemyStats = Enemy.GetComponent<CombatHandler>();
    }

    void Update()
    {
        ItemStats Item1 = Slot1.GetItemStats();
        ItemStats Item2 = Slot2.GetItemStats();
        float Slot1Speed = Item1 ? Item1.Speed : 1f;
        float Slot2Speed = Item2 ? Item2.Speed : 1f;
        float playerScaledDt = Time.deltaTime * Slot1Speed * Slot2Speed;
        bool ExecutePlayerTurn = PlayerStats.Process(playerScaledDt);
        bool ExecuteEnemyTurn = EnemyStats.Process(Time.deltaTime);

        if (ExecutePlayerTurn)
        {
            Debug.Log("Player Attacking!");

            int Slot1Attack = Item1 ? Item1.Attack : 0;
            int Slot2Attack = Item2 ? Item2.Attack : 0;
            int AttackDmg = PlayerStats.Attack + Slot1Attack + Slot2Attack;
            AttackDmg = Mathf.Max(AttackDmg, 0);
            bool IsEnemyDead = EnemyStats.DoDamage(AttackDmg);
            if (IsEnemyDead)
            {
                Debug.Log("Enemy killed!");
                ExecuteEnemyTurn = false;
            }
        }

        if (ExecuteEnemyTurn)
        {
            Debug.Log("Enemy Attacking!");

            // TODO enemy will have an attack sequence, some sort of array of actions, we will need to consider that when attacking
            bool IsPlayerDead = PlayerStats.DoDamage(EnemyStats.Attack);
            if (IsPlayerDead)
            {
                Debug.Log("Player is dead!");
            }
        }
    }
}
