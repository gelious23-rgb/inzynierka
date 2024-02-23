using UnityEngine;

namespace Script.Card.CardEffects
{
    public class Indignation : Effect
    {
        private int atkCount = 0;
         
        
        public override void OnAttack(CardInfoDisplay target, CardInfoDisplay self)
        {
            if(self == GetCard())
            if (atkCount < 2)
            {
                atkCount++;
                self.CanAttack = true;
            }
           
        }
        protected override void OnTurnEnd()
        {
            base.OnTurnEnd();
        }
    }
}
