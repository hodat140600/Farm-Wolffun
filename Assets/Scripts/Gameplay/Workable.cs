﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Location where workers can be assigned to work to
    /// </summary>

    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(UniqueID))]
    public class Workable : MonoBehaviour
    {
        public int workers_max = 5;        //Maximum number of workers that can work there.

        private Selectable select;
        private Interactable interact;
        private Construction contruct;
        private UniqueID uid;
        private int workers = 0;

        void Awake()
        {
            select = GetComponent<Selectable>();
            interact = GetComponent<Interactable>();
            contruct = GetComponent<Construction>();
            uid = GetComponent<UniqueID>();
        }

        private void Start()
        {
            if (uid.HasInt("workers"))
                workers = uid.GetInt("workers");
        }

        public void SetWorkerAmount(int value)
        {
            workers = Mathf.Clamp(value, 0, workers_max);
            uid.SetInt("workers", workers);
        }

        public int GetWorkerAmount()
        {
            return workers;
        }

        public List<Worker> GetAssignedWorkers()
        {
            return WorkerManager.Get().GetWorkingOn(interact);
        }

        public int CountAssignedWorkers()
        {
            return Worker.CountWorkingOn(interact);
        }

        public bool IsFullyAssigned()
        {
            return IsValid() && Worker.CountWorkingOn(interact) >= GetWorkerAmount();
        }

        public bool IsOverAssigned()
        {
            return IsValid() && Worker.CountWorkingOn(interact) > GetWorkerAmount();
        }

        public bool IsValid()
        {
            return contruct == null || contruct.IsCompleted();
        }

        public Selectable Selectable { get { return select; } }
        public Interactable Interactable { get { return interact; } }
    }
}
