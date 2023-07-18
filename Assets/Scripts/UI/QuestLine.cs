using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FarmWolffun
{

    public class QuestLine : MonoBehaviour
    {
        public Text title;

        public void SetQuest(QuestData quest)
        {
            title.text = quest.title;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
