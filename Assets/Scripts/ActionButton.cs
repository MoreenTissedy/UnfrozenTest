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
        [SerializeField] private Color activeColor = Color.cyan;
        [SerializeField] private Color selectedColor = Color.yellow;

        private GameLoop gameManager;
        private Image image;

        private void Start()
        {
            image = GetComponent<Image>();
            gameManager = GameLoop.instance;
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