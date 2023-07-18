using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Farm
{

    public class WorkLine : MonoBehaviour
    {
        public Text title;
        public Text status;
        public Toggle harvest_toggle;
        public Toggle build_toggle;
        public Toggle produce_toggle;

        private Worker _worker;

        void Start()
        {
            harvest_toggle.onValueChanged.AddListener(OnChangeToggle);
            build_toggle.onValueChanged.AddListener(OnChangeToggle);
            produce_toggle.onValueChanged.AddListener(OnChangeToggle);

            foreach (ActionButton button in GetComponentsInChildren<ActionButton>())
                button.onClick += OnClickAction;
        }

        public void SetLine(Worker worker)
        {
            this._worker = worker;

            if (worker != null)
            {
                title.text = worker.GetName();
                status.text = worker.Attributes.GetStatusText();

                WorkHarvest harvest = WorkBasic.Get<WorkHarvest>();
                WorkBuild build = WorkBasic.Get<WorkBuild>();
                WorkFactory produce = WorkBasic.Get<WorkFactory>();

                harvest_toggle.isOn = worker.SData.IsActionEnabled(harvest.id);
                build_toggle.isOn = worker.SData.IsActionEnabled(build.id);
                produce_toggle.isOn = worker.SData.IsActionEnabled(produce.id);

                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnChangeToggle(bool val)
        {
            if (_worker != null)
            {
                bool bharvest = harvest_toggle.isOn;
                bool bbuild = build_toggle.isOn;
                bool bproduce = produce_toggle.isOn;

                WorkHarvest harvest = WorkBasic.Get<WorkHarvest>();
                WorkBuild build = WorkBasic.Get<WorkBuild>();
                WorkFactory produce = WorkBasic.Get<WorkFactory>();

                _worker.SData.SetActionEnabled(harvest.id, bharvest);
                _worker.SData.SetActionEnabled(build.id, bbuild);
                _worker.SData.SetActionEnabled(produce.id, bproduce);
            }
        }

        private void OnClickAction(ActionBasic action)
        {
            if (_worker != null && !_worker.IsDead() && action.CanDoAction(_worker.Character, null))
                _worker.OrderInterupt(action, null);
        }

        public void OnClickWorker()
        {
            Selectable.UnselectAll();
            _worker.Selectable.Select();
            TheCamera.Get().MoveToTarget(_worker.transform.position);
            WorkPanel.Get().Hide();
        }
    }
}