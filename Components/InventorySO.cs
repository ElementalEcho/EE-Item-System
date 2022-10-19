using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EE.ItemSystem.Impl {
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


        public int NumberOfFilledSlots => Inventory.NumberOfFilledSlots;

        public bool IsFull => NumberOfFilledSlots >= Mathf.Max(inventoryDataSO.InventoryData.MaxInventorySize, 1); //Max inventory Size should be atleast one to prevent infinite loops.
        [ShowInInspector]
        public int Size => Inventory.Size;

        public AddItemDelegate ItemAddedEvent => Inventory.ItemAddedEvent;

        public RemoveItemDelegate ItemRemovedEvent => Inventory.ItemRemovedEvent;

        public ItemDelegate InventoryAlteredEvent => Inventory.InventoryAlteredEvent;

        //public int CurrentItemIndex => Inventory.CurrentItemIndex;

        public bool Add(Item item) {
            //If inventory is full replace current item
            //if (!ItemHasFreeSlot(item)) {
            //    RemoveItem();
            //}
            return Inventory.Add(item);
        }

        public bool Contains(IItemInfo item = null, int numberOfItems = 1) => Inventory.Contains(item, numberOfItems);

        public void RemoveAll() {
            Inventory.RemoveAll();
        }

        public void Remove(bool destroyItems = false, IItemInfo item = null, int numberOfItems = 1) {
            Inventory.Remove(destroyItems, item, numberOfItems);
        }

        public static void SwitchItemPosition(int position, int newPosition, IInventory oldInventory, IInventory newInventory = null) {
            if (newInventory == null) {
                newInventory = oldInventory;
            }
            Item oldItem = null;
            Item currentItem = newInventory.Get(newPosition);
            if (!Item.IsNull(currentItem)) {
                oldItem = new Item(currentItem.ItemInfo, currentItem.NumberOfItems);
            }
            Item oldInventoryItem = oldInventory.Get(position);
            newInventory.Replace(newPosition, oldInventoryItem);
            oldInventory.Replace(position,oldItem);
        }


        public event Action InventoryOpenedEvent;
        public void InventoryOpened() {
            InventoryOpenedEvent?.Invoke();
        }

        public void LoadData(InventorySaveData inventorySaveData, List<Item> items) {
            if (inventory != null) {
                inventory.LoadData(inventorySaveData, items);
            }
        }

        public Item Get(int index) => inventory.Get(index);

        public void Replace(int index, Item item) => inventory.Replace(index, item);

        public List<Item> GetItems() {
            var items = new List<Item>();

            for (int i = 0; i < inventory.Size; i++) {
                var item = inventory.Get(i);
                if (!Item.IsNull(item)) { 
                    items.Add(item);
                }
            }
            return items;
        }
    }

}
