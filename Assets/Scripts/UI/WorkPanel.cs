using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    public class WorkPanel : UIPanel
    {
        public GameObject lines_content;

        private WorkLine[] lines;

        private static WorkPanel instance;

        protected override void Awake()
        {
            base.Awake();
            instance = this;
            lines = lines_content.GetComponentsInChildren<WorkLine>();
        }

        private void RefreshPanel()
        {
            foreach (WorkLine line in lines)
                line.Hide();

            int index = 0;
            foreach (Worker worker in Worker.GetAll())
            {
                if (index < lines.Length)
                {
                    WorkLine line = lines[index];
                    line.SetLine(worker);
                    index++;
                }
            }
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            RefreshPanel();
        }

        public static WorkPanel Get()
        {
            return instance;
        }
    }
}
