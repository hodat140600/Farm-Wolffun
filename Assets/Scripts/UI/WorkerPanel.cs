using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FarmWolffun
{

    public class WorkerPanel : SelectPanel
    {
        [Header("Worker")]
        public Text title;
        public Text subtitle;
        public Text status_text;
        public AttributeUI[] attributes;

        private Worker _worker;

        private static WorkerPanel instance;

        protected override void Awake()
        {
            base.Awake();
            instance = this;

            foreach (ActionButton button in GetComponentsInChildren<ActionButton>())
                button.onClick += OnClickAction;
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            if (_worker != null)
            {
                status_text.text = _worker.Attributes.GetStatusText();

                foreach (AttributeUI attr in attributes)
                    attr.Hide();

                int index = 0;
                foreach (AttributeData attr in _worker.Attributes.attributes)
                {
                    if (index < attributes.Length)
                    {
                        AttributeUI attr_ui = attributes[index];
                        float value = _worker.Attributes.GetAttributeValue(attr.type);
                        float max = _worker.Attributes.GetAttributeMax(attr.type);
                        attr_ui.SetAttribute(attr.type, value, max);
                        attr_ui.SetLow(value <= attr.low_threshold);
                        index++;
                    }
                }
            }
        }

        public void OnClickAction(ActionBasic action)
        {
            if (_worker != null && !_worker.IsDead() && action.CanDoAction(_worker.Character,null))
                _worker.OrderInterupt(action, null);
        }

        public void OnClickBag()
        {
            if (BagPanel.Get().IsVisible())
                BagPanel.Get().Hide();
            else
                BagPanel.Get().ShowWorker(_worker);
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            Worker worker = select.GetComponent<Worker>();
            this._worker = worker;
            title.text = worker.GetName();
            subtitle.text = worker.data.title;
        }

        public override void Hide(bool instant = false)
        {
            base.Hide(instant);
            BagPanel.Get()?.Hide();
        }

        public override bool IsShowable(Selectable select)
        {
            Worker worker = select.GetComponent<Worker>();
            return worker != null;
        }
        
        public override int GetPriority()
        {
            return 50;
        }

        public static WorkerPanel Get()
        {
            return instance;
        }
    }
}
