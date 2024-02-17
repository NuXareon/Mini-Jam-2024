using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class BattleSystem : MonoBehaviour
{
    public GameObject Player;
    private CombatHandler PlayerStats;

    private GameObject Enemy;
    private CombatHandler EnemyStats;

    public SlotHandler Slot1;
    public SlotHandler Slot2;

    public AnnouncementsHandler Announcements;
    public SpawnerSystem Spawner;
    public ScoreSystem Score;

    private bool SuspendProcess = false;

    void Start()
    {
        PlayerStats = Player.GetComponent<CombatHandler>();

        Enemy = Spawner.SpawnEnemy();
        EnemyStats = Enemy.GetComponent<CombatHandler>();
    }

    void Update()
    {
        if (SuspendProcess)
        {
            return;
        }

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
            Announcements.Announce("Player is attacking");

            int Slot1Attack = Item1 ? Item1.Attack : 0;
            int Slot2Attack = Item2 ? Item2.Attack : 0;
            int AttackDmg = PlayerStats.Attack + Slot1Attack + Slot2Attack;
            AttackDmg = Mathf.Max(AttackDmg, 0);
            bool IsEnemyDead = EnemyStats.DoDamage(AttackDmg);
            if (IsEnemyDead)
            {
                Announcements.Announce(EnemyStats.Name + " has been defeated!");
                Debug.Log("Enemy killed!");
                Score.score += EnemyStats.Score;
                ExecuteEnemyTurn = false;
                StartCoroutine(SpawnNextEnemy());
            }
        }

        if (ExecuteEnemyTurn)
        {
            Debug.Log("Enemy Attacking!");

            EnemyStats.TempDefense = 0;
            EnemyStats.TempSpeed = 1f;

            int Slot1Defense = Item1 ? Item1.Defense : 0;
            int Slot2Defense = Item2 ? Item2.Defense : 0;
            int AttackDmg = EnemyStats.Attack - Slot1Defense - Slot2Defense;
            bool IsPlayerDead = false;

            AttackSequence attackSequence = Enemy.GetComponent<AttackSequence>();
            CustomAttackData customAttackData = attackSequence ? attackSequence.GetNextAttack() : null;
            if (customAttackData != null)
            {
                AttackDmg += customAttackData.Attack;
                EnemyStats.TempDefense = customAttackData.Defense;
                EnemyStats.TempSpeed = customAttackData.Speed;
                if (customAttackData.Healing > 0)
                {
                    EnemyStats.Heal(customAttackData.Healing);
                }
                if (customAttackData.Sprite != null)
                {
                    Enemy.GetComponent<SpriteRenderer>().sprite = customAttackData.Sprite;
                }
                Announcements.Announce(EnemyStats.Name + " " + customAttackData.AttackText);
            }
            else
            {
                Announcements.Announce(EnemyStats.Name + " is attacking");
            }

            AttackDmg = Mathf.Max(AttackDmg, 0);
            IsPlayerDead = PlayerStats.DoDamage(AttackDmg);

            if (IsPlayerDead)
            {
                Announcements.Announce("Player is dead. Game Over.");
                Debug.Log("Player is dead!");
            }

        }
    }

    IEnumerator SpawnNextEnemy()
    {
        SuspendProcess = true;
        yield return new WaitForSeconds(1.5f);

        Destroy(Enemy);

        Enemy = Spawner.SpawnEnemy();
        EnemyStats = Enemy.GetComponent<CombatHandler>();
        Announcements.Announce(EnemyStats.Name + " " + EnemyStats.Intro);

        SuspendProcess = false;
    }
}
