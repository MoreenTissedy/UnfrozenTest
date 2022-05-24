using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnfrozenTest
{
    public class GameLoop : MonoBehaviour
    {
        [Tooltip("Parameters to calculate character positions")]
        public float centralGap = 10, gap = 10;

        [Header("Battle squads (prefabs)")]
        public GameObject[] allies;
        public GameObject[] enemies;

        //TODO: remove debug
        public List<Character> playerSquad, enemySquad;
        private PositionManager positions;
        private Queue<Character> initiativeOrder;
        public Character currentCharacter;
        private BattleAction currentAction;

        private void Awake()
        {
            
            initiativeOrder = new Queue<Character>(PositionManager.squadLength * 2);
            //calculate battle positions
            positions = new PositionManager(centralGap, gap);
            
            //create squads, init character scripts and place characters accordingly
            playerSquad = CreateSquad(allies, positions.Allies, true, "Player Squad");
            enemySquad = CreateSquad(enemies, positions.Enemies, false, "Enemy Squad");
            
        }

        private void Start()
        {
            NewRound();    
        }

        void NewRound()
        {
            //form new initiative order
            initiativeOrder.Clear();
            List<Character> soldiers = new List<Character>(PositionManager.squadLength * 2);
            soldiers.AddRange(playerSquad);
            soldiers.AddRange(enemySquad);
            var totalSoldiers = soldiers.Count;
            for (int i = 0; i < totalSoldiers; i++)
            {
                int randomIndex = Random.Range(0, soldiers.Count);
                initiativeOrder.Enqueue(soldiers[randomIndex]);
                soldiers.RemoveAt(randomIndex);
            }
            StartTurn();
        }

        void StartTurn()
        {
            //check if one side is dead
            if (!(currentAction is null))
            {
                currentAction.Done -= StartTurn;
            }
            currentCharacter?.Highlight(false);
            if (initiativeOrder.Count == 0)
            {
                NewRound();
                return;
            }
            do
            {
                currentCharacter = initiativeOrder.Dequeue();
            } while (currentCharacter.Dead);
            //highlight current character for player
            currentCharacter.Highlight(true);
            //TODO display relevant UI with currentCharacter.Abilities
        }

        public void SelectAction(ActionType type, Ability ability = null)
        {
            Debug.Log(type);
            switch (type)
            {
                case ActionType.Wait:
                    currentAction = new WaitBattleAction(currentCharacter, this);
                    break;
                case ActionType.Offense:
                    if (ability is null)
                        return;
                    currentAction = new OffenseBattleAction(currentCharacter, ability, this);
                    break;
            }
            currentAction.Done += StartTurn;
            currentAction.Next();
        }

        public void WaitAction(Character character)
        {
            initiativeOrder.Enqueue(character);
        }

        public void SelectTarget(Character target)
        {
            currentAction.SetTarget(target);
            EnableTargetSelection(false);
        }

        public void EnableTargetSelection(bool on = true)
        {
            List<Character> opposite = currentCharacter.PlayerSide ? enemySquad : playerSquad;
            foreach (var enemy in opposite)
            {
                enemy.EnableSelection(on);
            }
        }

        List<Character> CreateSquad(GameObject[] prefabs, Vector3[] battlePositions, bool playerSide = true, string squadName = "Squad")
        {
            Transform parent = new GameObject(squadName).transform;
            List<Character> squad = new List<Character>(PositionManager.squadLength);
            for (var index = 0; index < prefabs.Length; index++)
            {
                var prefab = prefabs[index];
                if (prefab is null)
                    continue;
                GameObject soldier = Instantiate(prefab, battlePositions[index], Quaternion.identity);
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

                script.Init(playerSide, index, this);
                squad.Add(script);
            }

            return squad;
        }

        public void RemoveSoldier(Character character)
        {
            if (character.PlayerSide)
            {
                playerSquad.Remove(character);
            }
            else
            {
                enemySquad.Remove(character);
            }
            
        }
    }
}
