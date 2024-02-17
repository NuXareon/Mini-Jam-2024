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
}
