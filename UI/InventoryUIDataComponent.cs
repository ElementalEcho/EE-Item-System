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
        protected InventorySO inventorySO;
        [SerializeField]
        protected ItemDataBaseSO itemDataBaseSO;
        [SerializeField]
        protected ContentTypeSO numberOfItemsContentType;

        public List<DisplayData> GetObjects() {
            var displayDatas = new List<DisplayData>();

            for (int i = 0; i < inventorySO.Size; i++) {
                var itemSlot = inventorySO.Get(i);

                Sprite icon = null;
                if (itemSlot != null) {
                    var itemTypeSO = itemDataBaseSO.GetItemType(itemSlot.ItemInfo.ID);
                    icon = itemTypeSO.Icon;
                }
                var abilityDisplayData = new ItemDisplayData(itemSlot, icon, numberOfItemsContentType);

                displayDatas.Add(abilityDisplayData);
            }

            return displayDatas;
        }
    }

    public class ItemDisplayData : DisplayData {

        private readonly Item item;
        private readonly Sprite icon;
        private readonly ContentTypeSO contentTypeSO;

        public Sprite Icon => icon;

        public Color Color => Color.white;


        public ItemDisplayData(Item item, Sprite icon, ContentTypeSO contentTypeSO) {
            this.item = item;
            this.contentTypeSO = contentTypeSO;
            this.icon = icon;

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