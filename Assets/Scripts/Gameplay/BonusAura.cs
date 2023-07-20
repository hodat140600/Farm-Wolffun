using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Add this script to a construction or object, it will provide a bonus aura to characters around it.
    /// The bonus can either be an attribute increase (AttributeType), or a buff (BonusType)
    /// </summary>

    public class BonusAura : MonoBehaviour
    {
        public AttributeType attribute; //Which attribute in increased (set to None if using a bonus)
        public BonusType bonus;         //Bonus provided (set to None if using Attribute)
        public float value;             //Per game hour for attribute increase. Percentage must be represented in decimals (0.10 for 10%)
        public float range = 10f;       //Radius affected around the object

        private Construction _construction;

        private void Awake()
        {
            _construction = GetComponent<Construction>();
        }

        void Update()
        {
            if (TheGame.Get().IsPaused())
                return;

            foreach (Worker worker in Worker.GetAll())
            {
                float dist = (transform.position - worker.transform.position).magnitude;
                if (dist < range && !worker.IsDead())
                {
                    if (worker.Attributes != null && attribute != AttributeType.None)
                    {
                        float speed = TheGame.Get().GetGameTimeSpeed();
                        worker.Attributes.AddAttribute(attribute, value * speed * Time.deltaTime);
                    }

                    if (bonus != BonusType.None)
                    {
                        worker.Character.SetTempBonusEffect(bonus, value, 0.01f);
                    }
                }
            }

            foreach (Construction construction in Construction.GetAll())
            {
                if (bonus != BonusType.None)
                {
                    construction.BonusValue = _construction.GetBonusMult(BonusType.FactorySpeed);
                }
            }
        }
    }
}
