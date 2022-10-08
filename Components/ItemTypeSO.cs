using UnityEngine;
using System;
using System.Collections.Generic;

namespace EE.InventorySystem {
    public class ItemTypeSO : ScriptableObject, IItemType {
        [SerializeField, PropertyOnly]
        protected ItemInfo itemToDrop = null;
        public IItemInfo ItemType => itemToDrop;

#if UNITY_EDITOR
        private void OnValidate() {
            if (string.IsNullOrEmpty(itemToDrop.prefabGuid)) {
                itemToDrop.prefabGuid = Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
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



