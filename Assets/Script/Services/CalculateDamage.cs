using Script.Characters.Enemy;
using Script.Characters.Player;
using Script.Game;
using Script.Logic;
using Script.Spawner;

using UnityEngine;


namespace Script.Services
{
    public class CalculateDamage : MonoBehaviour
    {
        [SerializeField] private PlayerHealth _playerHealth;
        [SerializeField] private PlayerDeath _playerDeath;
        [SerializeField] private PlayerSpawnerCards _playerSpawnerCards;

        [SerializeField] private EnemyDeath _enemyDeath;
        [SerializeField] private EnemyHealth _enemyHealth;
        [SerializeField] private EnemySpawnerCards EnemySpawnerCards;
        
        [SerializeField] private TurnBehaviour _turnBehaviour;
        public void DealDamageToCharacterDirectly(IHealth character, int damage)
        {
            character.TakeDamage(damage);
        }

        


        private int CalculateDamageToEnemyForActiveCards() 
        { 
            int damageDealt = 0;
            
            foreach (var activeCard in _playerSpawnerCards.Board.FindAll(x => x.CanAttack))
                damageDealt += activeCard.CharacterCard.manacost;
            return damageDealt; 
        }
        private int CalculateDamageToPlayerForActiveCards() 
        {
            int damageDealt = 0;

            foreach (var activeCard in EnemySpawnerCards.Board.FindAll(x => x.CanAttack))
                damageDealt += activeCard.CharacterCard.manacost;
            return damageDealt; 
        } 
        
        public void DealDamageToEnemyHero(int damage)
        {

            int currentEnemyHealth = _enemyHealth.CurrentHealth;
            _enemyHealth.TakeDamage(damage);
            _enemyHealth.Show();
            _enemyDeath.Death(ref currentEnemyHealth);

        }
        
        public void DealDamageToPlayerHero(int damage)
        {
            int currentPlayerHealth = _playerHealth.CurrentHealth;
            _playerHealth.TakeDamage(damage);
            _playerHealth.Show();
            _playerDeath.Death( currentPlayerHealth);
        }

        public void CheckAmountCardsForCalculateDamage()
        {
            if (_turnBehaviour.IsPlayerTurn)
                CalculateDamageForPlayerTurn();
            else
                CalculateDamageForEnemyTurn();
        }

        private void CalculateDamageForPlayerTurn()
        {
            if (_playerSpawnerCards.Board.Exists(x => x.CanAttack) && EnemySpawnerCards.Board.Count == 0)
            {
                int damageDealt = CalculateDamageToEnemyForActiveCards();
                if (damageDealt > 0)
                    DealDamageToEnemyHero(damageDealt);
            }
        }

        private void CalculateDamageForEnemyTurn()
        {
            if (EnemySpawnerCards.Board.Exists(x => x.CanAttack) && _playerSpawnerCards.Board.Count == 0)
            {
                int damageDealt = CalculateDamageToPlayerForActiveCards();
                if (damageDealt > 0)
                    DealDamageToPlayerHero(damageDealt);
            }
        }
    }
}