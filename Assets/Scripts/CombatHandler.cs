using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    //  Total health, once it reaches 0 the entity dies.
    public int MaxHealth = 100;
    // How much damage we do per attack.
    public int Attack = 10;
    // Damage reduction. Will be substracted directly from the enemy attack.
    public int Defense = 1;
    // How fast the attack gauch feels. Higher speed will make the entity act more often.
    public float Speed = 1;
    // How much score does this enemy give when defeated.
    public int Score = 1;

    // TODO we will need to hook this up dynamically when spawning enemies
    public HealthBarHandler HealthBar = null;
    public HealthBarHandler GaucheBar = null;

    public string Name;
    public string Intro;

    private int CurrentHealth;
    private float AttackGauche = 0f;

    private float MaxAttackGauche = 5f;

    private float LowHealthRatio = 0.3f;

    public int TempDefense = 0;
    public float TempSpeedSelf = 1f;
    public float TempSpeedEnemy = 1f;

    private BattleSystem BattleSystem = null;

    void Awake()
    {
        CurrentHealth = MaxHealth;
    }

    private void Start()
    {
        BattleSystem = FindAnyObjectByType<BattleSystem>();
        UpdateUI();
    }

    public bool IsLowHealth()
    {
        return (float)CurrentHealth / (float)MaxHealth <= LowHealthRatio;
    }

    public bool Process(float scaledDeltaTime)
    {
        AttackGauche += scaledDeltaTime*Speed*TempSpeedSelf*TempSpeedEnemy;
        if (AttackGauche >= MaxAttackGauche) 
        {
            AttackGauche = 0f;
            return true;
        }

        GaucheBar.SetSize(AttackGauche / MaxAttackGauche);
        return false;
    }

    public bool DoDamage(int damage)
    {
        int damageMitigated = damage - Defense - TempDefense;
        damageMitigated = Mathf.Max(damageMitigated, 0);
        CurrentHealth -= damageMitigated;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        float HealthRatio = (float)CurrentHealth / (float)MaxHealth;
        HealthBar.SetSize(HealthRatio);
        HealthBar.SetText(CurrentHealth+"/"+MaxHealth);
        HealthPopup.Create(BattleSystem.HealthPopupPrefab.transform, transform.position, -damageMitigated);
        Debug.Log("Attack did " + damageMitigated + " damage, health remaining: " + CurrentHealth);
        return CurrentHealth <= 0;
    }

    public bool Heal(int health)
    {
        CurrentHealth += health;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0, MaxHealth);
        float HealthRatio = (float)CurrentHealth / (float)MaxHealth;
        HealthBar.SetSize(HealthRatio);
        HealthBar.SetText(CurrentHealth + "/" + MaxHealth);
        HealthPopup.Create(BattleSystem.HealthPopupPrefab.transform, transform.position, health);
        Debug.Log("Healed for " + health + " health, health remaining: " + CurrentHealth);
        return CurrentHealth <= 0;
    }

    public void UpdateUI()
    {
        float HealthRatio = (float)CurrentHealth / (float)MaxHealth;
        HealthBar.SetSize(HealthRatio);
        HealthBar.SetText(CurrentHealth + "/" + MaxHealth);
        GaucheBar.SetSize(AttackGauche / MaxAttackGauche);
    }
}
