using EasyButtons;
using Script.UI.Buttons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.UI.Panel
{
    public class LosePanel : MonoBehaviour, IPanel
    { 
        [SerializeField]
        public Image  enemyGameOver;
        [SerializeField]
        private RestartButton _restartTurnButton;
        

        [Button]
        public void Show()
        {
            Time.timeScale = 0;
            enemyGameOver.gameObject.SetActive(true);
            _restartTurnButton.gameObject.SetActive(true);

        }
    }
}