﻿using Script.Card.CardEffects;
using Script.Spawner;
using UnityEngine;

namespace Script.Card
{
    public class CardDeath : MonoBehaviour
    {   
        [SerializeField]
        private PlayerSpawnerCards _playerSpawnerCards;
        [SerializeField]
        private EnemySpawnerCards _enemySpawnerCards;

        public void DestroyCard(CardInfoDisplay card)
        { 
            CardEffectHandler.OnDeath.Invoke(card);
            
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (card.TryGetComponent<SisyphusEffect>(out var sisyphusEffect_) == false)
            {
                card.GetComponent<CardMove>().OnEndDrag(null);

                if (_enemySpawnerCards.Board.Exists(x => x == card))
                    _enemySpawnerCards.Board.Remove(card);

                if (_playerSpawnerCards.Board.Exists(x => x == card))
                    _playerSpawnerCards.Board.Remove(card);

                if (chechForMiracle(card) == null)
                {
                    Destroy(card.gameObject);
                }
                else
                {
                    card.Heal(card.MaxHp);
                }
            }
            else if (card.TryGetComponent<SisyphusEffect>(out var sisyphusEffect) == true)
            {
                if (_playerSpawnerCards.Board.Contains(card))
                {
                    sisyphusEffect.SisyphusRevive(_playerSpawnerCards.GetCardOfType(Card.Types.Powers));
                }
                else if (_enemySpawnerCards.Board.Contains(card))
                {
                    sisyphusEffect.SisyphusRevive(_enemySpawnerCards.GetCardOfType(Card.Types.Powers));
                }
            }
        }

        private Miracle chechForMiracle(CardInfoDisplay card)
        {
            if (card.TryGetComponent<Miracle>(out var thisMiracle) == true)
            {
                return thisMiracle;
            }
            else
            {
                return null;
            }
        }
    }
}