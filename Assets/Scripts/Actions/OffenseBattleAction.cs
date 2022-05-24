using System;
using UnityEngine;

namespace UnfrozenTest
{
    public class OffenseBattleAction:BattleAction
    {
        public override event Action Done;
        protected Ability ability;

        public Ability Ability => ability;

        protected Character target;
        public Character Target => target;

        public int[] damageDone;
        protected int[] DamageDone => damageDone;

        public OffenseBattleAction(Character actor, Ability ability, GameLoop gm) : base(actor, gm)
        {
            this.ability = ability;
        }

        public override void Resolve()
        {
            damageDone = Target.CalculateDamageTaken(ability);
            Cinematics.Play(this);
            Cinematics.Done += CinematicsDone;
        }

        void CinematicsDone()
        {
            Cinematics.Done -= CinematicsDone; 
            Done?.Invoke();
        }

        public override void Next()
        {
            gm.EnableTargetSelection();
        }

        public override void SetTarget(Character selectedTarget)
        {
            this.target = selectedTarget; 
            Resolve();
        }
    }
}