using System.Collections;
using System.Collections.Generic;
using Script.Card;
using Script.Card.CardEffects;
using Script.Game;
using Script.Spawner;
using UnityEngine;

namespace Script.Characters.Enemy
{
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField]
        private TurnBehaviour _turnBehaviour;

        [SerializeField]
        private EnemySpawnerCards EnemySpawnerCards;
        [SerializeField]
        private PlayerSpawnerCards PlayerSpawnerCards;
        [SerializeField]
        private EnemyMana _enemyMana;
        
        [SerializeField]
        private BattleBehaviour _battleBehaviour;
        
        public Transform EnemyField;

        public void MakeTurn() => StartCoroutine(EnemyTurn(EnemySpawnerCards.EnemyHandCards));

        IEnumerator EnemyTurn(List<CardInfoDisplay> cards)
        {
            yield return new WaitForSeconds(1);

            int count = cards.Count == 1 ? 1 :
                Random.Range(0, cards.Count);

            yield return ProcessCards(cards, count);

            yield return new WaitForSeconds(1);

            yield return HandleActiveCardsAttack();
            
            yield return new WaitForSeconds(1);
            _turnBehaviour.ChangeTurn();
        }
        
        private IEnumerator HandleActiveCardsAttack()
        {
            foreach (var activeCard in EnemySpawnerCards.Board.FindAll(x => x.CanAttack))
            {
                CardMoveAnimation activeCardMoveAnimation = activeCard.GetComponent<CardMoveAnimation>();

                if (PlayerSpawnerCards.Board.Count != 0)
                {
                    var enemy = PlayerSpawnerCards.Board[Random.Range(0, PlayerSpawnerCards.Board.Count)];

                    activeCard.ChangeAttackState(false);
                    activeCardMoveAnimation.MovetoTarget(enemy.transform);
                    yield return new WaitForSeconds(.75f);

                    _battleBehaviour.CardAttacking(enemy, activeCard);
                }

                yield return new WaitForSeconds(.2f);
            }
        }
        
        private IEnumerator ProcessCards(List<CardInfoDisplay> cards, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (EnemySpawnerCards.Board.Count > 5 || _enemyMana.CurrentEnemyMana == 0 || EnemySpawnerCards.EnemyHandCards.Count == 0)
                    break;

                List<CardInfoDisplay> cardList = cards.FindAll(x => _enemyMana.CurrentEnemyMana >= x.CharacterCard.manacost);

                if (cardList.Count == 0)
                    break;

                cardList[0].GetComponent<CardMoveAnimation>().MovetoField(EnemyField);
                _enemyMana.ReduceMana(cardList[0].CharacterCard.manacost);
                CardEffectHandler.OnBeingPlayed.Invoke(cardList[0]);

                yield return new WaitForSeconds(.51f);

                cardList[0].ShowCardInfo(cardList[0].CharacterCard, false);
                cardList[0].transform.SetParent(EnemyField);

                EnemySpawnerCards.Board.Add(cardList[0]);
                EnemySpawnerCards.EnemyHandCards.Remove(cardList[0]);
            }
        }
    }
}
