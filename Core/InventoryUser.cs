using System;
using System.Collections.Generic;

namespace EE.InventorySystem {
    public interface IInventoryUser {
        Item CurrentItem { get; }
        int CurrentItemIndex { get; }
        void AddInventoryAlteredEvent(Action func);
        void NextItem();
        void PreviousItem();
        void ChangeItem(int index);
        bool ContainsItem(IItemInfo item = null, int NumberOfItems = 1);
        bool AddItem(Item item);
        void RemoveAllItems();
        void RemoveItem(bool destroyItems = false, IItemInfo item = null, int NumberOfItems = 1);
        void AddItemAddedEvent(Action<IItemInfo, int> func);
        void AddRemovedAddedEvent(Action<IItemInfo, int, bool> func);
        bool ItemHasFreeSlot(Item item);
        void InventoryOpened();
        void LoadData(InventorySaveData inventorySaveData, List<Item> items);
        Item[] Content { get; }
        bool IsFull { get; }
        bool IsFullAndStacksAreFull { get; }
        int NumberOfFilledSlots { get; }

    }
    public class InventoryUser : IInventoryUser {
        private readonly IInventory inventory;
        public Item[] Content => inventory.Content;

        public Item CurrentItem => inventory.Content.Length > _currentItemIndex && inventory.Content[_currentItemIndex] != null ? inventory.Content[_currentItemIndex] : null;
        private int _currentItemIndex = 0;

        public int CurrentItemIndex => _currentItemIndex;
        private event Action InventoryAlteredEvent;

        public bool IsFull {
            get {
                for (int i = 0; i < inventory.Content.Length; i++) {
                    if (Item.IsNull(inventory.Content[i])) {
                        return false;
                    }
                }
                return true;
            }
        }
        public int NumberOfFilledSlots => inventory.NumberOfFilledSlots;
        public bool IsFullAndStacksAreFull => inventory.IsFullAndStacksAreFull;

        public InventoryUser(IInventory inventory) {
            this.inventory = inventory;
        }
        public void NextItem() {
            _currentItemIndex++;
            if (_currentItemIndex >= inventory.Content.Length) {
                _currentItemIndex = 0;
            }
            InventoryAlteredEvent?.Invoke();

        }
        public void PreviousItem() {
            _currentItemIndex--;
            if (_currentItemIndex < 0) {
                _currentItemIndex = inventory.Content.Length - 1;
            }
            InventoryAlteredEvent?.Invoke();
        }

        public void ChangeItem(int index) {
            _currentItemIndex = index;
            InventoryAlteredEvent?.Invoke();
        }
        public void AddInventoryAlteredEvent(Action func) {
            InventoryAlteredEvent += func;
        }
        private event Action<IItemInfo, int> ItemAddedEvent;
        private event Action<IItemInfo, int, bool> ItemRemovedEvent;



        public bool AddItem(Item itemToAdd) {
            bool retval = true;
            List<int> itemPositions = ContainsItem(itemToAdd.ItemInfo);
            int numberOfItems = itemToAdd.NumberOfItems;
            for (int i = 0; i < itemPositions.Count; i++) {
                if (numberOfItems <= 0) {
                    break;
                }
                Item itemSlot = inventory.Content[itemPositions[i]];
                int itemsInSlot = itemToAdd.ItemInfo.MaxItemStack - itemSlot.NumberOfItems;
                if (numberOfItems > 0) {
                    int numberofItemsToAdd = Math.Min(itemsInSlot, numberOfItems);
                    inventory.Content[itemPositions[i]].AddItems(numberofItemsToAdd);
                    numberOfItems -= numberofItemsToAdd;
                }
            }

            if (!IsFull) {
                AddItemToArray(itemToAdd.ItemInfo, numberOfItems);
            }
            else {
                retval = false;
            }

            ItemAddedEvent?.Invoke(itemToAdd.ItemInfo, itemToAdd.NumberOfItems);
            InventoryAlteredEvent?.Invoke();
            return retval;

        }
        public bool ItemHasFreeSlot(Item item) {
            List<int> itemPositions = ContainsItem(item.ItemInfo);
            for (int i = 0; i < itemPositions.Count; i++) {
                Item itemSlot = inventory.Content[itemPositions[i]];
                int itemsInSlot = item.ItemInfo.MaxItemStack - itemSlot.NumberOfItems;
                if (itemsInSlot > 0) {
                    return true;
                }
            }
            return false;
        }
        protected bool AddItemToArray(IItemInfo itemInfo, int numberToAdd) {
            int i = 0;
            while (i < inventory.Content.Length) {
                if (Item.IsNull(inventory.Content[i])) {
                    inventory.Content[i] = new Item(itemInfo, numberToAdd);
                    return true;
                }
                i++;
            }
            return false;
        }

        public void RemoveAllItems() {
            inventory.RemoveAllItems();
        }

        private List<int> ContainsItem(IItemInfo item) {
            List<int> list = new List<int>();

            for (int i = 0; i < inventory.Content.Length; i++) {
                if (inventory.Content[i] != null && inventory.Content[i].ItemInfo == item) {
                    list.Add(i);
                }
            }
            return list;

        }

        public bool ContainsItem(IItemInfo item = null, int NumberOfItems = 1) {
            if (item == null) {
                return CurrentItem != null && CurrentItem.ItemInfo != null;
            }
            else {
                List<int> itemPositions = ContainsItem(item);
                if (itemPositions.Count <= 0) {
                    return false;
                }
                int numberOfItemsInInventory = 0;
                foreach (var index in itemPositions) {
                    numberOfItemsInInventory += inventory.Content[index].NumberOfItems;
                    if (numberOfItemsInInventory >= NumberOfItems) {
                        return true;
                    }
                }

            }

            return false;
        }

        public void RemoveItem(bool destroyItems = false, IItemInfo item = null, int NumberOfItems = 1) {
            List<int> indexes;
            //Remove current item by default
                if ((indexes = ContainsItem(item)).Count > 0) {
                var numberOfItemsToRemove = NumberOfItems;
                for (int i = 0; i < indexes.Count; i++) {
                    inventory.Content[indexes[i]].RemoveItems(numberOfItemsToRemove);
                    numberOfItemsToRemove -= inventory.Content[indexes[i]].NumberOfItems;

                    if (inventory.Content[indexes[i]].NumberOfItems <= 0) {
                        inventory.Content[indexes[i]] = null;
                    }
                    if (numberOfItemsToRemove <= 0) {
                        break;
                    }
                }
            }
            ItemRemovedEvent?.Invoke(item, NumberOfItems, destroyItems);
            InventoryAlteredEvent?.Invoke();
        }





        public void AddItemAddedEvent(Action<IItemInfo, int> func) {
            ItemAddedEvent += func;
        }
        public void AddRemovedAddedEvent(Action<IItemInfo, int, bool> func) {
            ItemRemovedEvent += func;
        }
        public void InventoryOpened() {
            //Debug.LogWarning("Normal inventory does not implement Open inventoryEvent");
        }

        public void LoadData(InventorySaveData inventorySaveData, List<Item> items) {
            _currentItemIndex = inventorySaveData.ItemIndex;
            inventory.LoadData(inventorySaveData, items);

        }
    }
}