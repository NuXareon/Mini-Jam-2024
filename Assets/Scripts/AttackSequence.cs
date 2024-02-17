using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomAttackData
{
    public int Attack = 0;
    public int Defense = 0;
    public float Speed = 1f;
    public int Healing = 0;
    public string AttackText;
}

public class AttackSequence : MonoBehaviour
{
    public CustomAttackData[] AttackData;

    private int CurrentAttackIndex = 0;

    public CustomAttackData GetNextAttack()
    {
        if (AttackData.Length == 0)
        {
            return null;
        }

        CustomAttackData Result = AttackData[CurrentAttackIndex];
        CurrentAttackIndex++;
        if (CurrentAttackIndex  == AttackData.Length)
        {
            CurrentAttackIndex = 0;
        }
        return Result;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
