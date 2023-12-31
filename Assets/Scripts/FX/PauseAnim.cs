﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{

    public class PauseAnim : MonoBehaviour
    {
        private Animator animator;
        private bool was_paused;
        private float prev_speed = 1f;
        private float speed = 1f;

        void Start()
        {
            animator = GetComponent<Animator>();
            if (animator != null)
                speed = animator.speed;
        }

        void Update()
        {
            bool is_paused = TheGame.Get().IsPaused();
            float mult = TheGame.Get().GetSpeedMultiplier();

            if (animator != null)
            {
                if(is_paused != was_paused || !prev_speed.Equals(mult)) 
                animator.speed = is_paused ? 0f : speed * mult;
            }

            was_paused = is_paused;
            prev_speed = mult;
        }
    }
}
