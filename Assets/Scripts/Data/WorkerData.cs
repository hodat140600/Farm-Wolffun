using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    [CreateAssetMenu(fileName = "WorkerData", menuName = "FarmWolffun/WorkerData", order = 10)]
    public class WorkerData : CraftData
    {
        [Header("Class Bonus")]
        public BonusType bonus;     //When equipped, will provide this bonus
        public float bonus_value;
        public GroupData bonus_target; //If null, apply to all, if not apply to this target only (for gathering speed mostly)

        [Header("Skins")]
        public WorkerSkinData[] skins; //Possible skins when spawning

        private static List<WorkerData> worker_list = new List<WorkerData>();

        public WorkerSkinData GetRandomSkin()
        {
            if (skins.Length > 0)
                return skins[Random.Range(0, skins.Length)];
            return null;
        }

        public string GetSkinID(WorkerSkinData skin)
        {
            return skin != null ? skin.id : "";
        }

        public GameObject GetPrefab(WorkerSkinData skin)
        {
            return skin != null ? skin.skin_prefab : prefab;
        }

        public string GetRandomName(WorkerSkinData skin)
        {
            if (skin != null)
                return skin.GetRandomName();
            return title;
        }

        //Get a name that has not been used before
        public string GetRandomUniqueName(WorkerSkinData skin)
        {
            if (skin != null)
                return skin.GetRandomUniqueName();
            return title;
        }

        public static new void Load(string folder = "")
        {
            worker_list.Clear();
            worker_list.AddRange(Resources.LoadAll<WorkerData>(folder));
        }

        public static new WorkerData Get(string id)
        {
            foreach (WorkerData data in worker_list)
            {
                if (data.id == id)
                    return data;
            }
            return null;
        }

        public static new List<WorkerData> GetAll()
        {
            return worker_list;
        }
    }
}
