using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "FarmWolffun/BuildingData", order = 10)]
    public class ConstructionData : CraftData
    {
        [Header("Class Bonus")]
        public BonusType bonus;     //When equipped, will provide this bonus
        [Tooltip("Value in %")]
        public float bonus_value;
        public GroupData bonus_target; //If null, apply to all, if not apply to this target only (for gathering speed mostly)
        
        [Header("Construction")]
        public int build_limit = 0; //0 is infinite

        [Header("Upgrade")]
        public ConstructionData[] equivalents; //If you have this building, it also counts as all the buildings in this array when checking requirements (usually put lower level buildings)
        public ConstructionData[] upgrades; //List of buildings you can upgrade this to

        private static List<ConstructionData> const_list = new List<ConstructionData>();

        public int GetItemCost(ItemData item)
        {
            foreach (ItemSet aitem in craft_items)
            {
                if (aitem.item == item)
                    return aitem.quantity;
            }
            return 0;
        }

        public static new void Load(string folder = "")
        {
            const_list.Clear();
            const_list.AddRange(Resources.LoadAll<ConstructionData>(folder));
            const_list.Sort((ConstructionData a, ConstructionData b) => {
                if (a.sort_order == b.sort_order)
                    return a.title.CompareTo(b.title);
                return a.sort_order.CompareTo(b.sort_order); 
            });
        }

        public static new ConstructionData Get(string id)
        {
            foreach (ConstructionData data in const_list)
            {
                if (data.id == id)
                    return data;
            }
            return null;
        }

        public static new List<ConstructionData> GetAll()
        {
            return const_list;
        }
        
        public float BonusValue { get { return bonus_value / 100f; } }
        public BonusType BonusType { get { return bonus; } }
    }
}
