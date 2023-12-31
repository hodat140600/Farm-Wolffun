﻿using System.Collections.Generic;
using UnityEngine;


namespace FarmWolffun
{

    /// <summary>
    /// Works are automated actions automatically assigned to workers, in priority order
    /// </summary>

    public class WorkBasic : ScriptableObject
    {
        public string id;    //Id for the save file
        public int priority; //Highest priority will be executed first
        public float range = 100f; //Maximum range it can auto target

        private static List<WorkBasic> list = new List<WorkBasic>();

        public virtual void StartWork(Worker worker)
        {
            //Inherit this function to run code when the character starts working
        }

        public virtual void StopWork(Worker worker)
        {
            //Inherit this function to run code when the character stop working
        }

        public virtual void UpdateWork(Worker worker)
        {
            //Inherit this function to run code every frame while doing this work
        }

        public virtual bool CanDoWork(Worker worker, Interactable target)
        {
            return true; //Inherit this function put a condition on if the work can be performed or not
        }

        public virtual Interactable FindBestTarget(Vector3 pos)
        {
            return null; //Inherit this function to find the best target to do this work on
        }

        public virtual bool HasTarget()
        {
            return FindBestTarget(Vector3.zero) != null;    //This checks if a valid target exists
        }

        public static void Load(string folder = "")
        {
            list.Clear();
            list.AddRange(Resources.LoadAll<WorkBasic>(folder));
            list.Sort((WorkBasic a, WorkBasic b) => { return a.priority.CompareTo(b.priority); });
        }

        public static WorkBasic Get(string id)
        {
            foreach (WorkBasic action in list)
            {
                if (action.id == id)
                    return action;
            }
            return null;
        }

        public static T Get<T>() where T : WorkBasic
        {
            foreach (WorkBasic action in list)
            {
                if (action is T)
                    return (T)action;
            }
            return null;
        }

        public static List<WorkBasic> GetAll()
        {
            return list;
        }
    }
}
