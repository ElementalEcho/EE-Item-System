using EE.Core;
using EE.Core.PoolingSystem;
using EE.UI;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EE.InventorySystem.Impl {

    public class HasItemComponent : EEMonobehavior, IPoolable {
        public Item Item;

        public Image Icon;
        public TMP_Text numberOfItemsText;
        [SerializeField]
        private PoolableComponent poolableComponent = null;
        public PoolableComponent PoolableComponent => poolableComponent;

        [SerializeField, Tooltip("Containment for the inventory values of the player.")]
        private InventorySO inventoryItemContainment = null;
        public InventorySO InventoryItemContainment => inventoryItemContainment;
        [SerializeField]
        private DragableUIElement dragableUIElement = null;
        private void Start() {
            ChangeItem(Item);
        }
        public void SetInventorySO(InventorySO inventorySO) {
            inventoryItemContainment = inventorySO;
        }
        public void SetParent(GridElement parentRectTransform) {
            dragableUIElement.parentRectTransform = parentRectTransform;
        }
        public void ChangeItem(Item Item) {
            this.Item = Item;
            if (Item != null && Item.NumberOfItems > 0 && Item.ItemInfo != null && Item.ItemInfo.ItemToDrop != null) {
                SetIcon(Item.ItemInfo.ItemToDrop.GetComponent<SpriteRenderer>().sprite);
                SetText(Item.NumberOfItems.ToString());
            }
            else {
                SetIcon(null);
                SetText("");
            }
        }
        public void SetIcon(Sprite itemImage) {
            if (itemImage == null) {
                Icon.gameObject.SetActive(false);
            }
            else {
                Icon.sprite = itemImage;
                Icon.gameObject.SetActive(true);
            }
        }
        public void SetText(string text) {
            numberOfItemsText.text = text;
        }
    }
}
