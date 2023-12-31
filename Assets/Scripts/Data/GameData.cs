﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Generic game data (only one file)
    /// </summary>

    [CreateAssetMenu(fileName = "GameData", menuName = "FarmWolffun/GameData", order = 0)]
    public class GameData : ScriptableObject
    {
        [Header("Gameplay")]
        [Tooltip("Time goes X times faster than in real life")]
        public float game_speed = 144f;     //Time goes X times faster than in real life

        [Header("Music")]
        public AudioClip[] music_playlist;  //Music played at the start of each scene

        public static GameData Get()
        {
            return TheData.Get().data;
        }
    }

}
