using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnfrozenTest
{
    /// <summary>
    /// UI button script
    /// </summary>
    public class ActionButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private ActionType type;
        [SerializeField] private Ability ability;
        [SerializeField] private GameLoop gameManager;
        [SerializeField] private Color activeColor = Color.cyan;
        [SerializeField] private Color selectedColor = Color.yellow;

        private Image image;

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameManager is null)
            {
                gameManager = FindObjectOfType<GameLoop>();
                //I stumbled upon a problem where changes made to prefab instance fields
                //from code were being overwritten by the prefab values. This is a solution.
                if (PrefabUtility.IsPartOfPrefabInstance(this))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                }
            }
        }
        #endif

        private void Start()
        {
            image = GetComponent<Image>();
            gameManager.OnNewTurn += () => image.color = activeColor;
            gameManager.OnNewAction += GameManagerOnOnNewAction;
        }

        private void GameManagerOnOnNewAction(ActionType type, Ability ability)
        {
            if (this.type != type || this.ability != ability)
            {
                image.color = activeColor;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            GetComponent<Image>().color = selectedColor;
            gameManager?.SelectAction(type, ability);
        }
    }
}