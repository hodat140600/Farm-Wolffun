using System.Collections;
using System.Collections.Generic;
using FarmWolffun;
using FarmWolffun.Events;
using UnityEngine;

namespace FarmWolffun
{
    [CreateAssetMenu(fileName = "PauseEffect", menuName = "FarmWolffun/EventEffect/PauseEffect", order = 10)]
    public class VictoryEffect : CustomEffect
    {
        public override void DoEffect()
        {
            base.DoEffect();
            TheUI.Get().OnVictory();
        }
    }
}