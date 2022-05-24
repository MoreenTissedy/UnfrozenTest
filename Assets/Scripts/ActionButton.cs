using UnityEngine;
using UnityEngine.EventSystems;

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

        private void OnValidate()
        {
            if (gameManager is null)
                gameManager = FindObjectOfType<GameLoop>();
        }


        public void OnPointerClick(PointerEventData eventData)
        {
            gameManager?.SelectAction(type, ability);
        }
    }
}