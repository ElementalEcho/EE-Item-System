using EE.Core;
using EE.Core.PoolingSystem;
using EE.InventorySystem;
using EE.InventorySystem.Crafting;
using EE.InventorySystem.Crafting.Impl;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Tests {

    public class CrafterTests {
        private class TestItemInfo : IItemInfo {
            public PoolableComponent ItemToDrop => null;

            public int ManaToGive => 100;

            public int MaxItemStack => 5;

            public string PrefabGuid => throw new System.NotImplementedException();

            public GenericActionSO[] StartItemUseEffects => throw new System.NotImplementedException();

            public GenericActionSO[] AttackItemUseEffects => throw new System.NotImplementedException();

            public GenericActionSO[] ThrownItemEffects => throw new System.NotImplementedException();
        }
        private class TestCraftingData : ICraftingData {
            public TestCraftingData(List<IRecipe> recipes) {
                this.recipes = recipes;
            }
            private List<IRecipe> recipes = new List<IRecipe>();
            public List<IRecipe> Recipes => recipes;
        }
        TestItemInfo testItemType1;
        TestItemInfo testItemType2;
        Crafter crafter;

        [SetUp]
        public void Init() {
            testItemType1 = new TestItemInfo();
            testItemType2 = new TestItemInfo();

            Item[] requirements = new Item[] {
                    new Item(testItemType1, 3),
                    new Item(testItemType2, 4)
                };
            Recipe recipe1 = new Recipe(requirements, new Item(testItemType1, 3));
            Recipe recipe2 = new Recipe(requirements, new Item(testItemType2, 2));

            List<IRecipe> recipes = new List<IRecipe> {
                recipe1,
                recipe2
            };

            TestCraftingData craftingData = new TestCraftingData(recipes);
            crafter = new Crafter(craftingData);
        }

        [Test]
        public void Crafter_Should_CreateItem_If_Enough_Materials() {
            List<Item> availableItems = new List<Item>() {
                new Item(testItemType1, 3),
                new Item(testItemType2, 4)
            };
            Assert.IsTrue(crafter.Craft(availableItems,0, out Item item));

            Assert.AreEqual(0, availableItems.Count);
            Assert.AreEqual(testItemType1, item.ItemInfo);
            Assert.AreEqual(3, item.NumberOfItems);
        }

        [Test]
        public void Crafter_Should_CreateItem_Based_On_The_Index() {
            List<Item> availableItems = new List<Item>() {
                new Item(testItemType1, 3),
                new Item(testItemType2, 4)
            };
            Assert.IsTrue(crafter.Craft(availableItems, 1, out Item item));

            Assert.AreEqual(0, availableItems.Count);
            Assert.AreEqual(testItemType2, item.ItemInfo);
            Assert.AreEqual(2, item.NumberOfItems);

        }

        [Test]
        public void Crafting_Should_Fail_IfNot_Enough_Items() {
            Assert.IsFalse(crafter.Craft(new List<Item>(), 0, out Item item));
            Assert.IsNull(item);
        }

        [Test]
        public void Craft_Should_Not_Throw_Error_If_OutOfRange() {
            List<Item> availableItems = new List<Item>() {
                new Item(testItemType1, 3),
            };
            Assert.IsFalse(crafter.Craft(availableItems, 10, out Item item));
            Assert.IsNull(item);
        }

    }
}