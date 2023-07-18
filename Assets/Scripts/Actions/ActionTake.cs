using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm
{
    /// <summary>
    /// Action to take items
    /// </summary>
    
    [CreateAssetMenu(fileName = "Action", menuName ="Farm/Actions/Take", order = 50)]
    public class ActionTake : ActionBasic
    {
        public ActionBasic action_return;

        public override void StartAction(Character character, Interactable target)
        {
            if (character.Inventory.IsMax())
            {
                FindReturnTarget(character);
                return;
            }

            Item item = target.GetComponent<Item>();
            if (item != null)
            {
                character.PlayAnim("take");
                character.FaceToward(target.transform.position);
                character.WaitFor(1f, () =>
                {
                    if (item != null)
                    {
                        //int hold_quantity = item.item.GetQuantity(character.Inventory.CountAvailableCargo());
                        //int quantity = Mathf.Min(item.quantity, hold_quantity);
                        int quantity = item.quantity;
                        character.Inventory.AddItem(item.data, quantity);
                        item.quantity -= quantity;

                        if (item.quantity <= 0)
                            item.Kill();

                        FindReturnTarget(character);
                    }
                });
            }
        }

        public override void StopAction(Character character, Interactable target)
        {
            character.StopAnimate();
            character.HideTools();
        }

        public override void UpdateAction(Character character, Interactable target)
        {
            
        }

        public override bool CanDoAction(Character character, Interactable target)
        {
            Item item = target != null ? target.GetComponent<Item>() : null;
            return item != null && character.Inventory != null;
        }

        private void FindReturnTarget(Character character)
        {
            character.StopAnimate();
            character.HideTools();
            character.Stop();

            Storage storage = Storage.GetNearestActive(character.transform.position, 200f);
            if (storage != null && !character.Inventory.IsEmpty())
            {
                character.OrderInterupt(action_return, storage.Interactable);
            }
            else
            {
                character.Stop();
            }
        }
    }
}
