using UnityEngine;

namespace UnfrozenTest
{
    /// <summary>
    /// Data for different types of attacks
    /// </summary>
    [CreateAssetMenu(fileName = "New Ability", menuName = "Ability", order = 0)]
    public class Ability : ScriptableObject
    {
        [SerializeField] private int minDamage = 5, maxDamage = 10, numAttacks = 1, newAttackerPosition = -1;

        public int NewAttackerPosition => newAttackerPosition;
        public int MinDamage => minDamage;
        public int MaxDamage => maxDamage;
        public int NumAttacks => numAttacks;

        [SerializeField, Range(0, 100)] private int accuracy = 90;

        public int Accuracy => accuracy;

        [SerializeField] private AnimationSetItems animation;

        public AnimationSetItems Animation => animation;
    }
}