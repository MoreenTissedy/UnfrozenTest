using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnfrozenTest
{
    //This is a 'main' class that handles the game loop itself.
    [RequireComponent(typeof(BattleSetup))]
    public class GameLoop : MonoBehaviour
    {
        private PositionManager positions;
        [Tooltip("Parameters to calculate character positions")]
        public float centralGap = 10, gap = 10;

        private List<Character> playerSquad, enemySquad;
        private Queue<Character> initiativeOrder;
        private Character currentCharacter;
        private BattleAction currentAction;
        private BattleSetup setup;

        public event Action OnNewRound;
        public event Action OnNewTurn;
        public event Action<ActionType, Ability> OnNewAction;

        private void Awake()
        {
            setup = GetComponent<BattleSetup>();
            initiativeOrder = new Queue<Character>(PositionManager.squadLength * 2);
            //calculate battle positions
            positions = new PositionManager(centralGap, gap);
            
            //create squads, init character scripts and place characters accordingly
            playerSquad = setup.CreateSquad(positions.Allies, true, "Player Squad", this);
            enemySquad = setup.CreateSquad(positions.Enemies, false, "Enemy Squad", this);
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
            OnNewRound?.Invoke();
            StartTurn();
        }

        void StartTurn()
        {
            //TODO: check if one side is dead
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
            
            //basic AI
            BasicAI();
            
            OnNewTurn?.Invoke();
            //TODO display relevant UI with currentCharacter.Abilities
        }

        private void BasicAI()
        {
            if (!currentCharacter.PlayerSide)
            {
                StartCoroutine(AITurn());
            }
        }

        private IEnumerator AITurn()
        {
            //Hit the closest enemy with the first ability.
            //Small delay for smoother gameplay and to avoid bugs.
            yield return new WaitForSeconds(0.5f);
            currentAction = new OffenseBattleAction(currentCharacter, currentCharacter.Data.Abilities[0], this);
            currentAction.Done += StartTurn;
            SelectTarget(playerSquad[0]);
        }

        public void SelectAction(ActionType type, Ability ability = null)
        {
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
            OnNewAction?.Invoke(type, ability);
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
