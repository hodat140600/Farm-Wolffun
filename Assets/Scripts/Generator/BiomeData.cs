using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun.Generator
{

    [CreateAssetMenu(fileName = "Biome", menuName = "FarmWolffun/WorldGen/Biome", order = 100)]
    public class BiomeData : ScriptableObject
    {
        public string id;

        [Header("Spawns")]
        public BiomeSpawnData[] spawns;


    }

}