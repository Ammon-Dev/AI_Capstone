using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForEnemyTask : BTNode
{
    BTUnitManager btManager;

    public CheckForEnemyTask(BTUnitManager _btManager)
    {
        btManager = _btManager;
    }

    public override BTNodeStates Eval()
    {
        Unit activeUnit = btManager.activeUnit;

        Debug.Log("CheckForEnemyTask");


        if (btManager.CheckDanger(activeUnit.currentPosition))
        {
            return BTNodeStates.SUCCESS;
        }
        else
        {
            return BTNodeStates.FAILURE;
        }
    }
}
