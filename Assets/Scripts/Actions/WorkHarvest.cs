using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(fileName = "Work", menuName = "Farm/Work/WorkHarvest", order = 40)]
    public class WorkHarvest : WorkBasic
    {
        public ActionGather action_gather;
        public ActionTake action_take;

        public override void StartWork(Worker worker)
        {
            Interactable target = worker.GetWorkTarget();
            Zone zone = target.GetComponent<Zone>();
            Gatherable gather = target.GetComponent<Gatherable>();
            Item item = zone?.GetNearestItem(worker.transform.position);
            Destructible attack_targ = zone?.GetNearestDestructible(worker.transform.position);
            if (zone != null)
                gather = zone.GetNearestGatherable(worker.transform.position);

            if (item != null)
                worker.AutoOrder(action_take, item.Interactable);
            else if (gather != null)
                worker.AutoOrder(action_gather, gather.Interactable);
        }

        public override void StopWork(Worker worker)
        {

        }

        public override bool CanDoWork(Worker worker, Interactable target)
        {
            if (target != null)
            {
                Zone zone = target.GetComponent<Zone>();
                Gatherable gather = target.GetComponent<Gatherable>();
                bool azone = zone != null && zone.CanBeGathered();
                bool agather = gather != null && gather.CanHarvest();
                return azone || agather;
            }
            return false;
        }

        public override Interactable FindBestTarget(Vector3 pos)
        {
            Zone zone = Zone.GetNearestUnassigned(pos, range);
            Gatherable gather = Gatherable.GetNearestUnassigned(pos, range);
            if (zone != null)
                return zone.Interactable;
            else if (gather != null)
                return gather.Interactable;
            return null;
        }
    }
}
