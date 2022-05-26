using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UnfrozenTest
{
    /// <summary>
    /// This singleton class does all the cinematic effects on a battle action. 
    /// </summary>
    public class Cinematics: MonoBehaviour
    {
        public static Cinematics instance;
        public event Action Done;
        private int currentAttackCount = 0;
        private OffenseBattleAction currentAction;
        [SerializeField] private Image fadeScreen;
        [SerializeField][SortingLayerProperty] private string sortingLayer;
        [SerializeField] private float cameraTargetZoom = 7f;
        [SerializeField] private Canvas UI;

        [Header("Position parameters")] 
        [SerializeField] private float size = 8; 
        [SerializeField] private float gap = 5;
        [SerializeField] private float growShrinkDuration = 1f;

        private string prevSortingLayer;
        private Vector3 prevActorScale, prevTargetScale, prevActorPos, prevTargetPos;
        private float cameraInitialZoom;
        

        private void Awake()
        {
            if (instance is null)
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning($"Double singleton on scene: {this.GetType()}");
            }
            fadeScreen.enabled = false;
        }

        public void Play(OffenseBattleAction action)
        {
            currentAction = action;
            currentAttackCount = 0;
            action.Actor.Highlight(false);
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
            //zoom in with camera, hide UI
            cameraInitialZoom = Camera.main.orthographicSize;
            Camera.main.DOOrthoSize(cameraTargetZoom, growShrinkDuration);
            UI.enabled = false;
            
            //start attack animation
            action.Actor.Animate(action.Ability.Animation).HitTime += OnHitTime;
            action.Actor.AnimationDone += OnAnimationEnd;
        }

        private void OnAnimationEnd()
        {
            currentAction.Actor.HitTime -= OnHitTime;
            currentAction.Actor.AnimationDone -= OnAnimationEnd;
            currentAction.Actor.transform.DOScale(prevActorScale, growShrinkDuration);
            currentAction.Actor.transform.DOMove(prevActorPos, growShrinkDuration);
            if (!currentAction.Target.Dead)
            {
                currentAction.Target.transform.DOScale(prevTargetScale, growShrinkDuration);
                currentAction.Target.transform.DOMove(prevTargetPos, growShrinkDuration);
            }
            //zoom out
            Camera.main.DOOrthoSize(cameraInitialZoom, growShrinkDuration);
            StartCoroutine(CinematicsDone());
        }

        private IEnumerator CinematicsDone()
        {
            yield return new WaitForSeconds(growShrinkDuration);
            //change sorting
            currentAction.Actor.Rend.sortingLayerName = prevSortingLayer;
            currentAction.Target.Rend.sortingLayerName = prevSortingLayer;
            
            UI.enabled = true;
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