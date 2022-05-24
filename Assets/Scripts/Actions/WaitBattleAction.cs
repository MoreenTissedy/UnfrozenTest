using System;
using UnityEngine;

namespace UnfrozenTest
{
    public class WaitBattleAction:BattleAction
    {
        public override event Action Done;

        public WaitBattleAction(Character actor, GameLoop gm) : base(actor, gm)
        {
        }

        public override void Next()
        {
            Resolve();
        }
        
        public override void Resolve()
        {
            gm.WaitAction(actor);
            Done?.Invoke();
        }

        public override void SetTarget(Character selectedTarget)
        {
        }

    }
}