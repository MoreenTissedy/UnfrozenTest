using System.Collections.Generic;
using UnityEngine;

namespace UnfrozenTest
{
    public class BattleSetup : MonoBehaviour
    {
        [Header("Battle squads (prefabs)")] public GameObject[] allies;
        public GameObject[] enemies;

        public List<Character> CreateSquad(Vector3[] battlePositions, bool playerSide, string squadName, GameLoop gm)
        {
            GameObject[] prefabs = playerSide ? allies : enemies;
            Transform parent = new GameObject(squadName).transform;
            List<Character> squad = new List<Character>(PositionManager.squadLength);
            int position = 0;
            foreach (var prefab in prefabs)
            {
                if (!prefab)
                {
                    continue;
                }
                
                GameObject soldier = Instantiate(prefab, battlePositions[position], Quaternion.identity);
                soldier.transform.SetParent(parent);

                Character script = soldier.GetComponent<Character>();
                if (script is null)
                {
                    Destroy(soldier);
                    continue;
                }

                position++;
                if (!playerSide)
                {
                    script.FlipX();
                }
                script.Init(playerSide, position, gm);
                squad.Add(script);
            }

            return squad;
        }
    }
}