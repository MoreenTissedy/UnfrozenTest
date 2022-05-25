using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

namespace UnfrozenTest
{
    public class StatusBar : MonoBehaviour
    {
        [SerializeField] private float changeSpeed;
        [SerializeField] private Image healthBar;
        [SerializeField] private Text healthCount;
        [SerializeField] private GameObject drBar;
        [SerializeField] private Text DR;
        [SerializeField] private GameObject evasionBar;
        [SerializeField] private Text evasion;

        private CharacterData data;

        public void InitStatus(CharacterData data)
        {
            this.data = data;
            if (healthBar) healthBar.fillAmount = 1;
            if (healthCount) healthCount.text = $"{data.HP} / {data.HP}";
            if (drBar)
            {
                if (data.DR > 0)
                {
                    drBar.SetActive(true);
                    DR.text = data.DR.ToString();
                }
                else
                {
                    drBar.SetActive(false);
                }
            }

            if (evasionBar)
            {
                if (data.Evasion > 0)
                {
                    evasionBar.SetActive(true);
                    evasion.text = data.Evasion.ToString();
                }
                else
                {
                    evasionBar.SetActive(false);
                }
            }
        }

        public void UpdateHealth(int currentHP)
        {
            if (healthBar) healthBar.DOFillAmount((float) currentHP / data.HP, changeSpeed);
            if (healthCount) healthCount.text = $"{currentHP} / {data.HP}";
        }
        
        
    }
}