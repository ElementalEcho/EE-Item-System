using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EE.InventorySystem {
    public interface IInventory {
        int NumberOfFilledSlots { get; }
        bool IsFullAndStacksAreFull { get; }
        Item[] Content { get; }
        bool ContainsItem(IItemInfo item = null, int NumberOfItems = 1);
        bool AddItem(Item item);
        void RemoveAllItems();
        void RemoveItem(bool destroyItems = false, IItemInfo item = null, int NumberOfItems = 1);
        void AddItemAddedEvent(Action<IItemInfo, int> func);
        void AddRemovedAddedEvent(Action<IItemInfo, int, bool> func);
        void AddInventoryAlteredEvent(Action func);
        bool ItemHasFreeSlot(Item item);
        void InventoryOpened();
        void LoadData(InventorySaveData inventorySaveData, List<Item> items);
        public void InventoryAltered();
        public void ItemAdded(IItemInfo itemInfo, int numberOfItems);
        public void ItemRemoved(IItemInfo itemInfo, int numberOfItems, bool destroye);
    }
    [System.Serializable]
    public struct InventorySaveData {
        public int ItemIndex;
        public List<ItemSaveData> Items;
    }
    [System.Serializable]
    public struct ItemSaveData {
        public string itemGuid;
        public int numberOfItems;
    }
}
namespace EE.InventorySystem.Impl {
    [Serializable]
    public class Inventory : IInventory {
        readonly IInventoryData inventoryData;
        public Inventory(IInventoryData inventoryData) {
            this.inventoryData = inventoryData;
            content = new Item[inventoryData.MaxInventorySize];
            foreach (var inventoryItem in inventoryData.BaseItems) {
                AddItem(inventoryItem);
            }
        }
        public void LoadData(InventorySaveData inventorySaveData, List<Item> items) {
            content = new Item[inventoryData.MaxInventorySize];
            foreach (var inventoryItem in items) {
                AddItem(inventoryItem);
            }

        }

        private Item[] content;
        public Item[] Content => content;

        public int NumberOfFilledSlots { get {
                int numberOfItems = 0;
                for (int i = 0; i < content.Length; i++) {
                    if (!Item.IsNull(content[i])) {
                        numberOfItems++;
                    }
                }
                return numberOfItems;
            } 
        }

        public bool IsFullAndStacksAreFull {
            get {
                for (int i = 0; i < content.Length; i++) {
                    if (Item.IsNull(content[i]) || content[i].NumberOfItems < content[i].ItemInfo.MaxItemStack) {
                        return false;
                    }
                }
                return true;
            }
        }
        private event Action<IItemInfo, int> ItemAddedEvent;
        private event Action<IItemInfo, int, bool> ItemRemovedEvent;
        private event Action InventoryAlteredEvent;

        public void InventoryAltered() => InventoryAlteredEvent?.Invoke();
        public void ItemAdded(IItemInfo itemInfo, int numberOfItems) => ItemAddedEvent?.Invoke(itemInfo, numberOfItems);
        public void ItemRemoved(IItemInfo itemInfo, int numberOfItems, bool destroye) => ItemRemovedEvent?.Invoke(itemInfo, numberOfItems, destroye);

        public void AddInventoryAlteredEvent(Action func) {
            InventoryAlteredEvent += func;
        }

        public bool AddItem(Item itemToAdd) {
            bool retval = true;
            return retval;

        }
        public bool ItemHasFreeSlot(Item item) {
            List<int> itemPositions = ContainsItem(item.ItemInfo);
            for (int i = 0; i < itemPositions.Count; i++) {
                Item itemSlot = content[itemPositions[i]];
                int itemsInSlot = item.ItemInfo.MaxItemStack - itemSlot.NumberOfItems;
                if (itemsInSlot > 0) {
                    return true;
                }
            }
            return false;
        }
        protected bool AddItemToArray(IItemInfo itemInfo, int numberToAdd) {
            int i = 0;
            while (i < content.Length) {
                if (Item.IsNull(content[i])) {
                    content[i] = new Item(itemInfo, numberToAdd);
                    return true;
                }
                i++;
            }
            return false;
        }

        public void RemoveAllItems() {
            for (int i = 0; i < content.Length; i++) {
                content[i] = null;
            }
        }

        public List<int> ContainsItem(IItemInfo item) {
            List<int> list = new List<int>();

            for (int i = 0; i < content.Length; i++) {
                if (content[i] != null && content[i].ItemInfo == item) {
                    list.Add(i);
                }
            }
            return list;

        }

        public bool ContainsItem(IItemInfo item = null, int NumberOfItems = 1) {

            List<int> itemPositions = ContainsItem(item);
            if (itemPositions.Count <= 0) {
                return false;
            }
            int numberOfItemsInInventory = 0;
            foreach (var index in itemPositions) {
                numberOfItemsInInventory += content[index].NumberOfItems;
                if (numberOfItemsInInventory >= NumberOfItems) {
                    return true;
                }
            }

            

            return false;
        }

        public void RemoveItem(bool destroyItems = false, IItemInfo item = null, int NumberOfItems = 1) {

        }





        public void AddItemAddedEvent(Action<IItemInfo, int> func) {
            ItemAddedEvent += func;
        }
        public void AddRemovedAddedEvent(Action<IItemInfo, int, bool> func) {
            ItemRemovedEvent += func;
        }
        public void InventoryOpened() {
            Debug.LogWarning("Normal inventory does not implement Open inventoryEvent");
        }

    }

}
