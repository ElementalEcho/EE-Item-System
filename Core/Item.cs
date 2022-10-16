
using System;

namespace EE.InventorySystem {
    [Serializable]
    public class Item : IItem {

        private IItemInfo itemType = null;
        public IItemInfo ItemInfo => itemType;

        private int numberOfItems = 0;
        public int NumberOfItems => numberOfItems;

        public Item(IItemInfo itemType, int numberOfItems) {
            this.itemType = itemType;
            this.numberOfItems = numberOfItems;

        }
        public Item(IItemInfo itemType) {
            this.itemType = itemType;
            this.numberOfItems = 1;
        }
        public void AddItems(int numberToAdd) {
            numberOfItems += numberToAdd;
        }
        public void RemoveItems(int numberToReduce) {
            numberOfItems -= numberToReduce;
        }

        public static bool IsNull(Item item) {
            return item == null || item.numberOfItems <= 0;
        }
    }
}
namespace EE.InventorySystem {
    public interface IItem {
        public IItemInfo ItemInfo { get; }
        public int NumberOfItems { get; }
        void AddItems(int numberToAdd);
        void RemoveItems(int numberToReduce);
    }
}
