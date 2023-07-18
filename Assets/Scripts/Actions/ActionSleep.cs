using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Action to havest resources
    /// </summary>
    
    [CreateAssetMenu(fileName = "Action", menuName ="Farm/Actions/Sleep", order = 50)]
    public class ActionSleep : ActionBasic
    {
        public float health_increase; //Per game hour
        public float energy_increase;
        public string anim;

        public override void StartAction(Character character, Interactable target)
        {
            character.Animate(anim, true);
        }

        public override void StopAction(Character character, Interactable target)
        {
            character.Animate(anim, false);
        }

        public override void UpdateAction(Character character, Interactable target)
        {
            if (character.worker != null && character.worker.Attributes != null)
            {
                float speed = TheGame.Get().GetGameTimeSpeed();
                character.worker.Attributes.AddAttribute(AttributeType.Health, health_increase * speed * Time.deltaTime);
                character.worker.Attributes.AddAttribute(AttributeType.Energy, energy_increase * speed * Time.deltaTime);
            }
        }

        public override bool CanDoAction(Character character, Interactable target)
        {
            if (character.worker != null && character.worker.Attributes != null)
                return !character.worker.Attributes.IsDepleted(AttributeType.Hunger);
            return true;
        }
    }
}
