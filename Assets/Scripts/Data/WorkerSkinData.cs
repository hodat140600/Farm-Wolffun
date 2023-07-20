using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    [CreateAssetMenu(fileName = "WorkerSkinData", menuName = "FarmWolffun/WorkerSkinData", order = 10)]
    public class WorkerSkinData : ScriptableObject
    {
        public string id;

        [Header("Prefab")]
        public GameObject skin_prefab;

        [Header("Names")]
        public string[] names; //Possible names, overrite the default name

        private static List<WorkerSkinData> skin_list = new List<WorkerSkinData>();

        public string GetRandomName()
        {
            if (names.Length > 0)
                return names[Random.Range(0, names.Length)];
            return "";
        }

        //Get a name that has not been used before
        public string GetRandomUniqueName()
        {
            HashSet<string> existing_names = new HashSet<string>();
            foreach (Worker worker in Worker.GetAll())
                existing_names.Add(worker.SData.name);

            List<string> valid_names = new List<string>();
            foreach (string aname in names)
            {
                if (!existing_names.Contains(aname))
                    valid_names.Add(aname);
            }

            if (valid_names.Count > 0)
                return valid_names[Random.Range(0, valid_names.Count)];
            return GetRandomName();
        }

        public static void Load(string folder = "")
        {
            skin_list.Clear();
            skin_list.AddRange(Resources.LoadAll<WorkerSkinData>(folder));
        }

        public static WorkerSkinData Get(string id)
        {
            foreach (WorkerSkinData data in skin_list)
            {
                if (data.id == id)
                    return data;
            }
            return null;
        }

        public static List<WorkerSkinData> GetAll()
        {
            return skin_list;
        }
    }
}
