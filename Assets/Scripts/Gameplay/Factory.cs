using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Factories are building that produce things. Either by itself or by assigning workers to it.
    /// </summary>

    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(Workable))]
    [RequireComponent(typeof(UniqueID))]
    public class Factory : MonoBehaviour
    {
        public float worker_work_speed = 1f;        //Speed the factory's worker produces, in quantity per game-hour
        public float auto_work_speed = 0f;          //Speed the factory produces by itself, in quantity per game-hour

        [Header("Items")]
        public int item_quantity = 1;               //Quantity produced each time
        public CraftData default_item;              //Item selected at start
        public CraftData[] items;                   //List of available items that can be produced

        public bool produce_one_by_one;
        [ShowIf("produce_one_by_one")] 
        public float time_product_out_of_date = 1f; //Time product out of date, in game-hour
        
        private Selectable selectable;
        private Interactable interact;
        private Inventory inventory; //Can be null
        private Construction construction; //Can be null
        private Workable workable;
        private UniqueID uid;

        private CraftData selected_item = null;
        private float progress = 0f;

        private static List<Factory> factory_list = new List<Factory>();
        private Dictionary<float, ItemData> items_craft_time = new Dictionary<float, ItemData>();

        void Awake()
        {
            factory_list.Add(this);
            selectable = GetComponent<Selectable>();
            interact = GetComponent<Interactable>();
            construction = GetComponent<Construction>();
            inventory = GetComponent<Inventory>();
            workable = GetComponent<Workable>();
            uid = GetComponent<UniqueID>();
        }

        private void OnDestroy()
        {
            factory_list.Remove(this);
        }

        private void Start()
        {
            if (default_item != null)
            {
                selected_item = default_item;
            }

            //Set saved selection
            if (uid.HasString("selection"))
            {
                CraftData cdata = CraftData.Get(uid.GetString("selection"));
                if(cdata != null)
                    selected_item = cdata;
            }

            if (uid.HasFloat("progress"))
            {
                progress = uid.GetFloat("progress");
            }
        }
        
        void Update()
        {
            if (TheGame.Get().IsPaused())
                return;

            if (selected_item != null)
            {
                if (CanProduce()/* && IsValidProduce()*/)
                {
                    float gspeed = TheGame.Get().GetGameTimeSpeed();
                    progress += AutoWorkSpeed * gspeed * Time.deltaTime;
                    uid.SetFloat("progress", progress);

                    float duration = selected_item.craft_duration;
                    if (progress >= duration)
                    {
                        CompleteWork();
                    }
                }
            }
            CheckOutOfDateProduct();

            //save selection
            string sel_id = selected_item != null ? selected_item.id : "";
            uid.SetString("selection", sel_id);
        }

        public void SelectItem(CraftData item)
        {
            selected_item = item;
            SetProgress(0f);
        }

        public void UnselectItem()
        {
            selected_item = null;
            SetProgress(0f);
        }

        public void AddWorkerProgress(float speed = 1f)
        {
            if (CanProduce())
            {
                float gspeed = TheGame.Get().GetGameTimeSpeed();
                progress += worker_work_speed * gspeed * speed * Time.deltaTime;
                uid.SetFloat("progress", progress);
            }
        }

        public void CompleteWork()
        {
            if (selected_item != null)
            {
                SetProgress(0f);

                Inventory global = Inventory.GetGlobal();
                global.PayCraftCost(selected_item, item_quantity);

                if (selected_item is ItemData)
                {
                    ItemData item = (ItemData)selected_item;
                    items_craft_time.Add(TheGame.Get().PlayTime, item);
                    inventory.AddItem(item, selected_item.craft_quantity * item_quantity);
                }

                if (selected_item is CraftGroupData)
                {
                    CSObject.Create(selected_item, GetSpawnPos(), GetSpawnRot());
                }

                if (selected_item is WorkerData)
                {
                    Worker.Create((WorkerData)selected_item, GetSpawnPos(), GetSpawnRot());
                }

                if (selected_item is NPCData)
                {
                    NPC.Create((NPCData)selected_item, GetSpawnPos(), GetSpawnRot());
                }
            }
        }

        public bool CanProduce()
        {
            Inventory global = Inventory.GetGlobal();
            bool valid = IsValid() && selected_item != null && global.HasCraftCost(selected_item, item_quantity)
                         && selected_item.HasRequirements() && !IsPopCap();
            return valid;
        }

        public bool IsPopCap()
        {
            if (selected_item is CraftGroupData || selected_item is WorkerData)
            {
                return Worker.CountPopulation() >= House.CountMaxPopulation();
            }
            return false;
        }

        public Vector3 GetSpawnPos()
        {
            float radius = Random.Range(0f, 1f);
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            return transform.position + -transform.forward * (Selectable.Interactable.use_range + 1f) + offset;
        }

        public Quaternion GetSpawnRot()
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }

        public void SetProgress(float value)
        {
            progress = value;
            uid.SetFloat("progress", value);
        }

        public float GetProgress()
        {
            return progress;
        }

        public float GetProgressMax()
        {
            return selected_item != null ? selected_item.craft_duration : 0f;
        }

        public void CheckOutOfDateProduct()
        {
            if (produce_one_by_one)
            {
                if (items_craft_time.Count == 0)
                {
                    return;
                }
                List<float> key_remove = new List<float>();
                foreach (var data in items_craft_time)
                {
                    if (TheGame.Get().PlayTime - data.Key >= TimeOutOfDate)
                    {
                        inventory.AddItem(data.Value, -item_quantity);
                        key_remove.Add(data.Key);
                    }
                }

                foreach (var key in key_remove)
                {
                    items_craft_time.Remove(key);
                }
            }
        }
        
        public bool IsAlive()
        {
            return gameObject != null && gameObject.activeSelf;
        }

        public bool IsValid()
        {
            return construction == null || construction.IsCompleted();
        }

        public bool IsValidProduce()
        {
            if (produce_one_by_one)
            {
                if (!inventory.IsEmpty())
                {
                    return false;
                }
            }
            return true;
        }

        public CraftData GetSelected()
        {
            return selected_item;
        }
        
        public float AutoWorkSpeed { get { return auto_work_speed * construction.BonusValue; } }

        public float TimeOutOfDate { get { return time_product_out_of_date * 3600f; } }
        public Selectable Selectable { get { return selectable; } }
        public Interactable Interactable { get { return interact; } }
        public Construction Construction { get { return construction; } }
        public Inventory Inventory { get { return inventory; } }
        public Workable Workable { get { return workable; } }

        public static Factory GetNearestUnassigned(Vector3 pos, float range = 999f)
        {
            float min_dist = range;
            Factory nearest = null;
            foreach (Factory factory in factory_list)
            {
                float dist = (pos - factory.transform.position).magnitude;
                if (factory.CanProduce() && !factory.Workable.IsFullyAssigned() && dist < min_dist)
                {
                    min_dist = dist;
                    nearest = factory;
                }
            }
            return nearest;
        }
    }
}
