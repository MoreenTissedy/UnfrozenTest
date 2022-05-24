using System;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;
using Event = Spine.Event;
using Random = UnityEngine.Random;

namespace UnfrozenTest
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class Character : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation spineAnim;
        [SerializeField] private AnimationSet animationSet;
        [SerializeField] private int hp = 10, dr, evasion;
        [SerializeField] private Ability[] abilities;
        [SerializeField] private CharacterSelector selector;

        public Ability[] Abilities => abilities;

        private int currentHP;
        private bool dead = false;
        private bool playerSide;
        private MeshRenderer rend;
        public MeshRenderer Rend => rend;


        public bool PlayerSide => playerSide;

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
            this.playerSide = playerSide;
            this.position = position;
            this.gm = gm;
            currentHP = hp;

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


        /// <summary>
        /// Calculates damage taken on hits. If any hit missed — returns -1.
        /// </summary>
        /// <param name="ability">Attack parameters</param>
        /// <returns>damage taken</returns>
        public int[] CalculateDamageTaken(Ability ability)
        {
            int[] damageTaken = new int[ability.NumAttacks];
            for (int i = 0; i < ability.NumAttacks; i++)
            {
                int dice = Random.Range(0, 100);
                if (dice < 100 - ability.Accuracy + evasion)
                {
                    damageTaken[i] = -1;
                    continue;
                }
                int damageReceived = Random.Range(ability.MinDamage, ability.MaxDamage);
                int virtualHP = this.currentHP;
                for (int k = 0; k < i; k++)
                {
                    virtualHP -= (damageTaken[k] == -1) ? 0 : damageTaken[k];
                }
                damageTaken[i] = Mathf.Clamp(damageReceived - dr, 0, virtualHP);
            }
            string log = $"{gameObject.name} calculated damage: ";
            foreach (int dmg in damageTaken)
            {
                log += $"{dmg}+";
            }
            Debug.Log(log.TrimEnd('+'));
            return damageTaken;
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
            if (currentHP <= 0)
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