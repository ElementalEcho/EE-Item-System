using EE.Core;
using EE.Core.PoolingSystem;
using EE.ItemSystem;
using EE.ItemSystem.Crafting;
using EE.ItemSystem.Crafting.Impl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
namespace Tests {

    public class RecipeTests {        
        private class TestItemInfo : IItemInfo {
            public PoolableComponent ItemToDrop => null;
            public int ManaToGive => 100;

            public int MaxItemStack => 5;

            public string PrefabGuid => throw new NotImplementedException();

            public GenericActionSO[] StartItemUseEffects => throw new NotImplementedException();

            public GenericActionSO[] AttackItemUseEffects => throw new NotImplementedException();

            public GenericActionSO[] ThrownItemEffects => throw new NotImplementedException();
        }
        private class TestHasRecipe : IHasRecipe {
            public TestHasRecipe(Recipe recipe) {
                this.recipe = recipe;
            }
            private Recipe recipe;
            public IRecipe CraftingRecipe => recipe;
        }

        TestItemInfo testItemType1;
        TestItemInfo testItemType2;
        TestItemInfo testItemType3;

        [SetUp]
        public void Init() {
            testItemType1 = new TestItemInfo();
            testItemType2 = new TestItemInfo();
            testItemType3 = new TestItemInfo();
        }
        [Test]
        public void CanCraftItem_Should_Return_True_If_All_Items_Are_In_TheList() {
            Item[] requirements = new Item[] {
                new Item(testItemType1, 3),
                new Item(testItemType2, 4)
            };
            List<Item> availableItems = new List<Item>() {
                new Item(testItemType1, 3),
                new Item(testItemType2, 4)
            };
            Recipe recipe = new Recipe(requirements, new Item(testItemType1, 1));

            Assert.IsTrue(recipe.CanCraftItem(availableItems));
        }
        [Test]
        public void CanCraftItem_Should_Return_True_If_There_Is_More_Than_RequiredItems() {
            Item itemOne = new Item(testItemType1, 3);

            Item[] requirements = new Item[] {
                itemOne,
                new Item(testItemType2, 2)
            };
            List<Item> availableItems = new List<Item>() {
                itemOne,
                new Item(testItemType2, 4)
            };

            Recipe recipe = new Recipe(requirements, new Item(testItemType1, 1));

            Assert.IsTrue(recipe.CanCraftItem(availableItems));
        }
        [Test]
        public void CanCraftItem_Should_Return_False_If_ItemTypes_Are_Missing() {
            Item itemOne = new Item(testItemType1, 4);

            Item[] requirements = new Item[] {
                itemOne,
                new Item(testItemType2, 1)
            };
            List<Item> availableItems = new List<Item>() {
                itemOne,
                new Item(testItemType3, 1)
            };

            Recipe recipe = new Recipe(requirements, new Item(testItemType1, 1));

            Assert.IsFalse(recipe.CanCraftItem(availableItems));
        }
        [Test]
        public void CanCraftItem_Should_Return_False_If_NotEnough_Items() {
            Item itemTypeOne_With4 = new Item(testItemType1, 4);
            Item itemTypeOne_With1 = new Item(testItemType1, 1);
            Item itemThree = new Item(testItemType2, 1);

            Item[] requirements = new Item[] {
                itemTypeOne_With4,
                itemThree
            };
            List<Item> availableItems = new List<Item>() {
                itemTypeOne_With1,
                itemThree
            };

            Recipe recipe = new Recipe(requirements, new Item(testItemType1, 1));

            Assert.IsFalse(recipe.CanCraftItem(availableItems));
        }

        [Test]
        public void Craft_Should_Return_Item_If_Enough_Items() {
            Item itemOne = new Item(testItemType1, 4);
            Item result = new Item(testItemType1, 1);

            Item[] requirements = new Item[] {
                itemOne,
                new Item(testItemType2, 1)
            };
            List<Item> availableItems = new List<Item>() {
                itemOne,
                new Item(testItemType2, 1)
            };

            Recipe recipe = new Recipe(requirements, result);

            Assert.IsTrue(recipe.Craft(availableItems, out var craftResult));

            Assert.AreEqual(result, craftResult);
            Assert.AreEqual(result.ItemInfo, testItemType1);
            Assert.AreEqual(result.NumberOfItems, 1);
            Assert.AreEqual(availableItems.Count, 0);

        }
        [Test]
        public void Craft_Should_Return_Null_If_Items_Are_Missing() {
            Item itemOne = new Item(testItemType1, 4);
            Item result = new Item(testItemType1, 1);

            Item[] requirements = new Item[] {
                itemOne,
                new Item(testItemType2, 1)
            };
            List<Item> availableItems = new List<Item>() {
                itemOne,
            };

            Recipe recipe = new Recipe(requirements, result);

            Assert.IsFalse(recipe.Craft(availableItems, out var craftResult));

            Assert.IsNull(craftResult);
            Assert.AreEqual(1,availableItems.Count);
            Assert.AreEqual(itemOne, availableItems[0]);


        }
        [Test]
        public void Craft_Should_Return_Null_If_NumberOfItems_Is_Less_Than_Required() {
            Item itemOne = new Item(testItemType1, 4);
            Item itemTwo = new Item(testItemType2, 1);

            Item result = new Item(testItemType1, 1);

            Item[] requirements = new Item[] {
                itemOne,
                new Item(testItemType2, 4)
            };
            List<Item> availableItems = new List<Item>() {
                itemOne,
                itemTwo
            };

            Recipe recipe = new Recipe(requirements, result);

            Assert.IsFalse(recipe.Craft(availableItems, out var craftResult));

            Assert.IsNull(craftResult);
            Assert.AreEqual(2, availableItems.Count);
            Assert.AreEqual(testItemType2, availableItems[0].ItemInfo);
            Assert.AreEqual(1, availableItems[0].NumberOfItems);
            Assert.AreEqual(testItemType1, availableItems[1].ItemInfo);
            Assert.AreEqual(4, availableItems[1].NumberOfItems);
        }
        [Test]
        public void Craft_Should_Reduce_NumberOfItems_It_AvailablesItems_Is_More_Than_Required() {
            Item itemOne = new Item(testItemType1, 4);

            Item result = new Item(testItemType1, 1);

            Item[] requirements = new Item[] {
                itemOne,
                new Item(testItemType2, 3)
            };
            List<Item> availableItems = new List<Item>() {
                itemOne,
                new Item(testItemType2, 5),
                new Item(testItemType3, 1)
            };

            Recipe recipe = new Recipe(requirements, result);

            Assert.IsTrue(recipe.Craft(availableItems, out var craftResult));

            Assert.AreEqual(result,craftResult);
            Assert.AreEqual(2, availableItems.Count);
            Assert.AreEqual(testItemType3, availableItems[0].ItemInfo);
            Assert.AreEqual(1, availableItems[0].NumberOfItems);
            Assert.AreEqual(testItemType2, availableItems[1].ItemInfo);
            Assert.AreEqual(2, availableItems[1].NumberOfItems);
        }
        [Test]
        public void Recipies_Should_Return_All_Recipies() {
            Recipe recipe1 = new Recipe(new Item[0],new Item(testItemType1, 1));
            TestHasRecipe testHasRecipe1 = new TestHasRecipe(recipe1);

            Recipe recipe2 = new Recipe(new Item[0], new Item(testItemType1, 1));
            TestHasRecipe testHasRecipe2 = new TestHasRecipe(recipe2);

            List<IHasRecipe> recipes = new List<IHasRecipe>() {
                testHasRecipe1,
                testHasRecipe2
            };

            CraftingData craftingData = new CraftingData(recipes);

            Assert.AreEqual(2,craftingData.Recipes.Count);
            Assert.AreEqual(testHasRecipe1.CraftingRecipe, craftingData.Recipes[0]);
            Assert.AreEqual(testHasRecipe2.CraftingRecipe, craftingData.Recipes[1]);

        }
    }
}
