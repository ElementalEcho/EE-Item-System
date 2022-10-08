using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EE.InventorySystem.Impl {
    public class InventorySO : ScriptableObject, IInventory {
        [SerializeField]
        private InventoryDataSO inventoryDataSO = null;
        private IInventory inventory;
        public IInventory Inventory { get {
                if (inventory == null) {
                    inventory = new Inventory(inventoryDataSO.InventoryData);
                }
                return inventory;
            } 
        }

        public Item CurrentItem => Inventory.CurrentItem;

        public int NumberOfFilledSlots => Inventory.NumberOfFilledSlots;

        public bool IsFull => NumberOfFilledSlots >= Mathf.Max(inventoryDataSO.InventoryData.MaxInventorySize, 1); //Max inventory Size should be atleast one to prevent infinite loops.
        [ShowInInspector]
        public Item[] Content => Inventory.Content;
        public int CurrentItemIndex => Inventory.CurrentItemIndex;

        public bool IsFullAndStacksAreFull => inventory.IsFullAndStacksAreFull;

        public bool AddItem(Item item) {
            //If inventory is full replace current item
            if (!ItemHasFreeSlot(item)) {
                RemoveItem();
            }
            return Inventory.AddItem(item);
        }

        public bool ContainsItem(IItemInfo item = null, int numberOfItems = 1) => Inventory.ContainsItem(item, numberOfItems);

        public void RemoveAllItems() {
            Inventory.RemoveAllItems();
        }

        public void RemoveItem(bool destroyItems = false, IItemInfo item = null, int numberOfItems = 1) {
            Inventory.RemoveItem(destroyItems, item, numberOfItems);
        }

        [Button]
        public void PrintNumberOfItems() {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            foreach (var item in inventory.Content) {
                stringBuilder.AppendLine($"Item: { item.ItemInfo.ItemToDrop.name}. Amount: {item.NumberOfItems}.");
            }
            Debug.Log(stringBuilder.ToString());
        }
        public void AddInventoryAlteredEvent(Action func) {
            Inventory.AddInventoryAlteredEvent(func);
        }
        public void AddItemAddedEvent(Action<IItemInfo, int> func) {
            Inventory.AddItemAddedEvent(func);

        }
        public void AddRemovedAddedEvent(Action<IItemInfo, int, bool> func) {
            Inventory.AddRemovedAddedEvent(func);
        }

        public static void SwitchItemPosition(int position, int newPosition, IInventory oldInventory, IInventory newInventory = null) {
            if (newInventory == null) {
                newInventory = oldInventory;
            }
            Item oldItem = null;
            if (!Item.IsNull(newInventory.Content[newPosition])) {
                oldItem = new Item(newInventory.Content[newPosition].ItemInfo, newInventory.Content[newPosition].NumberOfItems);
            }

            newInventory.Content[newPosition] = !Item.IsNull(oldInventory.Content[position]) ? new Item(oldInventory.Content[position].ItemInfo, oldInventory.Content[position].NumberOfItems): null;

            oldInventory.Content[position] = oldItem;
        }
        public void NextItem() {
            Inventory.NextItem();
        }
        public void PreviousItem() {
            Inventory.PreviousItem();
        }

        public void ChangeItem(int index) {
            Inventory.ChangeItem(index);
        }
        public event Action InventoryOpenedEvent;
        public void InventoryOpened() {
            InventoryOpenedEvent?.Invoke();
        }

        public bool ItemHasFreeSlot(Item item) {
            return !inventory.IsFull || inventory.ItemHasFreeSlot(item);
        }

        public void LoadData(InventorySaveData inventorySaveData, List<Item> items) {
            if (inventory != null) {
                inventory.LoadData(inventorySaveData, items);
            }
        }
    }

}
