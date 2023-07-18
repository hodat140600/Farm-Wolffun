using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{

    public enum WorkerAreaAttributeType
    {
        GlobalAttribute = 0,    //Will show independant attribute update by script in WorkerAreaManager.cs
        WorkerAverage = 10,   //Will show the average of the attribute of all workers
    }

    [RequireComponent(typeof(AttributeUI))]
    public class WorkerAreaAttributeUI : MonoBehaviour
    {
        public WorkerAreaAttributeType type;
        public AttributeType attribute;

        private AttributeUI attribute_ui;
        private float timer = 99f;
        private float refresh_rate = 0.5f;

        void Awake()
        {
            attribute_ui = GetComponent<AttributeUI>();
        }

        private void Start()
        {
            UpdateUI();
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > refresh_rate)
            {
                timer = 0f;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            if (type == WorkerAreaAttributeType.WorkerAverage)
                UpdateAverage();
            else if (type == WorkerAreaAttributeType.GlobalAttribute)
                UpdateGlobal();
        }

        private void UpdateGlobal()
        {
            WorkerAreaManager worker_area = WorkerAreaManager.Get();
            float val = worker_area.GetAttributeValue(attribute);
            float mval = worker_area.GetAttributeMax(attribute);
            attribute_ui.SetAttribute(attribute, val, mval);
            attribute_ui.SetLow(worker_area.IsLow(attribute));
        }

        private void UpdateAverage()
        {
            AttributeData idata = AttributeData.Get(attribute);
            if (idata != null)
            {
                float average = WorkerManager.Get().GetAverageAttribute(attribute);
                attribute_ui.SetAttribute(attribute, average, idata.max_value);
                attribute_ui.SetLow(average <= idata.low_threshold);
            }
        }
    }
}