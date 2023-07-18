using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace FarmWolffun
{

    public class IdleButton : MonoBehaviour
    {
        public Text idle_count;

        private CanvasGroup canvas_group;
        private float timer = 0f;

        void Awake()
        {
            canvas_group = GetComponent<CanvasGroup>();
            SetVisible(false);
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > 0.4f)
            {
                timer = 0f;
                SlowUpdate();
            }

        }

        void SlowUpdate()
        {
            int idle = WorkerManager.Get().CountIdles();
            idle_count.text = idle.ToString();
            SetVisible(idle > 0);
        }

        public void SetVisible(bool visible)
        {
            canvas_group.alpha = visible ? 1f : 0f;
            canvas_group.blocksRaycasts = visible;
        }

        public void OnClick()
        {
            Worker worker = WorkerManager.Get().GetNextIdle();
            if (worker != null)
            {
                Selectable.UnselectAll();
                worker.Selectable.Select();
                TheCamera.Get().MoveToTarget(worker.transform.position);
            }
        }
    }
}
