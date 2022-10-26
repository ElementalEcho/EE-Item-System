using EE.ItemSystem;
using NUnit.Framework;
using System.Collections.Generic;

namespace Tests {
    public class InventoryTests {
        internal class TestInventoryData : IInventoryData {
            public TestInventoryData(int maxInventorySize, List<Item> baseItems) {
                MaxInventorySize = maxInventorySize;
                BaseItems = baseItems;
            }
            public int MaxInventorySize { get; set; }
            public List<Item> BaseItems { get; set; }
        }
        internal class TestInventoryDataWithMaxItems : IInventoryData {
            public TestInventoryDataWithMaxItems(int maxInventorySize) {
                MaxInventorySize = maxInventorySize;
                BaseItems = new List<Item>();
                for (int i = 0; i < maxInventorySize; i++) {
                    BaseItems.Add(new Item(new ItemInfo(), maxInventorySize));
                }
            }                       
            public int MaxInventorySize { get; set; }
            public List<Item> BaseItems { get; set; }
        }
        #region Init
        [Test]
        public void Inventory_Should_Init_Data_Correctly() {
            var item = new ItemInfo();
            var inventoryItem = new Item(item, 1);
            var baseItems = new List<Item>() {
                inventoryItem
            };
            TestInventoryData testInventoryDataWithItems = new TestInventoryData(5, baseItems);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            Assert.AreEqual(inventory.NumberOfFilledSlots, 1);
            //Assert.AreEqual(inventory.CurrentItem.ItemInfo, item);
            //Assert.AreEqual(inventory.CurrentItem.NumberOfItems, 1);
            //Assert.AreEqual(inventory.Content[0].ItemInfo, item);
        }
        [Test]
        public void Inventory_Should_Correctly_Show_InventoryIsFull() {
            int inventorySize = 5;
            TestInventoryDataWithMaxItems testInventoryDataWithItems = new TestInventoryDataWithMaxItems(inventorySize);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            Assert.AreEqual(inventory.NumberOfFilledSlots, inventorySize);
            Assert.IsTrue(inventory.IsFull);
        }
        #endregion
        #region Add item
        [Test]
        public void AddItem_Should_AddItem() {
            TestInventoryData testInventoryDataWithItems = new TestInventoryData(10, new List<Item>());
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            var item1 = new ItemInfo();
            var item2 = new ItemInfo();

            List<Item> itemsToAdd = new List<Item>() {
                new Item(item1, 2),
                new Item(item2, 1),
                new Item(item2, 3),

            };
            //foreach (var inventoryItem in itemsToAdd) {
            //    inventory.AddItem(inventoryItem);
            //}
            //Assert.AreEqual(inventory.NumberOfFilledSlots, 2);

            //Assert.AreEqual(inventory.Content[0].ItemInfo, item1);
            //Assert.AreEqual(inventory.Content[0].NumberOfItems, 2);

            //Assert.AreEqual(inventory.Content[1].ItemInfo, item2);
            //Assert.AreEqual(inventory.Content[1].NumberOfItems, 4);
        }

        [Test]
        public void When_Inventory_IsFull_AddItems_Should_DoNothing() {
            int inventorySize = 5;
            TestInventoryDataWithMaxItems testInventoryDataWithItems = new TestInventoryDataWithMaxItems(inventorySize);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            Assert.AreEqual(inventory.NumberOfFilledSlots, inventorySize);
            Assert.IsTrue(inventory.IsFull);

            //inventory.AddItem(new Item(new ItemInfo(), 5));

            Assert.AreEqual(inventory.NumberOfFilledSlots, inventorySize);

        }
        #endregion
        #region Contains
        [Test]
        public void ContainsItem_Should_Fail_IfItemIsNotInInventory() {
            TestInventoryData testInventoryDataWithItems = new TestInventoryData(10, new List<Item>());
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            //var containsItem = inventory.ContainsItem(new ItemInfo());
            //Assert.IsFalse(containsItem);
        }
        [Test]
        public void ContainsItem_Should_BeTrue_IfItemIsInInventory() {
            var item = new ItemInfo();

            TestInventoryData testInventoryDataWithItems = new TestInventoryData(
                10, 
                new List<Item>() {new Item(item, 1)}
            );
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            //var containsItem = inventory.ContainsItem(item);
            //Assert.IsTrue(containsItem);
        }
        [Test]
        public void ContainsItem_Should_Fail_IfNotEnoughItemsInInventory() {
            var item = new ItemInfo();
            var baseItems = new List<Item>() {
                new Item(item, 1)
            };

            TestInventoryData testInventoryDataWithItems = new TestInventoryData(10, baseItems);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            //var containsItem = inventory.ContainsItem(item);
            //Assert.IsTrue(containsItem);
            //containsItem = inventory.ContainsItem(item,10);
            //Assert.IsFalse(containsItem);
        }
        [Test]
        public void ContainsItem_Should_BeTrue_IfEnoughItemsInInventory() {
            var item = new ItemInfo();
            var baseItems = new List<Item>() {
                new Item(item, 10)
            };

            TestInventoryData testInventoryDataWithItems = new TestInventoryData(10, baseItems);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            //var containsItem = inventory.ContainsItem(item, 10);
            //Assert.IsTrue(containsItem);
        }
        #endregion
        #region RemoveItem
        [Test]
        public void RemoveItem_Should_RemoveItem() {
            var item = new ItemInfo();
            var baseItems = new List<Item>() {
                new Item(item)
            };

            TestInventoryData testInventoryDataWithItems = new TestInventoryData(10, baseItems);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            Assert.AreEqual(inventory.NumberOfFilledSlots, 1);

            //inventory.RemoveItem(false,item);
            Assert.AreEqual(inventory.NumberOfFilledSlots, 0);
        }
        [Test]
        public void RemoveItem_Should_With_NoParameters_Should_RemoveCurrentItem() {
            var item1 = new ItemInfo();
            var item2 = new ItemInfo();

            var baseItems = new List<Item>() {
                new Item(item1),
                new Item(item2)

            };

            TestInventoryData testInventoryDataWithItems = new TestInventoryData(10, baseItems);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            //Assert.AreEqual(2, inventory.NumberOfFilledSlots);
            //Assert.AreEqual(item1, inventory.CurrentItem.ItemInfo);
            //Assert.AreEqual(1, inventory.CurrentItem.NumberOfItems);

            //inventory.RemoveItem();
            //Assert.AreEqual(1, inventory.NumberOfFilledSlots);
            //Assert.AreEqual(null, inventory.CurrentItem);

        }

        [Test]
        public void RemoveItem_Should_ReduceNumberOfItems() {
            var item = new ItemInfo();
            var baseItems = new List<Item>() {
                new Item(item,10)
            };

            TestInventoryData testInventoryDataWithItems = new TestInventoryData(10, baseItems);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            //Assert.AreEqual(inventory.Content[0].NumberOfItems, 10);

            //inventory.RemoveItem(false, item,5);
            //Assert.AreEqual(inventory.Content[0].NumberOfItems, 5);
        }
        [Test]
        public void RemoveAllItems_Should_RemoveAllItems() {
            var baseItems = new List<Item>() {
                new Item(new ItemInfo(),10),
                new Item(new ItemInfo(),10),
                new Item(new ItemInfo(),10)

            };

            TestInventoryData testInventoryDataWithItems = new TestInventoryData(100, baseItems);
            Inventory inventory = new Inventory(testInventoryDataWithItems);

            Assert.AreEqual(inventory.NumberOfFilledSlots, 3);

            //inventory.RemoveAllItems();

            Assert.AreEqual(inventory.NumberOfFilledSlots, 0);

        }
        #endregion
    }
}

