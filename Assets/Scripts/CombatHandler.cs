using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatHandler : MonoBehaviour
{
    //  Total health, once it reaches 0 the entity dies
    public int MaxHealth = 100;
    // How much damage we do per attack
    public int Attack = 10;
    // Damage reduction. Will be substracted directly from the enemy attack.
    public int Defense = 1;
    // How fast the attack gauch feels. Higher speed will make the entity act more often.
    public float Speed = 1;

    // TODO we will need to hook this up dynamically when spawning enemies
    public HealthBarHandler HealthBar = null;
    public HealthBarHandler GaucheBar = null;

    public string Name;
    public string Intro;

    private int CurrentHealth;
    private float AttackGauche = 0f;

    private float MaxAttackGauche = 5f;

    void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public bool Process(float scaledDeltaTime)
    {
        AttackGauche += scaledDeltaTime*Speed;
        //Debug.Log(name + ": Increased Attack Gauche by " + scaledDeltaTime * Speed + ". Attack Gauche: " + AttackGauche);
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
        int damageMitigated = damage - Defense;
        damageMitigated = Mathf.Max(damageMitigated, 0);
        CurrentHealth -= damageMitigated;
        CurrentHealth = Mathf.Max(CurrentHealth, 0);
        float HealthRatio = (float)CurrentHealth / (float)MaxHealth;
        HealthBar.SetSize(HealthRatio);
        Debug.Log("Attack did " + damageMitigated + " damage, health remaining: " + CurrentHealth);
        return CurrentHealth <= 0;
    }
}
