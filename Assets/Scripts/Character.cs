using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using Event = Spine.Event;

namespace UnfrozenTest
{
    public class Character : MonoBehaviour
    {
        [SerializeField][SpineEvent(dataField:"spineAnim", fallbackToTextField: true)] 
        private string hitEvent = "Hit";
        [SerializeField] [SpineSkin(dataField: "spineAnim", fallbackToTextField: true)]
        private string deathSkin = "blood";
        [SerializeField] private SkeletonAnimation spineAnim;
        [SerializeField] private AnimationSet animationSet;
        [SerializeField] private CharacterSelector selector;
        [SerializeField] private CharacterData data;
        [SerializeField] private StatusBar healthBar;
        [SerializeField] private MeshRenderer rend;
        public CharacterData Data => data;

        private int currentHP;
        public int CurrentHP => currentHP;

        private bool dead = false;
        private bool playerSide;
        public bool PlayerSide => playerSide;
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

            if (!healthBar) healthBar = GetComponentInChildren<StatusBar>();
            healthBar?.InitStatus(data);
            if (!rend) rend = GetComponentInChildren<MeshRenderer>();
            if (!spineAnim) spineAnim = GetComponentInChildren<SkeletonAnimation>();
            if (spineAnim is null)
            {
                Debug.LogError($"{gameObject.name} no Spine animation found in hierarchy!");
            }
            if (!selector) selector = GetComponentInChildren<CharacterSelector>();
            if (!(selector is null))
            {
                selector.Image.enabled = false;
            }
        }

        public void FlipX()
        {
            var localScale = spineAnim.gameObject.transform.localScale;
            localScale = new Vector3(-localScale.x,
                localScale.y,
                localScale.z);
            spineAnim.gameObject.transform.localScale = localScale;
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
            if (e.data.name == hitEvent)
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
            healthBar.UpdateHealth(currentHP);
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
            spineAnim.Skeleton.SetSkin(deathSkin);
            spineAnim.Skeleton.SetSlotsToSetupPose();
            spineAnim.AnimationState.Apply(spineAnim.skeleton);
            //spineAnim.AnimationState.End += ctx => Destroy(gameObject);
        }
        
    }
}