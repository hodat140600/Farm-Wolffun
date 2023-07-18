using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FarmWolffun
{

    /// <summary>
    /// Worker Inventory
    /// </summary>

    public class BagPanel2 : UIPanel
    {
        public Text cargo;
        public ItemSlot[] slots;

        private Worker _worker;
        private float timer = 0f;

        private static BagPanel2 instance;

        protected override void Awake()
        {
            instance = this;
            base.Awake();

            foreach (ItemSlot slot in slots)
                slot.onClick += OnClickSlot;

            foreach (ItemSlot slot in slots)
                slot.Hide();
        }

        protected override void Update()
        {
            base.Update();

            if (!IsVisible() || _worker == null)
                return;

            timer += Time.deltaTime;
            if (timer > 1f)
            {
                RefreshPanel();
            }
        }

        private void RefreshPanel()
        {
            foreach (ItemSlot slot in slots)
                slot.Hide();
            foreach (ItemSlot slot in slots)
                slot.Hide();

            Inventory inventory = _worker.Inventory;

            int index = 0;
            foreach (ItemSet item in inventory.GetItems())
            {
                if (index < slots.Length)
                {
                    ItemSlot slot = slots[index];
                    slot.SetSlot(item.item, item.quantity);
                    index++;
                }
            }

            int cargo = inventory.GetCargo();
            int cargo_max = inventory.GetCargoMax();
            this.cargo.text = cargo + " / " + cargo_max;
        }

        public void ShowWorker(Worker worker)
        {
            this._worker = worker;
            if (worker != null && worker.Inventory != null)
            {
                Show();
                RefreshPanel();
            }
        }

        private void OnClickSlot(UISlot slot)
        {
            ItemSlot islot = (ItemSlot)slot;
            ItemData item = islot.GetItem();
            if (item != null)
            {
                _worker.Inventory.DropItem(item, _worker.transform.position);
                RefreshPanel();
            }
        }

        public Worker GetWorker()
        {
            return _worker;
        }

        public static BagPanel2 Get()
        {
            return instance;
        }
    }
}
