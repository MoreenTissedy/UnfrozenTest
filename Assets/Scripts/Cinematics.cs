using System;
using UnityEngine;

namespace UnfrozenTest
{
    public static class Cinematics
    {
        public static event Action Done;
        private static int currentAttackCount = 0;
        private static OffenseBattleAction currentAction;
        
        public static void Play(OffenseBattleAction action)
        {
            currentAction = action;
            currentAttackCount = 0;
            action.Actor.Animate(action.Ability.Animation).HitTime += OnHitTime;
            action.Actor.AnimationDone += OnAnimationEnd;
        }

        private static void OnAnimationEnd()
        {
            currentAction.Actor.HitTime -= OnHitTime;
            Done?.Invoke();
        }

        private static void OnHitTime()
        {
            if (currentAction.Ability.NumAttacks <= currentAttackCount)
                return;
            currentAction.Target.Damage(currentAction.damageDone[currentAttackCount]);
            currentAttackCount++;
        }
    }
}