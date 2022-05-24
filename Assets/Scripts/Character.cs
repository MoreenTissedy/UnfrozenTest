using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;

namespace UnfrozenTest
{
    //TODO: healthbar
    [RequireComponent(typeof(SkeletonAnimation))]
    public class Character : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation spineAnim;
        [SerializeField] private AnimationSet animationSet;
        [SerializeField] private CharacterSelector selector;
        [SerializeField] private CharacterData data;
        public CharacterData Data => data;

        private int currentHP;
        public int CurrentHP => currentHP;

        private bool dead = false;
        private bool playerSide;
        public bool PlayerSide => playerSide;
        private MeshRenderer rend;
        public MeshRenderer Rend => rend;

        private int position;
        private GameLoop gm;
        
        public event Action HitTime;
        public event Action AnimationDone;

        public bool Dead
        {
            get => dead;
        }

        public void Init(bool playerSide, int position, GameLoop gm)
        {
            if (data is null)
            {
                Debug.LogError($"Specify dataset for character {gameObject.name}");
                return;
            }
            
            this.playerSide = playerSide;
            this.position = position;
            this.gm = gm;
            currentHP = data.HP;

            rend = GetComponent<MeshRenderer>();
            spineAnim = GetComponent<SkeletonAnimation>();
            selector = GetComponentInChildren<CharacterSelector>();
            if (!(selector is null))
            {
                selector.Image.enabled = false;
            }
        }

        
        public void Highlight(bool on)
        {
            if (selector is null)
                return;
            selector.Image.enabled = on;
        }

        public void EnableSelection(bool on)
        {
            if (selector is null)
                return;
            selector.EnableSelection(on);
        }

        public void Select()
        {
            gm.SelectTarget(this);
            EnableSelection(false);
        }

        public Character Animate(AnimationSetItems animationTag)
        {
            string animName = animationSet.Get(animationTag);
            if (animName != null)
            {
                spineAnim.AnimationState.SetAnimation(1, animName, false);
                spineAnim.AnimationState.AddEmptyAnimation(1, 0.2f, 0f);
                spineAnim.AnimationState.Event += AnimationStateOnEvent;
                spineAnim.AnimationState.End += AnimationStateOnEnd;
            }

            return this;
        }


        private void AnimationStateOnEnd(TrackEntry entry)
        {
            spineAnim.AnimationState.Event -= AnimationStateOnEvent;
            AnimationDone?.Invoke();
        }

        private void AnimationStateOnEvent(TrackEntry trackentry, Event e)
        {
            if (e.data.name == "Hit")
            {
                Debug.Log("Hit!");
                HitTime?.Invoke();
            }
        }

        public void Damage(int amount)
        {
            if (amount == -1)
            {
                Debug.Log("miss!");
                return;
            }
            Debug.Log($"{gameObject.name} took damage: {amount}");
            currentHP -= amount;
            spineAnim.AnimationState.SetAnimation(1, 
                animationSet.Get(AnimationSetItems.GetDamage), 
                false);
            spineAnim.AnimationState.AddEmptyAnimation(1, 0.2f, 0f);
            if (CurrentHP <= 0)
                Death();
        }

        public void Death()
        {
            Debug.Log($"{gameObject.name} died!");
            dead = true;
            gm.RemoveSoldier(this);
            spineAnim.Skeleton.SetSkin("blood");
            spineAnim.Skeleton.SetSlotsToSetupPose();
            spineAnim.AnimationState.Apply(spineAnim.skeleton);
            //spineAnim.AnimationState.End += ctx => Destroy(gameObject);
        }
        
    }
}