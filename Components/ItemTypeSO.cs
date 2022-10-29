﻿using UnityEngine;
using System;
using System.Collections.Generic;
using EE.Core;
using EE.Core.PoolingSystem;
using Sirenix.OdinInspector;

namespace EE.ItemSystem {
    public class ItemTypeSO : ScriptableObject, IItemType {
        [SerializeField]
        protected ItemInfo itemToDrop = new ItemInfo();
        public IItemInfo ItemType => itemToDrop;

        [SerializeField, Tooltip("Item created when this item is dropped.")]
        protected PoolableReference itemPrefab = null;
        public PoolableComponent ItemToDrop => itemPrefab != null ? itemPrefab.Value : null;

        [SerializeField, Tooltip("Amount to mana this cost or gives when consumed.")]
        protected int manaToGive = 10;
        public int ManaToGive => manaToGive;

        [SerializeField]
        private GenericActionSO[] startItemUseEffects = new GenericActionSO[0];
        public GenericActionSO[] StartItemUseEffects => startItemUseEffects;

        [SerializeField]
        private GenericActionSO[] attackItemUseEffects = new GenericActionSO[0];
        public GenericActionSO[] AttackItemUseEffects => attackItemUseEffects;
        [SerializeField]
        private GenericActionSO[] thrownItemEffects = new GenericActionSO[0];
        public GenericActionSO[] ThrownItemEffects => thrownItemEffects;


        [ShowInInspector, ReadOnly]
        public string PrefabGuid => itemToDrop.ID;
        [ShowInInspector, ReadOnly]
        public string Name => itemToDrop.Name;
#if UNITY_EDITOR

        [Button]
        private void CreateItemInfo(string name, int maxItemSize) {
            itemToDrop = new ItemInfo(maxItemSize, name);
        }
        private void OnValidate() {
            if (itemToDrop == null) {
                itemToDrop = new ItemInfo();
            }

        }
#endif
    }
    [Serializable]
    public class InspectorItem {
        [SerializeField]
        private ItemTypeSO itemType = null;

        [SerializeField]
        private int numberOfItems = 0;

        public InspectorItem(ItemTypeSO itemType, int numberOfItems) {
            this.itemType = itemType;
            this.numberOfItems = numberOfItems;
        }

        public Item GetItem() => new(itemType.ItemType, numberOfItems);
    }

    public static class InspectorItemExtensions {
        public static Item[] GetItems(this InspectorItem[] inspectorItem) {
            Item[] items = new Item[inspectorItem.Length];
            for (int i = 0; i < inspectorItem.Length; i++) {
                items[i] = inspectorItem[i].GetItem();
            }
            return items;
        }
        public static List<Item> GetItems(this List<InspectorItem> inspectorItem) {
            List<Item> items = new List<Item>();
            for (int i = 0; i < inspectorItem.Count; i++) {
                var item = inspectorItem[i].GetItem();
                items.Add(item);
            }
            return items;
        }
    }
}



