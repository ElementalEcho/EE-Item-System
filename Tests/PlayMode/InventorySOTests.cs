using EE.ItemSystem.Impl;
using EE.Test.Util;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using static EE.ItemSystem.PlayMode.InventoryDataSOTests;
using static EE.ItemSystem.PlayMode.ItemTypeSOTests;

namespace EE.ItemSystem.PlayMode {
    public class InventorySOTests {
        internal class TestInventorySO : InventorySO {
            public void SetValues(InventoryDataSO inventoryDataSO) {
                this.inventoryDataSO = inventoryDataSO;
            }
            public static TestInventorySO CreateInventoryWithData(int maxInventorySize, List<InspectorItem> baseItems) {
                var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
                inventoryDataSO.SetValues(maxInventorySize, baseItems);
                var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
                inventorySO.SetValues(inventoryDataSO);
                return inventorySO;

            }
        }

        [UnityTest]
        public IEnumerator InventorySO_Init_Should_Fail_If_No_InventoryData() {
            var inventorySO = ScriptableObject.CreateInstance<InventorySO>();
            try {
                var inventory = inventorySO.Size;
                Assert.Fail();
            }
            catch (System.NullReferenceException) {

            }
            yield return null;
        }
        [UnityTest]
        public IEnumerator InventorySO_Init_Should_Be_Correct_If_InventoryData() {
            var inventoryDataSO = ScriptableObject.CreateInstance<InventoryDataSO>();
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO);

            Assert.AreEqual(5, inventorySO.Size);
            Assert.AreEqual(0, inventorySO.NumberOfFilledSlots);
            Assert.IsFalse(inventorySO.IsFull);
            yield return null;
        }

        [UnityTest]
        public IEnumerator InventorySO_Add_Should_Add() {
            var inventoryDataSO = ScriptableObject.CreateInstance<InventoryDataSO>();
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO);

            Assert.AreEqual(0, inventorySO.NumberOfFilledSlots);

            inventorySO.Add(new Item(new ItemInfo(),4));

            Assert.AreEqual(1, inventorySO.NumberOfFilledSlots);
            yield return null;
        }
        #region Getters
        [UnityTest]
        public IEnumerator ArrayInventory_Should_Be_Full() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 3),
                new InspectorItem(itemType, 3)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(2, baseItems);

            inventory.IsFull.Should().BeTrue();
            yield return null;
        }
    
        [UnityTest]
        public IEnumerator ArrayInventory_Should_Not_Be_Full() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 3),
                new InspectorItem(itemType, 3)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(3, baseItems);

            inventory.IsFull.Should().BeFalse();
            yield return null;
        }
        [UnityTest]
        public IEnumerator NumberOfFilledSlots_Should_Be_2() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 3),
                new InspectorItem(itemType, 3)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(3, baseItems);

            inventory.NumberOfFilledSlots.Should().Be(2);
            yield return null;
        }
        [UnityTest]
        public IEnumerator GetItems_Should_Return_AllItems() {
            var itemType1 = ScriptableObject.CreateInstance<ItemTypeSO>();
            var itemType2 = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 3),
                new InspectorItem(itemType2, 3)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(3, baseItems);

            var items = inventory.GetItems();

            items.Count.Should().Be(2);
            items[0].ItemInfo.Should().Be(itemType1.ItemType);
            items[1].ItemInfo.Should().Be(itemType2.ItemType);

            yield return null;
        }

        #endregion

        #region Remove

        [UnityTest]
        public IEnumerator RemoveAllItems_Should_Remove_Everything() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 3),
                new InspectorItem(itemType, 3)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(3, baseItems);

            inventory.NumberOfFilledSlots.Should().Be(2);

            inventory.RemoveAll();

            inventory.NumberOfFilledSlots.Should().Be(0);
            yield return null;
        }
        [UnityTest]
        public IEnumerator Remove_Should_Remove_Item() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 3),
                new InspectorItem(itemType, 3)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(3, baseItems);

            inventory.NumberOfFilledSlots.Should().Be(2);

            inventory.Remove(itemType.ItemType, 5);

            inventory.NumberOfFilledSlots.Should().Be(1);

            inventory.Get(0).Should().BeNull();
            inventory.Get(1).Should().BeNotNull();
            yield return null;
        }
        [UnityTest]
        public IEnumerator Remove_Should_Remove_Stacks_From_Item() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(3, baseItems);

            inventory.NumberOfFilledSlots.Should().Be(2);

            inventory.Remove(itemType.ItemType, 3);

            inventory.NumberOfFilledSlots.Should().Be(2);

            var itemFromInventory = inventory.Get(0);
            itemFromInventory.Should().BeNotNull();
            itemFromInventory.ItemInfo.Should().Be(itemType.ItemType);
            itemFromInventory.NumberOfItems.Should().Be(2);

            inventory.Get(1).Should().BeNotNull();
            yield return null;
        }
        [UnityTest]
        public IEnumerator Remove_Should_Remove_Stacks_From_Multiple_Items() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            inventory.NumberOfFilledSlots.Should().Be(2);

            inventory.Remove(itemType.ItemType, 7);

            inventory.NumberOfFilledSlots.Should().Be(1);

            inventory.Get(0).Should().BeNull();

            var itemFromInventory = inventory.Get(1);
            itemFromInventory.Should().BeNotNull();
            itemFromInventory.ItemInfo.Should().Be(itemType.ItemType);
            itemFromInventory.NumberOfItems.Should().Be(3);

            yield return null;
        }

        #endregion
        #region Contains

        [UnityTest]
        public IEnumerator ContainsItem_Should_Return_True_If_ItemType_Is_In_Inventory() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 5),
            };
            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            var retval = inventory.Contains(itemType.ItemType, 5);

            retval.Should().BeTrue();
            yield return null;
        }
        [UnityTest]
        public IEnumerator ContainsItem_Should_Return_True_If_ItemType_Is_In_Inventory_But_In_Separete_Stacts() {
            var itemType = TestItemTypeSO.CreateItemTypeSO(10);
            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 3),
                new InspectorItem(itemType, 9)
            };
            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);


            var retval = inventory.Contains(itemType.ItemType, 10);

            retval.Should().BeTrue();
            yield return null;
        }

        [UnityTest]
        public IEnumerator ContainsItem_Should_Be_False_If_ItemType_Is_Not_In_Inventory() {
            var itemType = TestItemTypeSO.CreateItemTypeSO(10);
            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 3),
                new InspectorItem(itemType, 9)
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            var retval = inventory.Contains(new ItemInfo(), 3);

            retval.Should().BeFalse();
            yield return null;
        }

        [UnityTest]
        public IEnumerator ContainsItem_Be_False_False_If_Not_Enough_Items() {
            var itemType = ScriptableObject.CreateInstance<ItemTypeSO>();
            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 5),
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            var retval = inventory.Contains(itemType.ItemType, 11);

            retval.Should().BeFalse();
            yield return null;
        }
        #endregion

        #region Adding
        [UnityTest]
        public IEnumerator AddItem_Should_Add_The_Item() {
            var inventory = TestInventorySO.CreateInventoryWithData(5, new List<InspectorItem>());

            var itemInfo = new ItemInfo();
            var item = new Item(itemInfo, 5);

            inventory.NumberOfFilledSlots.Should().Be(0);

            var retval = inventory.Add(item);
            retval.Should().BeTrue();
            inventory.NumberOfFilledSlots.Should().Be(1);

            var itemFromInventory = inventory.Get(0);
            itemFromInventory.Should().BeNotNull();
            itemFromInventory.ItemInfo.Should().Be(itemInfo);
            itemFromInventory.NumberOfItems.Should().Be(5);
            yield return null;
        }
        [UnityTest]
        public IEnumerator AddItem_Should_Increase_The_Number_Of_Stacks() {
            var itemType1 = TestItemTypeSO.CreateItemTypeSO(10);
            var itemType2 = TestItemTypeSO.CreateItemTypeSO(10);
            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 3),
                new InspectorItem(itemType2, 9)
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            inventory.NumberOfFilledSlots.Should().Be(2);

            var retval = inventory.Add(new Item(itemType1.ItemType, 5));

            retval.Should().BeTrue();
            inventory.NumberOfFilledSlots.Should().Be(2);
            var itemFromInventory = inventory.Get(0);
            itemFromInventory.Should().BeNotNull();
            itemFromInventory.ItemInfo.Should().Be(itemType1.ItemType);
            itemFromInventory.NumberOfItems.Should().Be(8);

            itemFromInventory = inventory.Get(1);
            itemFromInventory.Should().BeNotNull();
            itemFromInventory.ItemInfo.Should().Be(itemType2.ItemType);
            itemFromInventory.NumberOfItems.Should().Be(9);
            yield return null;
        }
        [UnityTest]
        public IEnumerator AddItem_Should_Increase_The_Number_Of_Stacks_And_Add_To_Empty_Slot() {
            var itemType1 = TestItemTypeSO.CreateItemTypeSO(10);
            var itemType2 = TestItemTypeSO.CreateItemTypeSO(10);
            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 3),
                new InspectorItem(itemType2, 9)
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);


            inventory.NumberOfFilledSlots.Should().Be(2);

            var retval = inventory.Add(new Item(itemType1.ItemType, 10));

            retval.Should().BeTrue();
            inventory.NumberOfFilledSlots.Should().Be(3);
            var itemFromInventory = inventory.Get(0);
            itemFromInventory.Should().BeNotNull();
            itemFromInventory.ItemInfo.Should().Be(itemType1.ItemType);
            itemFromInventory.NumberOfItems.Should().Be(10);

            itemFromInventory = inventory.Get(1);
            itemFromInventory.Should().BeNotNull();
            itemFromInventory.ItemInfo.Should().Be(itemType2.ItemType);
            itemFromInventory.NumberOfItems.Should().Be(9);

            itemFromInventory = inventory.Get(2);
            itemFromInventory.Should().BeNotNull();
            itemFromInventory.ItemInfo.Should().Be(itemType1.ItemType);
            itemFromInventory.NumberOfItems.Should().Be(3);
            yield return null;
        }
        [UnityTest]
        public IEnumerator AddItem_Should_Fail_If_Inventory_Is_Full() {
            var itemType1 = TestItemTypeSO.CreateItemTypeSO(5);
            var itemType2 = TestItemTypeSO.CreateItemTypeSO(5);
            var itemType3 = TestItemTypeSO.CreateItemTypeSO(6);
            var itemType4 = TestItemTypeSO.CreateItemTypeSO(7);
            var itemType5 = TestItemTypeSO.CreateItemTypeSO(4);

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 3),
                new InspectorItem(itemType2, 3),
                new InspectorItem(itemType3, 3),
                new InspectorItem(itemType4, 3),
                new InspectorItem(itemType5, 3),
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);


            var retval = inventory.Add(new Item(new ItemInfo(), 15));

            retval.Should().BeFalse();
            yield return null;
        }
        [UnityTest]
        public IEnumerator AddItem_Should_Fail_If_Inventory_Is_Full_And_Stacks_Are_Full() {
            var itemType = TestItemTypeSO.CreateItemTypeSO(5);
            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5),
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);


            var retval = inventory.Add(new Item(itemType.ItemType, 15));

            retval.Should().BeFalse();
            yield return null;
        }
        [UnityTest]
        public IEnumerator AddItem_Should_Fail_If_Inventory_Is_Full_And_Item_Cannot_Be_Fully_Added() {
            var itemType = TestItemTypeSO.CreateItemTypeSO(5);
            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 5),
                new InspectorItem(itemType, 3),
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            inventory.Get(4).NumberOfItems.Should().Be(3);

            var retval = inventory.Add(new Item(itemType.ItemType, 15));

            retval.Should().BeFalse();

            inventory.Get(4).NumberOfItems.Should().Be(5);
            yield return null;
        }

        #endregion

        #region Replace

        [UnityTest]
        public IEnumerator Replace_Should_Replace_Item_InSlot() {
            var itemInfo1 = TestItemTypeSO.CreateItemTypeSO(5);
            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemInfo1, 5),
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            var item = inventory.Get(0);
            item.ItemInfo.Should().Be(itemInfo1.ItemType);
            item.NumberOfItems.Should().Be(5);

            var itemInfo2 = new ItemInfo();
            inventory.Replace(0, new Item(itemInfo2, 3));

            item = inventory.Get(0);
            item.ItemInfo.Should().Be(itemInfo2);
            item.NumberOfItems.Should().Be(3);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SwitchItemPosition_Should_Change_Item_Positions() {
            var itemType1 = TestItemTypeSO.CreateItemTypeSO(5);
            var itemType2 = TestItemTypeSO.CreateItemTypeSO(5);
            var itemType3 = TestItemTypeSO.CreateItemTypeSO(5);

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 5),
                new InspectorItem(itemType2, 5),
                new InspectorItem(itemType3, 5),
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            inventory.Get(0).ItemInfo.Should().Be(itemType1.ItemType);
            inventory.Get(1).ItemInfo.Should().Be(itemType2.ItemType);
            inventory.Get(2).ItemInfo.Should().Be(itemType3.ItemType);

            TestInventorySO.SwitchItemPosition(0,2, inventory);

            inventory.Get(0).ItemInfo.Should().Be(itemType3.ItemType);
            inventory.Get(1).ItemInfo.Should().Be(itemType2.ItemType);
            inventory.Get(2).ItemInfo.Should().Be(itemType1.ItemType);

            yield return null;
        }
        [UnityTest]
        public IEnumerator SwitchItemPosition_Should_Set_Other_Slot_To_Null_If_No_Item_In_That_Slot() {
            var itemType1 = TestItemTypeSO.CreateItemTypeSO(5);
            var itemType2 = TestItemTypeSO.CreateItemTypeSO(5);
            var itemType3 = TestItemTypeSO.CreateItemTypeSO(5);

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 5),
                new InspectorItem(itemType2, 5),
                new InspectorItem(itemType3, 5),
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            inventory.Get(0).ItemInfo.Should().Be(itemType1.ItemType);
            inventory.Get(1).ItemInfo.Should().Be(itemType2.ItemType);
            inventory.Get(2).ItemInfo.Should().Be(itemType3.ItemType);
            inventory.Get(3).Should().BeNull();
            inventory.Get(4).Should().BeNull();

            TestInventorySO.SwitchItemPosition(1, 4, inventory);

            inventory.Get(0).ItemInfo.Should().Be(itemType1.ItemType);
            inventory.Get(1).Should().BeNull();
            inventory.Get(2).ItemInfo.Should().Be(itemType3.ItemType);
            inventory.Get(3).Should().BeNull();
            inventory.Get(4).ItemInfo.Should().Be(itemType2.ItemType);
            yield return null;
        }
        [UnityTest]
        public IEnumerator SwitchItemPosition_Should_Set_First_Slot_To_Null_If_No_Item_In_That_Slot() {
            var itemType1 = TestItemTypeSO.CreateItemTypeSO(5);
            var itemType2 = TestItemTypeSO.CreateItemTypeSO(5);
            var itemType3 = TestItemTypeSO.CreateItemTypeSO(5);

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 5),
                new InspectorItem(itemType2, 5),
                new InspectorItem(itemType3, 5),
            };

            var inventory = TestInventorySO.CreateInventoryWithData(5, baseItems);

            inventory.Get(0).ItemInfo.Should().Be(itemType1.ItemType);
            inventory.Get(1).ItemInfo.Should().Be(itemType2.ItemType);
            inventory.Get(2).ItemInfo.Should().Be(itemType3.ItemType);
            inventory.Get(3).Should().BeNull();
            inventory.Get(4).Should().BeNull();

            TestInventorySO.SwitchItemPosition(4, 2, inventory);

            inventory.Get(0).ItemInfo.Should().Be(itemType1.ItemType);
            inventory.Get(1).ItemInfo.Should().Be(itemType2.ItemType);
            inventory.Get(2).Should().BeNull();
            inventory.Get(3).Should().BeNull();
            inventory.Get(4).ItemInfo.Should().Be(itemType3.ItemType);
            yield return null;
        }

        #endregion
    }
}
