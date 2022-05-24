using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UnfrozenTest
{
    public class CharacterSelector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private SpriteRenderer image;

        public SpriteRenderer Image => image;

        [SerializeField] private Character character;
        private bool selectable = false;

        private void Awake()
        {
            image = GetComponent<SpriteRenderer>();
            character = GetComponentInParent<Character>();
        }

        public void EnableSelection(bool on)
        {
            selectable = on;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("click!");
            if (!selectable) return;
            image.enabled = false;
            character.Select();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!selectable) return;
            image.enabled = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!selectable) return;
            image.enabled = false;
        }
    }
}