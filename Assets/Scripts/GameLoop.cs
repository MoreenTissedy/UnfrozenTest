using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UnfrozenTest
{
    //This is a 'main' class that handles the game loop itself.
    [RequireComponent(typeof(BattleSetup))]
    public class GameLoop : MonoBehaviour
    {
        public static GameLoop instance;
        
        private PositionManager positions;
        [Tooltip("Parameters to calculate character positions")]
        [SerializeField] private float centralGap = 10, gap = 10;
        [Tooltip("Camera movement step")]
        [SerializeField] private float cameraMove = 1, cameraSpeed = 1f;
        [SerializeField] private Text winText;

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
            if (instance is null)
            {
                instance = this;
            }
            else
            {
                Debug.LogWarning($"Double singleton on scene: {this.GetType()}");
            }
            setup = GetComponent<BattleSetup>();
            initiativeOrder = new Queue<Character>(PositionManager.squadLength * 2);
            if (winText) winText.text = String.Empty;
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

        public void Move(Character target, int newPosition)
        {
            if (newPosition >= PositionManager.squadLength || newPosition < 0)
            {
                return;
            }
            if (target.PlayerSide)
            {
                if (!playerSquad.Contains(target))
                {
                    return;
                }
                playerSquad.Remove(target);
                playerSquad.Insert(0, target);
            }
            else
            {
                if (!enemySquad.Contains(target))
                {
                    return;
                }
                enemySquad.Remove(target);
                enemySquad.Insert(0, target);
            }
        }

        private void UpdatePositions()
        {
            for (var i = 0; i < enemySquad.Count; i++)
            {
                enemySquad[i].Move(i, positions);
            }

            for (var i = 0; i < playerSquad.Count; i++)
            {
                playerSquad[i].Move(i, positions);
            }
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
            UpdatePositions();
            if (!(currentAction is null))
            {
                currentAction.Done -= StartTurn;
            }
            currentCharacter?.Highlight(false);
            if (WinCheck())
            {
                return;
            }
            if (initiativeOrder.Count == 0)
            {
                NewRound();
                return;
            }
            currentCharacter = initiativeOrder.Dequeue();
            //highlight current character for player
            currentCharacter.Highlight(true);
            //move camera to the side depending on the current character
            int k = currentCharacter.PlayerSide ? -1 : 1;
            Camera.main.transform.DOMoveX(k*cameraMove, cameraSpeed).SetEase(Ease.InOutCirc);
            //basic AI
            BasicAI();
            
            OnNewTurn?.Invoke();
            //TODO display relevant UI with currentCharacter.Abilities
        }

        private bool WinCheck()
        {
            if (enemySquad.Count == 0)
            {
                if (winText) {winText.text = "You win!";}
                return true;
            }
            else if (playerSquad.Count == 0)
            {
                if (winText) {winText.text = "You lose";}
                return true;
            }
            return false;
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
            currentAction = new OffenseBattleAction(currentCharacter, currentCharacter.Data.Abilities[0]);
            currentAction.Done += StartTurn;
            SelectTarget(playerSquad[0]);
        }

        public void SelectAction(ActionType type, Ability ability = null)
        {
            switch (type)
            {
                case ActionType.Wait:
                    currentAction = new WaitBattleAction(currentCharacter);
                    break;
                case ActionType.Offense:
                    if (ability is null)
                        return;
                    currentAction = new OffenseBattleAction(currentCharacter, ability);
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

            if (initiativeOrder.Contains(character))
            {
                initiativeOrder = new Queue<Character>(initiativeOrder.Where(x => x != character));
            }
        }
    }
}
