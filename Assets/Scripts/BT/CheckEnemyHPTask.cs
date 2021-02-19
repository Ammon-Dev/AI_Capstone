using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckEnemyHPTask : BTNode
{
    BTUnitManager btManager;

    public CheckEnemyHPTask(BTUnitManager _btManager)
    {
        btManager = _btManager;
    }

    public override BTNodeStates Eval()
    {
        Unit activeUnit = btManager.activeUnit;

        float enemyHP = btManager.CheckEnemyHP();


        int attackAmmount = Mathf.FloorToInt(activeUnit.currentEnergy / activeUnit.weapon.energyRequired);
        float totalDmg = activeUnit.weapon.damage * attackAmmount;

        if (enemyHP < totalDmg)
        {
            return BTNodeStates.SUCCESS;
        }
        else
        {
            return BTNodeStates.FAILURE;
        }
    }
}

