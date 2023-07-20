using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    [CreateAssetMenu(fileName = "Action", menuName = "FarmWolffun/Actions/Trade", order = 50)]
    public class ActionShop : ActionBasic
    {
        public override void StartAction(Character character, Interactable select)
        {
            Trader shop = select.GetComponent<Trader>();
            if (shop != null)
                shop.OpenTrade(character);
        }

        public override bool CanDoAction(Character character, Interactable select)
        {
            Trader shop = select.GetComponent<Trader>();
            return shop != null;
        }
    }
}
