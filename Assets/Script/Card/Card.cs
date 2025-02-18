using System;
using System.Collections.Generic;
using Script.Card.CardEffects;
using Unity.VisualScripting;
using UnityEngine;

namespace Script.Card
{
    [CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
    public class Card : ScriptableObject
    {
        public new string name;
        [TextArea]  public string description;

        public  Types CardType; 

        public Sprite cardImage;

        public int hp;
        public int attack;
        public int manacost;
        [SerializeReference]
        public List<GameObject> Effects = new List<GameObject>();
        [System.Serializable]
        public enum Types
        {
            Tool, Man, Powers, Relic, Heroic
        };

        public void OnStart()
        {
             
            
        }
        
       public Types GetCardType()
        {

            switch (CardType)
            { 
                case Types.Tool:
                    return Types.Tool;
                    break;
                case Types.Man:
                    return Types.Man;
                    break;
                case Types.Heroic:
                    return Types.Heroic;
                    break;
                case Types.Powers:
                    return Types.Powers;
                    break;
                case Types.Relic:
                    return Types.Relic;
                    break;

            }

            return CardType;
        }
    }
}
