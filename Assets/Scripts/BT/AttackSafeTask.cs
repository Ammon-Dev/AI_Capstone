using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSafeTask : BTNode
{
    BTUnitManager btManager;

    public AttackSafeTask(BTUnitManager _btManager)
    {
        btManager = _btManager;
    }

    public override BTNodeStates Eval()
    {
        Unit activeUnit = btManager.activeUnit;

        var cover = btManager.findCover();

        int coverEnergy = cover.energy + activeUnit.weapon.energyRequired;

        while (activeUnit.currentEnergy > coverEnergy)
        {
            activeUnit.unitState = Unit.state.Attacking;

            btManager.AttackAction();
        }

        activeUnit.unitState = Unit.state.Moving;

        activeUnit.TeleportPlayer(cover.goal, cover.energy);

        activeUnit.unitState = Unit.state.Idle;


        return BTNodeStates.SUCCESS;
    }
}
