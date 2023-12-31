﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FarmWolffun
{

    public class EconomyLine : MonoBehaviour
    {
        public Image icon;
        public Text title;

        public Text amount;
        public Text income;
        public Text workers;

        private ItemData item;

        void Start()
        {

        }

        void Update()
        {

        }

        public void SetLine(ItemData item)
        {
            this.item = item;

            if (item != null)
            {
                icon.sprite = item.icon;
                title.text = item.title;

                amount.text = "";
                income.text = "";
                workers.text = "";

                Inventory global = Inventory.GetGlobal();
                ItemSet set = global.GetItem(item);
                if (set != null)
                {
                    amount.text = set.quantity.ToString();
                }

                int count = WorkerManager.Get().CountGathering(item) + WorkerManager.Get().CountProducing(item);
                workers.text = count + "/" + Worker.GetAll().Count;

                gameObject.SetActive(true);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
