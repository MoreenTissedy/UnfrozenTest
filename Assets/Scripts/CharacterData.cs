using UnityEngine;

namespace UnfrozenTest
{
    /// <summary>
    /// This class holds the game design data for characters
    /// </summary>
    [CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 0)]
    public class CharacterData : ScriptableObject
    {
         [SerializeField] private int hp = 40, dr, evasion;
         [SerializeField] private Ability[] abilities;
         public Ability[] Abilities => abilities;
         public int HP => hp;
         public int DR => dr;
         public int Evasion => evasion;

         /// <summary>
         /// Calculates damage taken on hits. If any hit missed — returns -1.
         /// </summary>
         /// <param name="ability">Attack parameters</param>
         /// <param name="currentHP">Current HP of the character</param>
         /// <returns>damage taken</returns>
         public int[] CalculateDamageTaken(Ability ability, int currentHP)
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
                 int virtualHP = currentHP;
                 for (int k = 0; k < i; k++)
                 {
                     virtualHP -= (damageTaken[k] == -1) ? 0 : damageTaken[k];
                 }
                 damageTaken[i] = Mathf.Clamp(damageReceived - dr, 0, virtualHP);
             }
             string log = "Calculated damage: ";
             foreach (int dmg in damageTaken)
             {
                 log += $"{dmg}+";
             }
             Debug.Log(log.TrimEnd('+'));
             return damageTaken;
         }
    }
}