using System.Collections.Generic;
using UnityEngine;

namespace FarmWolffun
{
    /// <summary>
    /// Script for any building that can be added by the player and then built with ressources and workers 
    /// </summary>

    [RequireComponent(typeof(Selectable))]
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(Buildable))]
    [RequireComponent(typeof(UniqueID))]
    public class Construction : CSObject
    {
        public ConstructionData data;

        [Header("Mesh")]
        public GameObject built_mesh;           //Mesh displayed when the construction is completed
        public GameObject[] building_mesh;      //Mesh displayed when the construction is in progress. If more than one will show then sequentially in order.

        [Header("FX")]
        public AudioClip built_audio;           //Audio and FX played when the construction is completed building
        public GameObject built_fx;

        private Selectable select;
        private Interactable interact;
        private Buildable buildable;
        private Workable workable; //may be null
        private Destructible destruct; //may be null
        private ConstructionAttribute attributes;
        private UniqueID uid;

        private static List<Construction> building_list = new List<Construction>();
        private Dictionary<BonusType, BonusEffectData> bonus_effects = new Dictionary<BonusType, BonusEffectData>();

        protected override void Awake()
        {
            base.Awake();
            building_list.Add(this);
            select = GetComponent<Selectable>();
            interact = GetComponent<Interactable>();
            buildable = GetComponent<Buildable>();
            workable = GetComponent<Workable>();
            destruct = GetComponent<Destructible>();
            uid = GetComponent<UniqueID>();
            if (destruct)
                destruct.onDeath += OnDeath;
            buildable.onBuild += OnPlaced;
            select.onSelect += OnSelect;
            select.onUnselect += OnUnselect;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            building_list.Remove(this);
        }

        //Start Placing the building on the map
        public void StartBuild()
        {
            SData.built = false;
            SData.paid = false;
            HideAllMesh();
            built_mesh.SetActive(true);
            buildable.StartBuild();
        }

        //Start the building progress bar
        public void StartConstruct()
        {
            SaveConstructionData chdata = SData;
            chdata.id = data.id;
            chdata.scene = SceneNav.GetCurrentScene();
            chdata.pos = transform.position;
            chdata.rot = transform.rotation;
            chdata.spawned = true;
            chdata.paid = false;
            chdata.built = false;
            chdata.build_progress = 0f;
            ShowBuildingMesh(0);
        }

        public void PayBuildCost()
        {
            Inventory global = Inventory.GetGlobal();
            if (!HasPaidCost() && global.HasCraftCost(data))
            {
                global.PayCraftCost(data);
                SData.paid = true;
            }
        }

        private void OnPlaced()
        {
            StartConstruct();

            if (TheControls.Get().IsShiftHold())
                CreateCopy();
        }

        private void OnSelect()
        {
            if (IsCompleted() && Selectable.GetSelectionCount() == 1)
            {
                if (data.upgrades.Length > 0)
                    UpgradePanel.Get().ShowBuilding(this);
            }
        }

        private void OnUnselect()
        {
            if(UpgradePanel.Get().IsVisible())
                UpgradePanel.Get().Hide();
            if (UpgradeInfoPanel.Get().IsVisible())
                UpgradeInfoPanel.Get().Hide();
        }

        private void CreateCopy()
        {
            Construction construct = CreateBuildMode(data);
            construct.transform.position = transform.position;
            construct.transform.rotation = transform.rotation;
        }

        public void AddProgress(float value)
        {
            if (!IsCompleted() && HasPaidCost())
            {
                SData.build_progress += value;

                UpdateBuildingMesh();

                if (SData.build_progress > data.craft_duration)
                    FinishProgress();
            }
        }

        public void FinishProgress()
        {
            if (!IsCompleted())
            {
                SData.built = true;
                SData.paid = true;
                HideAllMesh();
                built_mesh.SetActive(true);

                if (workable != null)
                    workable.SetWorkerAmount(1);

                if (built_fx != null)
                        Instantiate(built_fx, transform.position, Quaternion.identity);
                TheAudio.Get().PlaySFX3D("build", built_audio, transform.position);
            }
        }

        public void Upgrade(ConstructionData upgrade)
        {
            Inventory global = Inventory.GetGlobal();
            if (global.HasCraftCost(upgrade))
            {
                ConstructionData prev_data = data;
                SaveData.Get().RemoveBuilding(uid.uid);
                select.Remove();

                //Create Upgrade
                Construction build = Create(upgrade, transform.position, transform.rotation);
                build.StartConstruct();
                build.SData.upgraded_from = prev_data.id;
            }
        }

        //Cancel build while progress not completed (and return resources)
        public void CancelConstruct()
        {
            if (!IsBuildMode() && !IsCompleted())
            {
                ConstructionData original = null;
                if(!string.IsNullOrEmpty(SData.upgraded_from))
                    original = ConstructionData.Get(SData.upgraded_from);
                
                if (HasPaidCost())
                {
                    Inventory global = Inventory.GetGlobal();
                    global.RefundCraftCost(data);
                }

                Kill();

                if (original != null)
                    Create(original, transform.position, transform.rotation);
            }
        }

        public override void Kill()
        {
            OnDeath();
            select.Kill();
        }

        private void OnDeath()
        {
            SaveData.Get().RemoveBuilding(uid.uid);
        }

        private void ShowBuildingMesh(int index)
        {
            HideAllMesh();
            if (index < building_mesh.Length && index >= 0)
                building_mesh[index].SetActive(true);
            else
                built_mesh.SetActive(true);
        }

        private void UpdateBuildingMesh()
        {
            if (!IsCompleted() && building_mesh.Length > 0 && data.craft_duration > 0.001f)
            {
                int stage = Mathf.FloorToInt(SData.build_progress * building_mesh.Length / data.craft_duration);
                stage = Mathf.Clamp(stage, 0, building_mesh.Length - 1);
                ShowBuildingMesh(stage);
            }
        }

        private void HideAllMesh()
        {
            built_mesh.SetActive(false);
            foreach (GameObject mesh in building_mesh)
                mesh.SetActive(false);
        }

        public bool CanPayBuildCost()
        {
            Inventory global = Inventory.GetGlobal();
            return global.HasCraftCost(data);
        }

        public float GetProgress()
        {
            return SData.build_progress;
        }

        public float GetProgressDuration()
        {
            return data.craft_duration;
        }

        public bool IsUnassigned()
        {
            return Worker.CountWorkingOn(interact) == 0;
        }
        
        //Does not return all bonus (like tech, equipment), only returns class bonus
        public float GetClassBonus(BonusType type, Selectable target, CraftData itarget)
        {
            float bonus = 0f;
            bool is_any = target == null && itarget == null;
            bool is_valid_select = target != null && target.HasGroup(data.bonus_target);
            bool is_valid_item = itarget != null && itarget.HasGroup(data.bonus_target);

            //Only one of the target need to be valid, or have no target
            if (data.BonusType == type && (is_any || is_valid_select || is_valid_item))
                bonus = data.BonusValue;
            return bonus;
        }

        //Does not return all bonus (like class, equipment), only returns tech bonus
        public float GetTechBonus(BonusType type, Selectable target, CraftData itarget)
        {
            return TechManager.Get().GetTechBonus(type, Selectable, target, itarget);
        }
        
        public void SetTempBonusEffect(BonusType type, float value, float duration)
        {
            if (bonus_effects.ContainsKey(type))
            {
                if(bonus_effects[type].value < value || bonus_effects[type].duration < duration)
                    bonus_effects[type] = new BonusEffectData(type, value, duration);
            }
            else
            {
                bonus_effects[type] = new BonusEffectData(type, value, duration);
            }
        }
        public float GetTempBonusEffectValue(BonusType type)
        {
            if (bonus_effects.ContainsKey(type))
                return bonus_effects[type].value;
            return 0f;
        }
        
        //Bonus Raw values
        public float GetBonusValue(BonusType type, Selectable target = null, CraftData itarget = null)
        {
            float bbonus = GetTempBonusEffectValue(type);                                        //Temporary Bonus (buff)
            float tbonus = GetTechBonus(type, target, itarget);        //Tech Bonus
            float cbonus = GetClassBonus(type, target, itarget); //Class bonus
            return bbonus + tbonus + cbonus;
        }

        //Bonus multiplier
        public float GetBonusMult(BonusType type, Selectable target = null, CraftData itarget = null)
        {
            return 1f + GetBonusValue(type, target, itarget);
        }
        public float BonusValue { get; set; }

        public bool HasPaidCost() { return SData.paid || SData.built; }
        public bool IsCompleted() { return SData.built; }
        public bool IsConstructing() { return !buildable.IsBuilding() && !IsCompleted(); }
        public bool IsBuildMode() { return buildable.IsBuilding(); }

        public Selectable Selectable { get { return select; } }
        public Interactable Interactable { get { return interact; } }
        public Destructible Destructible { get { return destruct; } }
        public ConstructionAttribute Attributes {  get { return attributes; }}
        public SaveConstructionData SData { get { return SaveData.Get().GetBuilding(uid.uid); } } //SData is the saved data linked to this object

        public static int CountConstructions(ConstructionData data)
        {
            int count = 0;
            foreach (Construction construct in building_list)
            {
                if (construct.IsCompleted() && construct.data != null)
                {
                    bool equivalent = false;
                    foreach (ConstructionData equiv in construct.data.equivalents)
                    {
                        if (equiv == data)
                            equivalent = true;
                    }

                    if (construct.data == data || equivalent)
                        count++;
                }
            }
            return count;
        }

        public static Construction GetNearestUnassigned(Vector3 pos, float range = 999f)
        {
            float min_dist = range;
            Construction nearest = null;
            foreach (Construction construct in building_list)
            {
                float dist = (pos - construct.transform.position).magnitude;
                if (construct.IsConstructing() && construct.IsUnassigned() && dist < min_dist)
                {
                    min_dist = dist;
                    nearest = construct;
                }
            }
            return nearest;
        }

        public static new List<Construction> GetAll()
        {
            return building_list;
        }

        public static Construction Spawn(string uid, SaveConstructionData data)
        {
            ConstructionData bdata = ConstructionData.Get(data.id);
            if (bdata != null && data.scene == SceneNav.GetCurrentScene())
            {
                GameObject obj = Instantiate(bdata.prefab, data.pos, data.rot);
                UniqueID uniqueid = obj.GetComponent<UniqueID>();
                uniqueid.uid = uid;
                Construction building = obj.GetComponent<Construction>();
                building.built_mesh?.SetActive(data.built);
                building.UpdateBuildingMesh();
                return building;
            }
            return null;
        }

        public static Construction Create(ConstructionData data, Vector3 pos)
        {
            return Create(data, pos, data.prefab.transform.rotation);
        }

        public static Construction Create(ConstructionData data, Vector3 pos, Quaternion rot)
        {
            GameObject obj = Instantiate(data.prefab, pos, rot);
            Construction building = obj.GetComponent<Construction>();
            building.data = data;
            UniqueID uniqueid = obj.GetComponent<UniqueID>();
            uniqueid.uid = UniqueID.GenerateUniqueID();
            SaveData sdata = SaveData.Get();
            SaveConstructionData chdata = sdata.GetBuilding(uniqueid.uid);
            chdata.id = data.id;
            chdata.scene = SceneNav.GetCurrentScene();
            chdata.pos = pos;
            chdata.rot = rot;
            chdata.spawned = true;
            chdata.built = true;
            chdata.paid = true;
            return building;
        }

        public static Construction CreateBuildMode(ConstructionData data)
        {
            Vector3 pos = TheControls.Get().GetMouseWorldPos();
            GameObject obj = Instantiate(data.prefab, pos, Quaternion.identity);
            Construction building = obj.GetComponent<Construction>();
            building.data = data;
            UniqueID uniqueid = obj.GetComponent<UniqueID>();
            uniqueid.uid = UniqueID.GenerateUniqueID();
            building.StartBuild();
            return building;
        }
    }
}
