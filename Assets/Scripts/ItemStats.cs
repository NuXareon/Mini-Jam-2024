using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStats : MonoBehaviour
{
    // Player's attack damage will increase by this amonut
    public int Attack = 0;
    // Player's damage received will be reduced by this amount
    public int Defense = 0;
    // Player's attack gauche fill rate will be scaled by this amount
    public float Speed = 1;
    // Player's health will increase by this amount
    public int Healing = 0;
    // Enemy's speed will be reduced by this amount
    public float EnemySlowPercent = 0f;
    // Player's attack damange will increase by this amoung when they are low on health
    public int ExtraAttackOnLowHealth = 0;
    // Damage received will be reflected to the enemy by this amount
    public float ReflectedDamagePercent = 0f;
}
