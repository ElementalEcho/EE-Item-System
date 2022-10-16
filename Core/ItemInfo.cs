using UnityEngine;
using System;
using Sirenix.OdinInspector;

namespace EE.InventorySystem {
    [Serializable]
    public class ItemInfo : IItemInfo {
        [SerializeField, Tooltip("Max item stack.")]
        protected int maxItemStack = 5;
        public int MaxItemStack => maxItemStack;
        [SerializeField,ReadOnly]
        public string prefabGuid = "";
        public string PrefabGuid => prefabGuid;

    }
}


