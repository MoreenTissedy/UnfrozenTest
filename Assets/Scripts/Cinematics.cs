using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UnfrozenTest
{
    public class Cinematics:MonoBehaviour
    {
        public static Cinematics instance;
        public event Action Done;
        private int currentAttackCount = 0;
        private OffenseBattleAction currentAction;
        [SerializeField] private Image fadeScreen;
        [SerializeField][SortingLayerProperty] private string sortingLayer;

        [Header("Position parameters")] 
        [SerializeField] private float size = 8; 
        [SerializeField] private float gap = 5;
        [SerializeField] private float growShrinkDuration = 1f;

        private string prevSortingLayer;
        private Vector3 prevActorScale, prevTargetScale, prevActorPos, prevTargetPos;
        

        private void Awake()
        {
            if (instance is null)
                instance = this;
            fadeScreen.enabled = false;
        }

        public void Play(OffenseBattleAction action)
        {
            currentAction = action;
            currentAttackCount = 0;
            //fade
            fadeScreen.enabled = true;
            //change sorting & enlarge actors, save previous parameters
            prevSortingLayer = action.Actor.Rend.sortingLayerName;
            action.Actor.Rend.sortingLayerName = sortingLayer;
            action.Target.Rend.sortingLayerName = sortingLayer;
            prevActorScale = action.Actor.transform.localScale;
            prevTargetScale = action.Target.transform.localScale;
            prevActorPos = action.Actor.transform.position;
            prevTargetPos = action.Target.transform.position;
            action.Actor.transform.DOScale(prevActorScale * size, growShrinkDuration);
            action.Target.transform.DOScale(prevTargetScale * size, growShrinkDuration);
            action.Actor.transform.DOMoveX(action.Actor.PlayerSide ? -gap
                : gap, growShrinkDuration);
            action.Target.transform.DOMoveX(action.Target.PlayerSide ? -gap
                : gap, growShrinkDuration);
            
            //start attack animation
            action.Actor.Animate(action.Ability.Animation).HitTime += OnHitTime;
            action.Actor.AnimationDone += OnAnimationEnd;
        }

        private void OnAnimationEnd()
        {
            currentAction.Actor.HitTime -= OnHitTime;
            //change sorting & shrink actors
            currentAction.Actor.Rend.sortingLayerName = prevSortingLayer;
            currentAction.Target.Rend.sortingLayerName = prevSortingLayer;
            currentAction.Actor.transform.DOScale(prevActorScale, growShrinkDuration);
            currentAction.Target.transform.DOScale(prevTargetScale, growShrinkDuration);
            currentAction.Actor.transform.DOMove(prevActorPos, growShrinkDuration);
            currentAction.Target.transform.DOMove(prevTargetPos, growShrinkDuration);
            
            fadeScreen.enabled = false;
            Done?.Invoke();
        }

        private void OnHitTime()
        {
            if (currentAction.Ability.NumAttacks <= currentAttackCount)
                return;
            currentAction.Target.Damage(currentAction.damageDone[currentAttackCount]);
            currentAttackCount++;
        }
    }
}