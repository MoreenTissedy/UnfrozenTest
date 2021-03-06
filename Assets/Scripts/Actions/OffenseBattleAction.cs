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

        public OffenseBattleAction(Character actor, Ability ability) : base(actor)
        {
            this.ability = ability;
        }

        public override void Resolve()
        {
            damageDone = Target.Data.CalculateDamageTaken(ability, Target.CurrentHP);
            Cinematics.instance.Play(this);
            Cinematics.instance.Done += CinematicsDone;
        }

        void CinematicsDone()
        {
            Cinematics.instance.Done -= CinematicsDone;
            if (ability.NewAttackerPosition >= 0)
            {
                gm.Move(actor, ability.NewAttackerPosition);
            }
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