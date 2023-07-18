using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FarmWolffun
{

    public class AttributeUI : MonoBehaviour
    {
        public Text title;
        public Text value_text;
        public ProgressBar progress;

        public Sprite high_sprite;
        public Sprite low_sprite;

        private AttributeType type;
        private bool active = false;

        private void Update()
        {
            if (!active)
                gameObject.SetActive(false);
        }

        public void SetAttribute(AttributeType type, float value, float value_max)
        {
            AttributeData attribute = AttributeData.Get(type);
            if (attribute != null)
            {
                this.type = type;
                title.text = attribute.title;
                value_text.text = Mathf.RoundToInt(value) + " / " + value_max;
                progress.value = value;
                progress.value_max = value_max;
                active = true;
                gameObject.SetActive(true);
            }
        }

        public void SetLow(bool low)
        {
            progress.fill.sprite = low ? low_sprite : high_sprite;
        }

        public void Hide()
        {
            type = AttributeType.None;
            active = false;
        }
    }
}
