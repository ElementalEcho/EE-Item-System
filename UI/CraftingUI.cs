using EE.Core;
using EE.Core.PoolingSystem;
using EE.Core.ScriptableObjects;
using EE.ItemSystem;
using EE.ItemSystem.Crafting;
using EE.ItemSystem.Crafting.Impl;
using EE.ItemSystem.Impl;
using EE.UI.Extensions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EE.UI {
    public class CraftingUI : Singleton<CraftingUI> {
        [SerializeField]
        private CrafterSO craftingController = null;
        [SerializeField]
        private InventorySO inventorySO = null;

        public GameObject craftuingUI = null;
        public List<GridElement> additionalItemsWindows = null;

        public HasItemComponent resultWindow = null;


        public GridElement craftButton = null;

        public MainInventoryUI mainInventoryUI;
        Dictionary<IItemInfo, GridElement> disabledItems = new Dictionary<IItemInfo, GridElement>();

        [SerializeField, Tooltip("Containment for the inventory values of the player.")]

        private bool menuOpen = false;

        public ContentTypeSO itemContentType = null;

        [SerializeField, Tooltip("Button for items.")]
        private GridElement craftingButtonElement = null;
        [SerializeField, Tooltip("Grid used to display items.")]
        private FlexibleGridLayuout flexibleGridLayuout = null;
        [SerializeField, Tooltip("Grid used to display items.")]
        private DragAreaElement dragAreaElement = null;

        [SerializeField]
        private EventActivatorSO pauseEvent = null;

        private void Start() {
            inventorySO.InventoryOpenedEvent += EnableCraftingUI;
            CreateButtons();
            ItemChanged();
            pauseEvent.Add(DisableUI);
        }
        [Button]
        public void EnableCraftingUI() {
            if (menuOpen) {
                craftuingUI.SetActive(false);
                menuOpen = false;
                CreateButtons();
            }
            else {
                CreateButtons();
                craftuingUI.SetActive(true);
                menuOpen = true;
            }
            ItemChanged();
        }
        public void DisableUI() {
            craftuingUI.SetActive(false);
            menuOpen = false;
        }
        [SerializeField]
        private int numberOfCraftingItems = 4;
        [Button]
        public void CreateButtons() {
            ClearButtons();
            for (int i = 0; i < numberOfCraftingItems; i++) {
                GridElement gridElement = PoolManager.SpawnObjectAsChild(craftingButtonElement.PoolableComponent, flexibleGridLayuout.transform).GetComponent<GridElement>();
                gridElement.id = i;
                gridElement.ResetScale();

                //gridElement.ButtonClickedWithLeftEvent += RemoveItem;
                additionalItemsWindows.Add(gridElement);
                if (gridElement.TryGetComponent<DragPositionElement>(out var dragPositionElement)) {
                    dragAreaElement.dragPositionElements.Add(dragPositionElement);
                    //dragPositionElement.ElementDraggedToThisEvent += ItemDroped;
                }
            }
        }
        public void ClearButtons() {
            foreach (GridElement inventoryUI in additionalItemsWindows) {
                PoolManager.ReleaseObject(inventoryUI.PoolableComponent);
            }
            additionalItemsWindows.Clear();
            dragAreaElement.dragPositionElements.Clear();
        }
        [Button]
        public void ItemChanged() {
            List<Item> availableItems = new List<Item>();
            //Remove items from crafting window that are no longer in the inventory
            foreach (var window in additionalItemsWindows) {
                HasItemComponent hasItemComponent = window.HasContent(itemContentType) ? window.GetContent(itemContentType).GetComponent<HasItemComponent>() : null;
                if (hasItemComponent == null) {
                    continue;
                }
                if (hasItemComponent.Item != null && inventorySO.Contains(hasItemComponent.Item.ItemInfo)) {
                    hasItemComponent.ChangeItem(hasItemComponent.Item);
                    availableItems.Add(hasItemComponent.Item);
                    if (disabledItems.TryGetValue(hasItemComponent.Item.ItemInfo, out var gridElement)) {
                        gridElement.GetComponent<HasItemComponent>().ChangeItem(hasItemComponent.Item);
                    }
                }
                else {
                    if (hasItemComponent.Item != null && disabledItems.TryGetValue(hasItemComponent.Item.ItemInfo, out var gridElement)) {
                        //gridElement.Enable();
                        gridElement.GetComponent<HasItemComponent>().ChangeItem(null);
                        disabledItems.Remove(hasItemComponent.Item.ItemInfo);
                        hasItemComponent.ChangeItem(null);
                    }
                }
            }
            foreach (var item in disabledItems) {
                //item.Value.Disable();
            }
            //Update Recipe window if can no longer craft the item.
            if (craftingController.CanCraftÍtem(availableItems, out IRecipe recipe)) {
                resultWindow.ChangeItem(recipe.Result);
                //craftButton.Enable();
            }
            else {
                //craftButton.Disable();
                resultWindow.ChangeItem(null);
            }            
        }

        public void AddItemToCraftingUI(GridElement gridElement) {
            GameObject hasItems = gridElement.GetContent(itemContentType);
            if (hasItems == null) {
                return;
            }
            foreach (var window in additionalItemsWindows) {
                if (!window.HasContent(itemContentType)) {
                    window.ReplaceOrAddContent(itemContentType, hasItems);
                    gridElement.RemoveContent(itemContentType);
                    break;
                }
            }        
            ItemChanged();
        }

        public void Craft(GridElement gridElement) {
            List<Item> items = new List<Item>();
            foreach (var window in additionalItemsWindows) {
                GameObject gameObject = window.GetContent(itemContentType);
                if (gameObject == null) {
                    continue;
                }
                Item item = gameObject.GetComponent<HasItemComponent>().Item;
                items.Add(item);
            }
            Item craftedItem = craftingController.Craft(items);

            ItemChanged();
        }

        public void RemoveItem(GridElement gridElement) {
            if (!gridElement.HasContent(itemContentType)) {
                return;
            }
            HasItemComponent hasItems = gridElement.GetContent(itemContentType).GetComponent<HasItemComponent>();

            foreach (var window in mainInventoryUI.uiButtons) {
                if (!window.HasContent(itemContentType)) {
                    window.ReplaceOrAddContent(itemContentType, hasItems.gameObject);
                    gridElement.RemoveContent(itemContentType);
                    break;
                }
            }
            ItemChanged();
        }

        public void OnDisable() {
            pauseEvent.Remove(DisableUI);
        }
        public void ReturnItemToInventory(GridElement gridElement,GameObject hasItems) {
            foreach (var window in mainInventoryUI.uiButtons) {
                if (!window.HasContent(itemContentType)) {
                    window.ReplaceOrAddContent(itemContentType, hasItems);
                    gridElement.RemoveContent(itemContentType);
                    break;
                }
            }

            CraftingUI.Instance.ItemChanged();
        }
    }

}
