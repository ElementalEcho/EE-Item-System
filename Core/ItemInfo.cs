using EE.Core;
using UnityEngine;
using System;
using EE.Core.PoolingSystem;
using Sirenix.OdinInspector;

namespace EE.InventorySystem {
    [Serializable]
    public class ItemInfo : IItemInfo {
        [SerializeField, Tooltip("Item created when this item is dropped.")]
        protected PoolableReference itemToDrop = null;
        public PoolableComponent ItemToDrop => itemToDrop != null ? itemToDrop.Value : null;

        [SerializeField, Tooltip("Amount to mana this cost or gives when consumed.")]
        protected int manaToGive = 10;
        public int ManaToGive => manaToGive;

        [SerializeField, Tooltip("Max item stack.")]
        protected int maxItemStack = 5;
        public int MaxItemStack => maxItemStack;
        [SerializeField,ReadOnly]
        public string prefabGuid = "";
        public string PrefabGuid => prefabGuid;

        [SerializeField]
        private GenericActionSO[] startItemUseEffects = new GenericActionSO[0];
        public GenericActionSO[] StartItemUseEffects => startItemUseEffects;
        [SerializeField]
        private GenericActionSO[] attackItemUseEffects = new GenericActionSO[0];
        public GenericActionSO[] AttackItemUseEffects => attackItemUseEffects;

        [SerializeField]
        private GenericActionSO[] thrownItemEffects = new GenericActionSO[0];
        public GenericActionSO[] ThrownItemEffects => thrownItemEffects;
    }
}


