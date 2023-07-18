using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    [CreateAssetMenu(fileName = "Work", menuName = "Farm/Work/WorkBuild", order = 40)]
    public class WorkBuild : WorkBasic
    {
        public ActionBuild action_build;

        public override void StartWork(Worker worker)
        {
            Interactable target = worker.GetWorkTarget();
            Construction construct = target.GetComponent<Construction>();
            if (construct != null)
            {
                worker.AutoOrder(action_build, target);
            }
        }

        public override void StopWork(Worker worker)
        {
            
        }

        public override bool CanDoWork(Worker worker, Interactable target)
        {
            if (target != null)
            {
                Construction construct = target.GetComponent<Construction>();
                return construct != null && construct.IsConstructing() && action_build.CanDoAction(worker.Character, target);
            }
            return false;
        }

        public override Interactable FindBestTarget(Vector3 pos)
        {
            Construction construct = Construction.GetNearestUnassigned(pos, range);
            return construct?.Interactable;
        }
    }
}