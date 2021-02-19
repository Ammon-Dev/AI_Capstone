using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTask : BTNode
{
    BTUnitManager btManager;

    public AttackTask(BTUnitManager _btManager)
    {
        btManager = _btManager;
    }

    public override BTNodeStates Eval()
    {
        Unit activeUnit = btManager.activeUnit;

        while (activeUnit.currentEnergy > 0)
        {
            activeUnit.unitState = Unit.state.Attacking;

            btManager.AttackAction();
        }

        activeUnit.unitState = Unit.state.Idle;


        return BTNodeStates.SUCCESS;
    }
}
