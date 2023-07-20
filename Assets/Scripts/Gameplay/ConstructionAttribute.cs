using System;
using UnityEngine;

namespace FarmWolffun
{
    [RequireComponent(typeof(Factory))]
    public class ConstructionAttribute : MonoBehaviour
    {
        [Header("Attributes")]
        public AttributeData[] attributes;  //List of available attributes

        private Construction _construction;

        private void Awake()
        {
            _construction = GetComponent<Construction>();
        }
        
        void Start()
        {
            //Init attributes
            foreach (AttributeData attr in attributes)
            {
                if (!ConstructionData.HasAttribute(attr.type))
                    ConstructionData.SetAttributeValue(attr.type, attr.start_value, attr.max_value);
            }
        }
        
        public void AddAttribute(AttributeType type, float value)
        {
            if (HasAttribute(type))
            {
                ConstructionData.AddAttributeValue(type, value, GetAttributeMax(type));
            }
        }

        public void SetAttribute(AttributeType type, float value)
        {
            if (HasAttribute(type))
            {
                ConstructionData.SetAttributeValue(type, value, GetAttributeMax(type));
            }
        }

        public bool HasAttribute(AttributeType type)
        {
            return ConstructionData.HasAttribute(type) && GetAttribute(type) != null;
        }

        public float GetAttributeValue(AttributeType type)
        {
            return ConstructionData.GetAttributeValue(type);
        }

        public float GetAttributeMax(AttributeType type)
        {
            AttributeData adata = GetAttribute(type);
            if (adata != null)
                return adata.max_value;
            return 100f;
        }

        public AttributeData GetAttribute(AttributeType type)
        {
            foreach (AttributeData attr in attributes)
            {
                if (attr.type == type)
                    return attr;
            }
            return null;
        }
        
        public SaveConstructionData ConstructionData
        {
            get { return _construction.SData; }
        }
        public Construction Construction { get { return _construction; } }
    }
}