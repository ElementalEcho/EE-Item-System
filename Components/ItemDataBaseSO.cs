using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace EE.InventorySystem {

    public class ItemDataBaseSO : ScriptableObject {
        [SerializeField, InlineEditor]
        private List<ItemTypeSO> prefabGuidList = new List<ItemTypeSO>();
        public List<ItemTypeSO> ItemList => prefabGuidList;

        [ReadOnly, ShowInInspector]
        private Dictionary<string, ItemTypeSO> prefabDictonary = new Dictionary<string, ItemTypeSO>();
        public IItemType GetItemType(string guid) {
            if (prefabDictonary.Count == 0) {
                InitDictonary();
            }
            if (!prefabDictonary.TryGetValue(guid, out var poolableReference)) {
                Debug.LogError("Prefab missing: " + guid);
                return null;
            }
            return poolableReference;
        }

        public void InitDictonary() {
            foreach (var prefabGuid in prefabGuidList) {
                prefabDictonary.Add(prefabGuid.ItemType.PrefabGuid, prefabGuid);
            }
        }
    }

}