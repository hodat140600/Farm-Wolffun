using System.Collections;
using UnityEngine;

namespace FarmWolffun.EditorTool
{
    /// <summary>
    /// Default Settings file for the CreatObject editor script
    /// </summary>
    
    [CreateAssetMenu(fileName = "CreateObjectSettings", menuName = "Farm/CreateObjectSettings", order = 100)]
    public class CreateObjectSettings : ScriptableObject
    {

        [Header("Save Folders")]
        public string prefab_folder = "Farm/Prefabs";
        public string prefab_equip_folder = "Farm/Prefabs/Equip";
        public string items_folder = "Farm/Resources/Items";
        public string constructions_folder = "Farm/Resources/Constructions";

        [Header("Default Values")]
        public Material outline;
        public GameObject death_fx;
        public AudioClip craft_audio;
        public AudioClip build_audio;
        public GameObject build_fx;

    }

}