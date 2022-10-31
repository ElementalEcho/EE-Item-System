using EE.Core.PoolingSystem;
using EE.Core.ScriptableObjects;
using EE.Core.UI;
using EE.ItemSystem.Impl;
using EE.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.ItemSystem.UI {

    public class InventoryUIDataComponent : MonoBehaviour, IUIContent {
        [SerializeField]
        private InventorySO inventorySO;
        [SerializeField]
        private ItemDataBaseSO itemDataBaseSO;
        [SerializeField]
        private EventActivatorSO eventActivatorSO;
        [SerializeField]
        private ContentTypeSO contentTypeSO;
        public void Start() {
            inventorySO.InventoryAlteredEvent.Add(eventActivatorSO.Invoke);            
        }
        public List<DisplayData> GetObjects() {
            var displayDatas = new List<DisplayData>();

            for (int i = 0; i < inventorySO.Size; i++) {
                var itemSlot = inventorySO.Get(i);
                var abilityDisplayData = new ItemDisplayData(itemSlot, itemDataBaseSO, contentTypeSO);

                displayDatas.Add(abilityDisplayData);
            }

            return displayDatas;
        }
    }

    public class ItemDisplayData : DisplayData {

        private ItemTypeSO itemTypeSO;
        private Item item;

        private Sprite icon;
        public Sprite Icon => icon;

        public Color Color => Color.white;

        private ContentTypeSO contentTypeSO;

        public ItemDisplayData(Item item, ItemDataBaseSO itemDataBaseSO, ContentTypeSO contentTypeSO) {
            this.item = item;
            this.contentTypeSO = contentTypeSO;
            if (item != null) {
                itemTypeSO = itemDataBaseSO.GetItemType(item.ItemInfo.ID);

                icon = itemTypeSO.ItemToDrop.GetComponent<SpriteRenderer>().sprite;
            }
        }

        public void AddElements(GridElement gridElement) {
            var text = gridElement.GetContent(contentTypeSO).GetComponent<TMPro.TextMeshProUGUI>();

            if (item != null && item.NumberOfItems > 0) {
                text.text = item.NumberOfItems.ToString();
                text.gameObject.SetActive(true);
            }
            else {
                text.gameObject.SetActive(false);
            }
        }
    }
}