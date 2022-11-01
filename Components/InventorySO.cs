using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EE.ItemSystem.Impl {
    public class InventorySO : ScriptableObject, IInventory {
        [SerializeField]
        protected InventoryDataSO inventoryDataSO = null;
        private IInventory inventory;
        private IInventory Inventory { get {
                if (inventory == null) {
                    inventory = new Inventory(inventoryDataSO);
                }
                return inventory;
            } 
        }
        public AddItemDelegate ItemAddedEvent => Inventory.ItemAddedEvent;
        public RemoveItemDelegate ItemRemovedEvent => Inventory.ItemRemovedEvent;
        public ItemDelegate InventoryAlteredEvent => Inventory.InventoryAlteredEvent;
        public event Action InventoryOpenedEvent;

        public void InventoryOpened() {
            InventoryOpenedEvent?.Invoke();
        }

        [ShowInInspector]
        public int Size => Inventory.Size;
        public bool IsFull => NumberOfFilledSlots >= Mathf.Max(inventoryDataSO.MaxInventorySize, 1); //Max inventory Size should be atleast one to prevent infinite loops.

        public int NumberOfFilledSlots => Inventory.NumberOfFilledSlots;

        public Item Get(int index) => Inventory.Get(index);

        public void Replace(int index, Item item) => Inventory.Replace(index, item);

        public bool Add(Item item) {
            return Inventory.Add(item);
        }
        public void Remove(IItemInfo item = null, int numberOfItems = 1) {
            Inventory.Remove(item, numberOfItems);
        }

        public void RemoveAll() {
            Inventory.RemoveAll();
        }

        public bool Contains(IItemInfo item = null, int numberOfItems = 1) => Inventory.Contains(item, numberOfItems);

        public List<Item> GetItems() {
            var items = new List<Item>();

            for (int i = 0; i < Inventory.Size; i++) {
                var item = Inventory.Get(i);
                if (!Item.IsNull(item)) {
                    items.Add(item);
                }
            }
            return items;
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
    }

}
