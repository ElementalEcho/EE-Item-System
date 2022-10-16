using UnityEngine;
using EE.Core;
using EE.InventorySystem;
using System.Collections.Generic;
using EE.InventorySystem.Impl;
using EE.Core.PoolingSystem;

namespace EE.UI {
    /// <summary>
    /// Displays inventory of the player.
    /// </summary>
    public class InventoryUIController : MonoBehaviour {

        [SerializeField, Tooltip("Containment for the inventory values of the player.")]
        private InventorySO inventoryItemContainment = null;

        [SerializeField, Tooltip("Grid used to display items.")]
        private FlexibleGridLayuout flexibleGridLayuout = null;

        [SerializeField, Tooltip("Button for items.")]
        private GridElement buttonPrefab = null;

        private List<GridElement> uiButtons = new List<GridElement>();
        public ContentTypeSO itemContentType = null;
        public HasItemComponent itemContentPrefab = null;
        [SerializeField]
        private ItemDataBaseSO itemDataBaseSO = null;
        private void Start() {
            inventoryItemContainment.AddInventoryAlteredEvent(InventoryUpdated);
            InventoryUpdated();
        }

        /// <summary>
        /// Updates UI when inventory values are changed.
        /// </summary>
        private void InventoryUpdated() {

            CreateCraftableObjects();
        }

        public void CreateCraftableObjects() {
            ClearButtons();
            if (inventoryItemContainment.NumberOfFilledSlots <= 0) {
                flexibleGridLayuout.gameObject.SetActive(false);
                return;
            }
            flexibleGridLayuout.gameObject.SetActive(true);

            foreach (Item inventoryItem in inventoryItemContainment.Content) {
                if (Item.IsNull(inventoryItem)) {
                    continue;
                }
                GridElement gridElement = PoolManager.SpawnObjectAsChild(buttonPrefab.PoolableComponent, flexibleGridLayuout.transform).GetComponent<GridElement>();
                gridElement.ResetScale();

                HasItemComponent hasItemComponent = PoolManager.SpawnObjectAsChild(itemContentPrefab.PoolableComponent, flexibleGridLayuout.transform).GetComponent<HasItemComponent>();
                hasItemComponent.ChangeItem(inventoryItem);
                var itemtype = itemDataBaseSO.GetItemType(inventoryItem.ItemInfo.PrefabGuid);
                hasItemComponent.SetIcon(itemtype.ItemToDrop.GetComponent<SpriteRenderer>().sprite);
                hasItemComponent.SetText(inventoryItem.NumberOfItems.ToString());
                gridElement.ReplaceOrAddContent(itemContentType, hasItemComponent.gameObject);

                if (hasItemComponent.TryGetComponent(out DragableUIElement dragelement)) {
                    dragelement.parentRectTransform = gridElement;
                }              
                 

                uiButtons.Add(gridElement);
            }

        }

        public void ClearButtons() {
            foreach (GridElement inventoryUI in uiButtons) {
                PoolManager.ReleaseObject(inventoryUI.PoolableComponent);
            }
            uiButtons.Clear();
        }
    }

}
