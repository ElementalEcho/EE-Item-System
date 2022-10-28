using EE.Core;
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
            public void SetValues(InventorySO inventorySO = null, ItemDataBaseSO itemDataBaseSO = null, InventoryDataSO inventoryDataSO = null,  GenericActionSO[] itemAddedActions = null) {
                this.inventoryDataSO = inventoryDataSO;
                this.inventorySO = inventorySO;
                this.itemDataBaseSO = itemDataBaseSO;
                this.itemAddedActions = itemAddedActions != null ? itemAddedActions : new GenericActionSO[0];
            }
            public InventoryDataSO ExposeInventoryDataSO => inventoryDataSO;

            public InventorySO ExposeInventorySO => inventorySO;

            public ItemDataBaseSO ExposeItemDataBaseSO => itemDataBaseSO;

            public ItemDropInfoContainer ExposeItemDropInfoContainer => itemDropInfoContainer;
            public GenericActionSO[] ExposeItemAddedActions => itemAddedActions;

            public IFacingDirection ExposeItemDropOffPoint => itemDropOffPoint;

        }

        private TestInventoryComponent CreateTestInventoryComponent(InventorySO inventorySO = null, ItemDataBaseSO itemDataBaseSO = null, InventoryDataSO inventoryDataSO = null, GenericActionSO[] itemAddedActions = null) {
            GameObject gameObject = new GameObject();
            var physicsComponent = gameObject.AddComponent<TestInventoryComponent>();
            physicsComponent.SetValues(inventorySO, itemDataBaseSO, inventoryDataSO, itemAddedActions);
            physicsComponent.ExposeAwake();
            return physicsComponent;
        }

        private TestInventoryComponent CreateTestInventoryComponentWithData(int maxInventorySize, List<InspectorItem> baseItems, ItemDataBaseSO itemDataBaseSO = null, GenericActionSO[] itemAddedActions = null) {
            var inventoryDataSO = ScriptableObject.CreateInstance<TestInventoryDataSO>();
            inventoryDataSO.SetValues(maxInventorySize, baseItems);
            var inventorySO = ScriptableObject.CreateInstance<TestInventorySO>();
            inventorySO.SetValues(inventoryDataSO);
            return CreateTestInventoryComponent(inventorySO, itemDataBaseSO, inventoryDataSO, itemAddedActions);

        }

        [UnityTest]
        public IEnumerator InventoryComponent_Should_Init_Correctly() {
            var inventoryComponent = CreateTestInventoryComponent();
            inventoryComponent.ExposeInventoryDataSO.Should().BeNull();
            inventoryComponent.ExposeInventorySO.Should().BeNull();
            inventoryComponent.ExposeItemDataBaseSO.Should().BeNull();
            inventoryComponent.ExposeItemAddedActions.Length.Should().Be(0);

            inventoryComponent.CurrentItem.Should().BeNull();
            inventoryComponent.CurrentItemIndex.Should().Be(0);
            inventoryComponent.IsFull.Should().BeFalse();
            inventoryComponent.NumberOfFilledSlots.Should().Be(0);

            inventoryComponent.ExposeItemDropOffPoint.Should().BeNotNull();


            var itemDropOffData = inventoryComponent.ExposeItemDropInfoContainer;

            itemDropOffData.dropForce.Should().Be(10);
            itemDropOffData.dropRange.Should().Be(3);
            itemDropOffData.dropRotationSpeed.Should().Be(3);
            itemDropOffData.dropRotationAngle.Should().Be(90);
            itemDropOffData.arcHight.Should().Be(90);
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
            inventoryComponent.ExposeInventoryDataSO.InventoryData.MaxInventorySize.Should().Be(5);
            inventoryComponent.ExposeInventoryDataSO.InventoryData.BaseItems.Count.Should().Be(2);

            inventoryComponent.ExposeInventorySO.Should().Be(inventorySO);
            inventoryComponent.ExposeItemDataBaseSO.Should().Be(itemDataBaseSO);
            inventoryComponent.ExposeItemAddedActions.Length.Should().Be(0);

            inventoryComponent.CurrentItem.Should().BeNotNull();
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(3);


            inventoryComponent.CurrentItemIndex.Should().Be(0);
            inventoryComponent.IsFull.Should().BeFalse();
            inventoryComponent.NumberOfFilledSlots.Should().Be(2);

            inventoryComponent.ExposeItemDropOffPoint.Should().BeNotNull();

            var itemDropOffData = inventoryComponent.ExposeItemDropInfoContainer;

            itemDropOffData.dropForce.Should().Be(10);
            itemDropOffData.dropRange.Should().Be(3);
            itemDropOffData.dropRotationSpeed.Should().Be(3);
            itemDropOffData.dropRotationAngle.Should().Be(90);
            itemDropOffData.arcHight.Should().Be(90);
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

            inventoryComponent.CurrentItemIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(4);

            inventoryComponent.NextItem();
            inventoryComponent.CurrentItemIndex.Should().Be(1);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType2.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(3);

            inventoryComponent.NextItem();
            inventoryComponent.CurrentItemIndex.Should().Be(2);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType3.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(2);

            inventoryComponent.NextItem();
            inventoryComponent.CurrentItemIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1.ItemType);
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

            inventoryComponent.CurrentItemIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(4);

            inventoryComponent.PreviousItem();
            inventoryComponent.CurrentItemIndex.Should().Be(2);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType3.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(2);

            inventoryComponent.PreviousItem();
            inventoryComponent.CurrentItemIndex.Should().Be(1);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType2.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(3);

            inventoryComponent.PreviousItem();
            inventoryComponent.CurrentItemIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1.ItemType);
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

            inventoryComponent.CurrentItemIndex.Should().Be(0);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType1.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(4);

            inventoryComponent.ChangeItem(2);

            inventoryComponent.CurrentItemIndex.Should().Be(2);
            inventoryComponent.CurrentItem.ItemInfo.Should().Be(itemType3.ItemType);
            inventoryComponent.CurrentItem.NumberOfItems.Should().Be(2);

            inventoryComponent.ChangeItem(8);

            inventoryComponent.CurrentItemIndex.Should().Be(8);
            inventoryComponent.CurrentItem.Should().BeNull();
            yield return null;
        }

    }
}
