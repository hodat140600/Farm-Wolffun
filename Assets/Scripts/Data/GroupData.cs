using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Groups do not do anything on their own. But they are useful to classify objects into groups. And filter them with some functions.
    /// </summary>

    [CreateAssetMenu(fileName = "GroupData", menuName = "FarmWolffun/GroupData", order = 5)]
    public class GroupData : CSData
    {
        public string id;
        public string title;
        public Sprite icon;
    }
}
