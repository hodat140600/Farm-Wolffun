using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(fileName = "Work", menuName = "Farm/Work/WorkFactory", order = 40)]
    public class WorkFactory : WorkBasic
    {
        public override void StartWork(Worker worker)
        {
            Interactable target = worker.GetWorkTarget();
            ActionBasic action = worker.Character.GetPriorityAction(target);
            worker.AutoOrder(action, target);
        }

        public override void StopWork(Worker worker)
        {

        }

        public override bool CanDoWork(Worker worker, Interactable target)
        {
            if (target != null)
            {
                Factory factory = target.GetComponent<Factory>();
                if (factory != null)
                {
                    CraftData data = factory.GetSelected();
                    return data != null && data.HasRequirements();
                }
            }
            return false;
        }

        public override Interactable FindBestTarget(Vector3 pos)
        {
            Factory factory = Factory.GetNearestUnassigned(pos, range);
            return factory?.Interactable;
        }
    }
}
