using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Action to havest resources
    /// </summary>
    
    [CreateAssetMenu(fileName = "Action", menuName ="Farm/Actions/Use", order = 50)]
    public class ActionUse : ActionBasic
    {
        public GroupData item_group;         //Item will be limited to this group, all items if null
        public AttributeType item_attribute; //Items will be prioritized with the one with highest attribute, if none, will be prioritized by priority
        public string anim;                  //Animation played

        public override void StartAction(Character character, Interactable target)
        {
            character.PlayAnim(anim);
            character.WaitFor(0.5f, () =>
            {
                Worker worker = character.worker;
                if (worker != null)
                {
                    Inventory global = Inventory.GetGlobal();
                    worker.UseItem(global, item_group, item_attribute);
                    worker.Character.Stop();
                }
            });
        }

        public override void StopAction(Character character, Interactable target)
        {

        }

        public override void UpdateAction(Character character, Interactable target)
        {
            
        }

        public override bool CanDoAction(Character character, Interactable target)
        {
            Worker worker = character.worker;
            Inventory global = Inventory.GetGlobal();
            return worker != null && global.HasItemGroup(item_group);
        }
    }
}
