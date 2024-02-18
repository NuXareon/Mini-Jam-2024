using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public TextMeshProUGUI EnemyName;

    public GameObject HealthPopupPrefab;

    private bool SuspendProcess = false;

    void Start()
    {
        PlayerStats = Player.GetComponent<CombatHandler>();

        StartCoroutine(SpawnNextEnemy(true));
    }

    void OnEnemyKilled()
    {
        Announcements.Announce(EnemyStats.Name + " has been defeated!");
        Debug.Log("Enemy killed!");
        Score.IncreaseScore(EnemyStats.Score);
        StartCoroutine(SpawnNextEnemy(false));
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
            Debug.Log("Hero Attacking!");
            Announcements.Announce("Hero is attacking");

            bool IsPlayerDead = false;

            int Slot1Healing = Item1 ? Item1.Healing : 0;
            int Slot2Healing = Item2 ? Item2.Healing : 0;
            int Healing = Slot1Healing + Slot2Healing;
            if (Healing != 0)
            {
                IsPlayerDead = PlayerStats.Heal(Healing);
            }

            float Slot1Slow = Item1 ? Item1.EnemySlowPercent : 0f;
            float Slot2Slow = Item2 ? Item2.EnemySlowPercent : 0f;
            float Slow = Slot1Slow + Slot2Slow;
            if (Slow > 0f)
            {
                EnemyStats.TempSpeedEnemy = Slow/100f;
            }

            int ExtraAttackOnLowHealth = 0;
            if (PlayerStats.IsLowHealth())
            {
                int Slot1ExtraAttackOnLowHealth = Item1 ? Item1.ExtraAttackOnLowHealth : 0;
                int Slot2ExtraAttackOnLowHealth = Item2 ? Item2.ExtraAttackOnLowHealth : 0;
                ExtraAttackOnLowHealth = Slot1ExtraAttackOnLowHealth + Slot2ExtraAttackOnLowHealth;
            }

            int Slot1Attack = Item1 ? Item1.Attack : 0;
            int Slot2Attack = Item2 ? Item2.Attack : 0;
            int AttackDmg = PlayerStats.Attack + Slot1Attack + Slot2Attack + ExtraAttackOnLowHealth;
            AttackDmg = Mathf.Max(AttackDmg, 0);
            bool IsEnemyDead = EnemyStats.DoDamage(AttackDmg);
            if (IsPlayerDead)
            {
                ExecuteEnemyTurn = false;
                if (IsEnemyDead)
                {
                    Debug.Log("Enemy killed!");
                    Score.IncreaseScore(EnemyStats.Score);
                }
                StartCoroutine(OnPlayerDeath());
            }
            else if (IsEnemyDead)
            {
                ExecuteEnemyTurn = false;
                OnEnemyKilled();
            }
        }

        if (ExecuteEnemyTurn)
        {
            Debug.Log("Enemy Attacking!");

            int Slot1Defense = Item1 ? Item1.Defense : 0;
            int Slot2Defense = Item2 ? Item2.Defense : 0;
            int AttackDmg = EnemyStats.Attack;

            bool IsEnemyDead = false;
            AttackSequence attackSequence = Enemy.GetComponent<AttackSequence>();
            CustomAttackData customAttackData = attackSequence ? attackSequence.GetNextAttack() : null;
            if (customAttackData != null)
            {
                Debug.Log("Custom attack");
                AttackDmg += customAttackData.Attack;

                float Slot1ReflectDamage = Item1 ? Item1.ReflectedDamagePercent : 0f;
                float Slot2ReflectDamage = Item2 ? Item2.ReflectedDamagePercent : 0f;
                float ReflectedDamagePercent = Slot1ReflectDamage + Slot2ReflectDamage;

                if (ReflectedDamagePercent > 0f)
                {
                    int DamageReflected = (int)(AttackDmg * (ReflectedDamagePercent / 100f));
                    IsEnemyDead |= EnemyStats.DoDamage(DamageReflected);
                }

                EnemyStats.TempDefense = customAttackData.Defense;
                EnemyStats.TempSpeedSelf = customAttackData.Speed;
                if (customAttackData.Healing != 0)
                {
                    IsEnemyDead |= EnemyStats.Heal(customAttackData.Healing);
                }
                if (customAttackData.Sprite != null)
                {
                    Enemy.GetComponent<SpriteRenderer>().sprite = customAttackData.Sprite;
                }
                Announcements.Announce(EnemyStats.Name + " " + customAttackData.AttackText);
            }
            else
            {
                float Slot1ReflectDamage = Item1 ? Item1.ReflectedDamagePercent : 0f;
                float Slot2ReflectDamage = Item2 ? Item2.ReflectedDamagePercent : 0f;
                float ReflectedDamagePercent = Slot1ReflectDamage + Slot2ReflectDamage;
                if (ReflectedDamagePercent > 0f)
                {
                    int DamageReflected = (int)((float)AttackDmg * (ReflectedDamagePercent / 100f));
                    IsEnemyDead |= EnemyStats.DoDamage(DamageReflected);
                }

                EnemyStats.TempDefense = 0;
                EnemyStats.TempSpeedSelf = 1f;

                Announcements.Announce(EnemyStats.Name + " is attacking");
            }

            EnemyStats.TempSpeedEnemy = 1f;

            AttackDmg = AttackDmg - Slot1Defense - Slot2Defense;
            AttackDmg = Mathf.Max(AttackDmg, 0);

            bool IsPlayerDead = PlayerStats.DoDamage(AttackDmg);
            if (IsPlayerDead)
            {
                if (IsEnemyDead)
                {
                    Debug.Log("Enemy killed!");
                    Score.IncreaseScore(EnemyStats.Score);
                }
                StartCoroutine(OnPlayerDeath());
            }
            else if (IsEnemyDead)
            {
                OnEnemyKilled();
            }
        }
    }

    IEnumerator SpawnNextEnemy (bool firstEnemy)
    {
        SuspendProcess = true;
        if (firstEnemy)
        {
            yield return new WaitForSeconds(2.5f);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }

        if (Enemy)
        {
            Destroy(Enemy);
        }

        Enemy = Spawner.SpawnEnemy();
        EnemyStats = Enemy.GetComponent<CombatHandler>();
        Announcements.Announce(EnemyStats.Name + " " + EnemyStats.Intro);
        EnemyName.text = EnemyStats.Name;

        SuspendProcess = false;
    }

    IEnumerator OnPlayerDeath()
    {
        Debug.Log("Hero is dead!");
        Announcements.Announce("Hero is dead! Game Over.");
        SuspendProcess = true;
        Score.DisplayFinalScore();
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(0);
    }
}
