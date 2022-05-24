using System;
using UnityEngine;

namespace UnfrozenTest
{
    /// <summary>
    /// I think this is called the 'Command' pattern. All character actions can be logged and reversed. Cool.
    /// </summary>
    public abstract class BattleAction
    {
        protected GameLoop gm;
        protected Character actor;
        public Character Actor => actor;
        public abstract event Action Done;

        protected BattleAction(Character actor, GameLoop gm)
        {
            this.actor = actor;
            this.gm = gm;
        }

        //this method does the actual work and raises the Done event
        public abstract void Resolve();
        //this method will be called after the constructor
        public abstract void Next();
        //this method is used to set the action target
        public abstract void SetTarget(Character selectedTarget);
    }
}