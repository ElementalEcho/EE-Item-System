using UnityEngine;
using System.Collections.Generic;
using EE.ItemSystem.Impl;
using Sirenix.OdinInspector;
using EE.Core.ScriptableObjects;
using EE.Core.PoolingSystem;

namespace EE.UI {
    /// <summary>
    /// Displays inventory of the player.
    /// </summary>
    public class MainInventoryUI : MonoBehaviour {

        [SerializeField, Tooltip("Containment for the inventory values of the player.")]
        private InventorySO inventoryItemContainment = null;

        [SerializeField, Tooltip("Grid used to display items.")]
        private FlexibleGridLayuout flexibleGridLayuout = null;

        [SerializeField, Tooltip("Button for items.")]
        private GridElement buttonPrefab = null;

        public List<GridElement> uiButtons = new List<GridElement>();
        private bool active = false;

        [SerializeField]
        private DragAreaElement dragAreaElement = null;
        [SerializeField]
        private ContentTypeSO itemContentType = null;
        [SerializeField]
        private HasItemComponent itemContentPrefab = null;


        [SerializeField]
        private EventActivatorSO pauseEvent = null;
        [SerializeField]
        private int numberOfSlots = 50;
        [SerializeField]
        private bool canBeDisabled = true;
        private void Start() {
            inventoryItemContainment.InventoryAlteredEvent.Add(InventoryUpdated);
            InventoryUpdated();

            if (canBeDisabled) {
                inventoryItemContainment.InventoryOpenedEvent += EnableUI;
            }

            if (pauseEvent != null) {
                pauseEvent.Add(DisableUI);
            }
        }
        public void EnableUI() {
            if (active) {
                DisableUI();
            }
            else {
                CreateButtons();
                flexibleGridLayuout.gameObject.SetActive(true);
                active = true;
            }
        }
        public void DisableUI() {
            flexibleGridLayuout.gameObject.SetActive(false);
            active = false;
        }
        /// <summary>
        /// Updates UI when inventory values are changed.
        /// </summary>
        private void InventoryUpdated() {
            CreateButtons();
        }
        [Button]
        public void CreateButtons() {
            ClearButtons();
            for (int i = 0; i < numberOfSlots; i++) {
                //Dont create more than max inventory size
                if (i >= inventoryItemContainment.Size) {
                    break;
                }
                GridElement gridElement = PoolManager.SpawnObjectAsChild(buttonPrefab.PoolableComponent, flexibleGridLayuout.transform).GetComponent<GridElement>();
                gridElement.id = i;
                gridElement.ResetScale();

                //if (inventoryItemContainment.CurrentItemIndex == i) {
                //    gridElement.Selected = true;
                //    gridElement.ButtonEnter();
                //}
                //else {
                    gridElement.Selected = false;
                    gridElement.ButtonExit();
                //}
                var item = inventoryItemContainment.Get(i);
                if (item != null) {
                    HasItemComponent hasItemComponent = PoolManager.SpawnObjectAsChild(itemContentPrefab.PoolableComponent, flexibleGridLayuout.transform).GetComponent<HasItemComponent>();
                    hasItemComponent.ChangeItem(item);
                    hasItemComponent.SetInventorySO(inventoryItemContainment);
                    hasItemComponent.SetParent(gridElement);
                    gridElement.ReplaceOrAddContent(itemContentType, hasItemComponent.gameObject);
                }
                //gridElement.ButtonClickedWithLeftEvent += InventoryButtonPressed;
                uiButtons.Add(gridElement);
                if (gridElement.TryGetComponent<DragPositionElement>(out var dragPositionElement)) {
                    dragAreaElement.dragPositionElements.Add(dragPositionElement);
                    dragPositionElement.ElementDraggedToThisEvent += ItemDroped;
                }
            }
        }

        public void ClearButtons() {
            foreach (GridElement inventoryUI in uiButtons) {
                PoolManager.ReleaseObject(inventoryUI.PoolableComponent);
            }
            uiButtons.Clear();
            dragAreaElement.dragPositionElements.Clear();
        }

        public void ItemDroped(DragPositionElement dragPositionElement, DragableUIElement dragableUIElement) {
            GridElement oldElement = dragableUIElement.parentRectTransform;
            GridElement newElement = dragPositionElement.GetComponent<GridElement>();

            HasItemComponent dragableHasItems = dragableUIElement.GetComponent<HasItemComponent>();

            int oldElementPosition = oldElement.id;
            int newElementPosition = newElement.id;


            dragableUIElement.parentRectTransform = dragPositionElement.GetComponent<GridElement>();

            GameObject oldContent = null;
            InventorySO oldInventorySO = dragableHasItems.InventoryItemContainment;

            if (newElement.HasContent(itemContentType)) {
                oldContent = newElement.GetContent(itemContentType);
            }

            newElement.ReplaceOrAddContent(itemContentType,dragableUIElement.gameObject);

            if (oldContent != null) {
                oldElement.ReplaceOrAddContent(itemContentType, oldContent);
                DragableUIElement dragableUI = oldContent.GetComponent<DragableUIElement>();
                dragableUI.parentRectTransform = oldElement.GetComponent<GridElement>();
            }
            else {
                oldElement.RemoveContent(itemContentType);

            }
            dragableHasItems.SetInventorySO(inventoryItemContainment);

            InventorySO.SwitchItemPosition(oldElementPosition, newElementPosition, oldInventorySO, inventoryItemContainment);
            InventoryUpdated();
        }
        public void OnDisable() {
            if (pauseEvent != null) {
                pauseEvent.Remove(DisableUI);
            }
        }
    }

}
