using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    [RequireComponent(typeof(Worker))]
    public class WorkerAttribute : MonoBehaviour
    {
        [Header("Attributes")]
        public AttributeData[] attributes;  //List of available attributes

        [Header("Auto Eat")]
        public bool auto_eat = true;        //Will the worker try to eat automatically when hungry?
        public GroupData food_group;        //Food item group for the auto eat

        private Character character;
        private Worker _worker;
        private Destructible destruct;

        private float move_speed_mult = 1f;
        private float gather_mult = 1f;
        private bool depleting = false;

        private void Awake()
        {
            character = GetComponent<Character>();
            _worker = GetComponent<Worker>();
            destruct = GetComponent<Destructible>();
        }

        void Start()
        {
            //Init attributes
            foreach (AttributeData attr in attributes)
            {
                if (!CharacterData.HasAttribute(attr.type))
                    CharacterData.SetAttributeValue(attr.type, attr.start_value, attr.max_value);
            }

            //Events
            destruct.onDeath += OnDeath;
        }

        void Update()
        {
            if (TheGame.Get().IsPaused())
                return;

            if (character.IsDead())
                return;

            //Update attributes
            float game_speed = TheGame.Get().GetGameTimeSpeed();

            //Update Attributes
            foreach (AttributeData attr in attributes)
            {
                float update_value = attr.value_per_hour;
                update_value = update_value * game_speed * Time.deltaTime;
                CharacterData.AddAttributeValue(attr.type, update_value, attr.max_value);
            }

            //Penalty for depleted attributes
            move_speed_mult = 1f;
            gather_mult = 1f;
            depleting = false;

            foreach (AttributeData attr in attributes)
            {
                if (GetAttributeValue(attr.type) < 0.01f)
                {
                    move_speed_mult = move_speed_mult * attr.deplete_move_mult;
                    gather_mult = gather_mult * attr.deplete_gather_mult;
                    float update_value = attr.deplete_hp_loss * game_speed * Time.deltaTime;
                    AddAttribute(AttributeType.Health, update_value);
                    if (attr.deplete_hp_loss < 0f)
                        depleting = true;
                }
            }

            UpdateAutoActions();

            //Update hp
            if (destruct != null)
                destruct.hp = Mathf.RoundToInt(GetAttributeValue(AttributeType.Health));

            //Dying
            float health = GetAttributeValue(AttributeType.Health);
            if (health < 0.01f)
                character.Kill();

        }

        private void UpdateAutoActions()
        {
            if (_worker.Character.IsWaiting())
                return;

            //Auto Eat
            if (auto_eat && HasAttribute(AttributeType.Hunger) && !Character.IsSleeping())
            {
                if (IsLow(AttributeType.Hunger))
                {
                    Inventory global = Inventory.GetGlobal();
                    _worker.UseItem(global, food_group, AttributeType.Hunger);
                }
            }
        }

        private void OnDeath()
        {
            CharacterData.SetAttributeValue(AttributeType.Health, 0f, 0f);
        }

        public void AddAttribute(AttributeType type, float value)
        {
            if (HasAttribute(type) && !character.IsDead())
            {
                CharacterData.AddAttributeValue(type, value, GetAttributeMax(type));
            }
        }

        public void SetAttribute(AttributeType type, float value)
        {
            if (HasAttribute(type) && !character.IsDead())
            {
                CharacterData.SetAttributeValue(type, value, GetAttributeMax(type));
            }
        }

        public bool HasAttribute(AttributeType type)
        {
            return CharacterData.HasAttribute(type) && GetAttribute(type) != null;
        }

        public float GetAttributeValue(AttributeType type)
        {
            return CharacterData.GetAttributeValue(type);
        }

        public float GetAttributeMax(AttributeType type)
        {
            AttributeData adata = GetAttribute(type);
            if (adata != null)
                return adata.max_value;
            return 100f;
        }

        public AttributeData GetAttribute(AttributeType type)
        {
            foreach (AttributeData attr in attributes)
            {
                if (attr.type == type)
                    return attr;
            }
            return null;
        }

        public bool IsLow(AttributeType type)
        {
            AttributeData attr = GetAttribute(type);
            float val = GetAttributeValue(type);
            return (val <= attr.low_threshold);
        }

        public bool IsDepleted(AttributeType type)
        {
            float val = GetAttributeValue(type);
            return (val <= 0f);
        }

        public bool IsAnyDepleted()
        {
            foreach (AttributeData attr in attributes)
            {
                float val = GetAttributeValue(attr.type);
                if (val <= 0f)
                    return true;
            }
            return false;
        }

        public float GetSpeedMult()
        {
            return Mathf.Max(move_speed_mult, 0.01f);
        }

        public float GetGatherMult()
        {
            return Mathf.Max(gather_mult, 0.01f);
        }

        public bool IsDepletingHP()
        {
            return depleting;
        }

        public string GetStatusText()
        {
            List<string> tlist = new List<string>();
            foreach (AttributeData attr in attributes)
            {
                if (IsLow(attr.type))
                    tlist.Add(attr.low_status);
            }
            return string.Join(", ", tlist.ToArray());
        }

        public SaveWorkerData CharacterData
        {
            get { return _worker.SData; }
        }

        public Character Character { get { return character; } }
        public Worker worker { get { return _worker; } }
    }
}
