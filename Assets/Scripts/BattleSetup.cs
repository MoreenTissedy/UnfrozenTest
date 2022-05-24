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
            for (var index = 0; index < prefabs.Length; index++)
            {
                var prefab = prefabs[index];
                if (!prefab)
                {
                    continue;
                }
                GameObject soldier = Instantiate(prefab, battlePositions[position], Quaternion.identity);
                position++;
                soldier.transform.SetParent(parent);
                if (!playerSide)
                {
                    //flipX enemies
                    var localScale = soldier.transform.localScale;
                    localScale = new Vector3(-localScale.x,
                        localScale.y,
                        localScale.z);
                    soldier.transform.localScale = localScale;
                }

                Character script = soldier.GetComponent<Character>();
                if (script is null)
                {
                    Debug.LogError($"No Character script on prefab {soldier.name}");
                    continue;
                }

                script.Init(playerSide, position, gm);
                squad.Add(script);
            }

            return squad;
        }
    }
}