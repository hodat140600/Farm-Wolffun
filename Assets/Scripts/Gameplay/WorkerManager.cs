using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Script that managages all Workers and assign them to tasks
    /// </summary>

    public class WorkerManager : MonoBehaviour
    {
        private HashSet<Worker> registered_workers = new HashSet<Worker>();
        private HashSet<Worker> working_workers = new HashSet<Worker>();
        private Dictionary<Interactable, HashSet<Worker>> assigned_workers = new Dictionary<Interactable, HashSet<Worker>>();

        private int idle_index = 0;
        private float update_timer = 0f;

        private static WorkerManager instance;

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            if (TheGame.Get().IsPaused())
                return;

            update_timer += Time.deltaTime;
            if (update_timer > 0.5f)
            {
                update_timer = 0f;
                SlowUpdate();
            }
        }

        private void SlowUpdate()
        {
            //Start new work
            Vector3 base_pos = GetBaseCenter();
            foreach (WorkBasic work in WorkBasic.GetAll())
            {
                //Check if work has target
                Interactable target = work.FindBestTarget(base_pos);
                if (target != null)
                {
                    if (target.Workable == null || !target.Workable.IsFullyAssigned())
                    {
                        //Find the best worker for the work
                        Worker worker = FindBestWorker(work, target);
                        if (worker != null)
                        {
                            StartWork(worker, work, target);
                        }
                    }
                }
            }

            //Stop work
            foreach (Worker worker in Worker.GetAll())
            {
                WorkBasic current_work = worker.GetWork();
                Interactable work_target = worker.GetWorkTarget();
                if (current_work != null)
                {
                    //Stop work
                    if (worker.IsIdle() || !worker.CanDoWork(current_work, work_target) || worker.Attributes.IsAnyDepleted())
                        worker.StopWork();
                    if(work_target != null && work_target.Workable != null && work_target.Workable.IsOverAssigned())
                        worker.StopWork();
                }
            }
        }

        public void RegisterWorker(Worker worker)
        {
            //Add worker to list and register events
            if (!registered_workers.Contains(worker))
            {
                registered_workers.Add(worker);
                worker.onStartWork += OnStartWork;
                worker.onStopWork += OnStopWork;
            }
        }

        public void UnregisterWorker(Worker worker)
        {
            //Remove worker events
            if (registered_workers.Contains(worker))
            {
                registered_workers.Remove(worker);
                worker.onStartWork -= OnStartWork;
                worker.onStopWork -= OnStopWork;
            }
        }

        public void StartWork(Worker worker, WorkBasic work, Interactable target)
        {
            //Start working on a task
            if (worker != null && work != null && target != null)
            {
                worker.StartWork(work, target);
            }
        }

        private void OnStartWork(Worker worker, WorkBasic work)
        {
            AssignWorker(worker, worker.GetWorkTarget());
        }

        private void OnStopWork(Worker worker)
        {
            UnassignWorker(worker);
        }

        private void AssignWorker(Worker worker, Interactable select)
        {
            //Assign worker to selectable
            UnassignWorker(worker);
            working_workers.Add(worker);
            if (select != null)
            {
                if (!assigned_workers.ContainsKey(select))
                    assigned_workers[select] = new HashSet<Worker>();
                assigned_workers[select].Add(worker);
            }
        }

        private void UnassignWorker(Worker worker)
        {
            //Unlink worker from all selectables
            if (working_workers.Contains(worker))
            {
                working_workers.Remove(worker);
                foreach (KeyValuePair<Interactable, HashSet<Worker>> pair in assigned_workers)
                {
                    if (pair.Value.Contains(worker))
                        pair.Value.Remove(worker);
                }
            }
        }

        public Worker FindBestWorker(WorkBasic work, Interactable target)
        {
            Worker best = null;
            float min_dist = work.range;
            foreach (Worker worker in Worker.GetAll())
            {
                if (worker.IsAuto() || worker.IsIdle())
                {
                    if (!worker.IsWorking() || work.priority > worker.GetWork().priority)
                    {
                        float dist = (worker.transform.position - target.Transform.position).magnitude;
                        if (dist < work.range && worker.CanDoWork(work, target))
                        {
                            bool best_idle = best != null && best.IsIdle();
                            bool is_idle_better = worker.IsIdle() || !best_idle; //Idle workers priority over working ones
                            bool is_better = is_idle_better && dist < min_dist;

                            if (is_better)
                            {
                                min_dist = dist;
                                best = worker;
                            }
                        }
                    }
                }
            }
            return best;
        }

        public Worker GetNextIdle()
        {
            idle_index++;
            if (idle_index >= CountIdles())
                idle_index = 0;
            return GetIdle(idle_index);
        }

        public Worker GetIdle(int index)
        {
            int aindex = 0;
            foreach (Worker worker in Worker.GetAll())
            {
                if (worker.IsActive() && worker.IsIdle())
                {
                    if (index == aindex)
                        return worker;
                    aindex++;
                }
            }
            return null;
        }

        public int CountIdles()
        {
            int count = 0;
            foreach (Worker worker in Worker.GetAll())
            {
                if (worker.IsActive() && worker.IsIdle())
                    count++;
            }
            return count;
        }

        public List<Worker> GetWorkingOn(Interactable target)
        {
            if (assigned_workers.ContainsKey(target))
            {
                List<Worker> workers = new List<Worker>(assigned_workers[target]);
                return workers;
            }
            return new List<Worker>();
        }

        //Only count those who are assigned automatically with a WorkBasic action
        public int CountWorkingOn(Interactable target)
        {
            if (assigned_workers.ContainsKey(target))
            {
                HashSet<Worker> workers = assigned_workers[target];
                return workers.Count;
            }
            return 0;
        }
        
        //Count how many are currently gathering a resource
        public int CountGathering(ItemData item)
        {
            int count = 0;
            foreach (Worker worker in Worker.GetAll())
            {
                Interactable target = worker.GetCurrentActionTarget();
                Gatherable gather = target?.Gatherable;
                if (gather != null && gather.GetItem() == item)
                {
                    count++;
                }
            }
            return count;
        }

        public int CountProducing(ItemData item)
        {
            int count = 0;
            foreach (Worker worker in Worker.GetAll())
            {
                Interactable target = worker.GetCurrentActionTarget();
                Factory factory = target?.Factory;
                if (factory != null && factory.GetSelected() == item)
                {
                    count++;
                }
            }
            return count;
        }

        public float GetAverageAttribute(AttributeType type)
        {
            int count = 0;
            float total = 0f;
            foreach (Worker worker in Worker.GetAll())
            {
                if (!worker.IsDead() && worker.Attributes != null)
                {
                    total += worker.Attributes.GetAttributeValue(type);
                    count += 1;
                }
            }
            if(count > 0)
                return total / (float)count;
            return 0f;
        }

        public bool IsAssigned(Worker worker)
        {
            return working_workers.Contains(worker);
        }

        public Vector3 GetBaseCenter()
        {
            Headquarter hq = Headquarter.GetMain();
            if(hq != null)
                return hq.transform.position;
            return Vector3.zero;
        }

        public static WorkerManager Get()
        {
            if (instance == null)
                return FindObjectOfType<WorkerManager>();
            return instance;
        }
    }
}
