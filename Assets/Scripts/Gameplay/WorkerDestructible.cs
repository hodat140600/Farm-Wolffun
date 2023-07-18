using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FarmWolffun
{
    /// <summary>
    /// Updated version of Destructible, for Workers.
    /// Allow to link the HP to the Health attribute if the character is a worker
    /// </summary>

    [RequireComponent(typeof(Worker))]
    public class WorkerDestructible : Destructible
    {
        private Character character;
        private Worker _worker;

        protected override void Awake()
        {
            base.Awake();
            character = GetComponent<Character>();
            _worker = GetComponent<Worker>();
        }

        protected override void Update()
        {
            base.Update();


        }

        public override int HP
        {
            get {
                if (_worker.Attributes != null)
                    return (int) _worker.Attributes.GetAttributeValue(AttributeType.Health);
                return hp;
            }
            set
            {
                if (_worker.Attributes != null)
                    _worker.Attributes.SetAttribute(AttributeType.Health, value);
                hp = value;
            }
        }

        public override int MaxHP
        {
            get
            {
                if (_worker.Attributes != null)
                    return (int)_worker.Attributes.GetAttributeMax(AttributeType.Health);
                return max_hp;
            }
        }

        public override int Armor
        {
            get
            {
                float bonus = 1f + character.GetBonusValue(BonusType.ArmorPercent);
                int armor = Mathf.RoundToInt(this.armor * bonus) + Mathf.RoundToInt(character.GetBonusValue(BonusType.ArmorValue));
                return armor;
            }
        }
    }
}