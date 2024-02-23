using Script.Card.CardEffects;
using System.Collections;
using Script.Card.CardEffects;
using Script.Characters.Enemy;
using Script.Characters.Player;
using Script.Services;
using Script.Spawner;
using Script.UI;
using Script.UI.Buttons;
using UnityEngine;

namespace Script.Game
{
    public class TurnBehaviour : MonoBehaviour
    {
        private int _turn;
        private int _maxMana = 1;
        
        [Header("Player")]
        [SerializeField] private PlayerSpawnerCards PlayerSpawnerCards;
        [SerializeField] private PlayerMana _playerMana;
        
        [Header("Enemy")]
        [SerializeField] private EnemySpawnerCards EnemySpawnerCards;
        [SerializeField] private EnemyAI _enemyAI;
        [SerializeField] private EnemyMana _enemyMana;

        [SerializeField] private UiTimer TimerReset;
        [SerializeField] private EndTurnButton _endTurnButton;
        [SerializeField] private CalculateDamage _calculateDamage;

        public bool IsPlayerTurn => _turn % 2 == 0;

        private void Start()
        {
            _turn = 0;
            StartCoroutine(TurnFunc());
            
        }
        
        IEnumerator TurnFunc()
        {
            CardEffectHandler.OnTurnStart.Invoke();
            
                PrepareTurn();
            
            
            if (IsPlayerTurn)
                HandlePlayerTurn();
            else
                HandleEnemyTurn();
            
            foreach (var p in TimerReset.NextTurnTime())
                yield return p;
            ChangeTurn();
        }

        public void ChangeTurn()
        {
            StopAllCoroutines();
            TurnEnd();

            
            _calculateDamage.CheckAmountCardsForCalculateDamage();
            _turn++;

            _endTurnButton.EndTurn.interactable = IsPlayerTurn;
            
            if (IsPlayerTurn)
            {
                PlayerSpawnerCards.GiveNewCards();
                EnemySpawnerCards.GiveNewCards(); 
                UpdateMana();
                _playerMana.ShowMana();
                _enemyMana.ShowMana();
            }
            StartCoroutine(TurnFunc());
        }

        private void TurnEnd()
        {
            CardEffectHandler.OnTurnEnd.Invoke();
            Debug.Log("Turn ended");

        }

        private void HandlePlayerTurn()
        {
            foreach (var card in PlayerSpawnerCards.PlayerFieldCards)
            {
                card.ChangeAttackState(true);
                card.HighlightCard();
            }
        }

        private void HandleEnemyTurn()
        {
            foreach (var card in EnemySpawnerCards.EnemyFieldCards)
                card.ChangeAttackState(true);

            _enemyAI.MakeTurn();
        }

        private void PrepareTurn()
        {
            TimerReset.ResetTime();
            foreach (var card in PlayerSpawnerCards.PlayerFieldCards)
                card.DeHighlightCard();

            CheckCardsForAvailability();
        }



        private void UpdateMana()
        {
            _maxMana = Mathf.Min(_maxMana + 1, 10);
            _playerMana.CurrentPlayerMana = _enemyMana.CurrentEnemyMana = _maxMana;
        }

        public void CheckCardsForAvailability()
        {
            foreach (var card in PlayerSpawnerCards.PlayerHandCards)
                card.CheckForAvailability(_playerMana.CurrentPlayerMana);
        }
        public void HighlightTargets(bool highlight)
        {
            foreach (var card in EnemySpawnerCards.EnemyFieldCards)
                card.HighlightAsTarget(highlight);
        }



    }
}