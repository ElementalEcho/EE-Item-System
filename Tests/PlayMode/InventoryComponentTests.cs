using EE.Core;
using EE.Core.ScriptableObjects;
using EE.ItemSystem.Impl;
using EE.Test.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using static EE.ItemSystem.PlayMode.InventoryDataSOTests;
using static EE.ItemSystem.PlayMode.InventorySOTests;

namespace EE.ItemSystem.PlayMode {
    public class InventoryComponentTests
    {
        internal class TestInventoryComponent : InventoryComponent {
            public void ExposeAwake() {
                Awake();
            }
            public void SetValues(InventorySO inventorySO = null, ItemDataBaseSO itemDataBaseSO = null, InventoryDataSO inventoryDataSO = null, EventActivatorSO eventActivatorSO = null) {
                this.inventoryDataSO = inventoryDataSO;
                this.inventorySO = inventorySO;
                this.itemDataBaseSO = itemDataBaseSO;
                this.eventActivatorSO = eventActivatorSO;
            }
            public InventoryDataSO ExposeInventoryDataSO => inventoryDataSO;

            public InventorySO ExposeInventorySO => inventorySO;

            public ItemDataBaseSO ExposeItemDataBaseSO => itemDataBaseSO;

            public EventActivatorSO ExposeEventActivatorSO => eventActivatorSO;

        }

        private TestInventoryComponent CreateTestInventoryComponent(InventorySO inventorySO = null, ItemDataBaseSO itemDataBaseSO = null, InventoryDataSO inventoryDataSO = null, EventActivatorSO eventActivatorSO = null) {
            GameObject gameObject = new GameObject();
            var physicsComponent = gameObject.AddComponent<TestInventoryComponent>();
            physicsComponent.SetValues(inventorySO, itemDataBaseSO, inventoryDataSO, eventActivatorSO);
            physicsComponent.ExposeAwake();
            return physicsComponent;
        }

        private TestInventoryComponent CreateTestInventoryComponentWithData(int maxInventorySize, List<InspectorItem> baseItems, ItemDataBaseSO itemDataBaseSO = null, EventActivatorSO eventActivatorSO = null) {
            var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
            inventoryDataSO.SetValues(maxInventorySize, baseItems);
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO);
            return CreateTestInventoryComponent(inventorySO, itemDataBaseSO, inventoryDataSO, eventActivatorSO);

        }

        [UnityTest]
        public IEnumerator InventoryComponent_Should_Init_Correctly() {
            var inventoryComponent = CreateTestInventoryComponent();
            inventoryComponent.ExposeInventoryDataSO.Should().BeNull();
            inventoryComponent.ExposeInventorySO.Should().BeNull();
            inventoryComponent.ExposeItemDataBaseSO.Should().BeNull();
            inventoryComponent.ExposeEventActivatorSO.Should().BeNull();

            inventoryComponent.CurrentItem.Should().BeNull();
            inventoryComponent.CurrentIndex.Should().Be(0);
            inventoryComponent.IsFull.Should().BeFalse();
            inventoryComponent.NumberOfFilledSlots.Should().Be(0);
            yield return null;
        }

        [UnityTest]
        public IEnumerator InventoryComponent_Should_Init_Correctly_With_Data() {
            var itemType1 = ScriptableObject.CreateInstance<ItemTypeSO>();
            var itemType2 = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 3),
                new InspectorItem(itemType2, 3)
            };
            var itemDataBaseSO = ScriptableObject.CreateInstance<ItemDataBaseSO>();

            var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
            inventoryDataSO.SetValues(5, baseItems);
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO);

            var inventoryComponent = CreateTestInventoryComponent(inventorySO, itemDataBaseSO, inventoryDataSO, null);
            inventoryComponent.ExposeInventoryDataSO.Should().Be(inventoryDataSO);
            inventoryComponent.ExposeInventoryDataSO.MaxSize.Should().Be(5);
            inventoryComponent.ExposeInventoryDataSO.BaseItems.Count.Should().Be(2);

            inventoryComponent.ExposeInventorySO.Should().Be(inventorySO);
            inventoryComponent.ExposeItemDataBaseSO.Should().Be(itemDataBaseSO);
            inventoryComponent.ExposeEventActivatorSO.Should().BeNull();

            inventoryComponent.CurrentItem.Should().BeNotNull();
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(3);


            inventoryComponent.CurrentIndex.Should().Be(0);
            inventoryComponent.IsFull.Should().BeFalse();
            inventoryComponent.NumberOfFilledSlots.Should().Be(2);
            yield return null;
        }

        [UnityTest]
        public IEnumerator NextItem_Should_Increase_CurrentIndex() {
            var itemType1 = ScriptableObject.CreateInstance<ItemTypeSO>();
            var itemType2 = ScriptableObject.CreateInstance<ItemTypeSO>();
            var itemType3 = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 4),
                new InspectorItem(itemType2, 3),
                new InspectorItem(itemType3, 2)

            };
            var itemDataBaseSO = ScriptableObject.CreateInstance<ItemDataBaseSO>();

            var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
            inventoryDataSO.SetValues(3, baseItems);
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO);
            var inventoryComponent = CreateTestInventoryComponent(inventorySO, itemDataBaseSO, inventoryDataSO, null);

            inventoryComponent.CurrentIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(4);

            inventoryComponent.IncreaseIndex();
            inventoryComponent.CurrentIndex.Should().Be(1);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType2);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(3);

            inventoryComponent.IncreaseIndex();
            inventoryComponent.CurrentIndex.Should().Be(2);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType3);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(2);

            inventoryComponent.IncreaseIndex();
            inventoryComponent.CurrentIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(4);
            yield return null;

        }

        [UnityTest]
        public IEnumerator PreviousItem_Should_Decrease_CurrentIndex() {
            var itemType1 = ScriptableObject.CreateInstance<ItemTypeSO>();
            var itemType2 = ScriptableObject.CreateInstance<ItemTypeSO>();
            var itemType3 = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 4),
                new InspectorItem(itemType2, 3),
                new InspectorItem(itemType3, 2)

            };
            var itemDataBaseSO = ScriptableObject.CreateInstance<ItemDataBaseSO>();

            var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
            inventoryDataSO.SetValues(3, baseItems);
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO);
            var inventoryComponent = CreateTestInventoryComponent(inventorySO, itemDataBaseSO, inventoryDataSO, null);

            inventoryComponent.CurrentIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(4);

            inventoryComponent.DecreaseIndex();
            inventoryComponent.CurrentIndex.Should().Be(2);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType3);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(2);

            inventoryComponent.DecreaseIndex();
            inventoryComponent.CurrentIndex.Should().Be(1);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType2);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(3);

            inventoryComponent.DecreaseIndex();
            inventoryComponent.CurrentIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(4);
            yield return null;

        }

        [UnityTest]
        public IEnumerator ChangeItem_Should_Change_Index() {
            var itemType1 = ScriptableObject.CreateInstance<ItemTypeSO>();
            var itemType2 = ScriptableObject.CreateInstance<ItemTypeSO>();
            var itemType3 = ScriptableObject.CreateInstance<ItemTypeSO>();

            var baseItems = new List<InspectorItem>() {
                new InspectorItem(itemType1, 4),
                new InspectorItem(itemType2, 3),
                new InspectorItem(itemType3, 2)

            };
            var itemDataBaseSO = ScriptableObject.CreateInstance<ItemDataBaseSO>();

            var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
            inventoryDataSO.SetValues(10, baseItems);
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO);
            var inventoryComponent = CreateTestInventoryComponent(inventorySO, itemDataBaseSO, inventoryDataSO, null);

            inventoryComponent.CurrentIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(4);

            inventoryComponent.ChangeIndex(2);

            inventoryComponent.CurrentIndex.Should().Be(2);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType3);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(2);

            inventoryComponent.ChangeIndex(8);

            inventoryComponent.CurrentIndex.Should().Be(8);
            inventoryComponent.CurrentItem.Should().BeNull();
            yield return null;
        }
        [UnityTest]
        public IEnumerator TODO_EventActivatorTEsts() {
            var inventoryComponent = CreateTestInventoryComponent();

            inventoryComponent.ExposeEventActivatorSO.Should().BeNotNull();

            yield return null;
        }
    }
}
