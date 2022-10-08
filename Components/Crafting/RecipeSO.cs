using EE.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EE.InventorySystem.Crafting {
    public class RecipeSO : ScriptableObject , IHasRecipe {
        [SerializeField]
        private Recipe craftingRecipe;
        public IRecipe CraftingRecipe => craftingRecipe;

    }
    [System.Serializable]
    public class Recipe : IRecipe {
        [SerializeField]
        private Item[] requirements = null;
        [SerializeField]
        private InspectorItem[] itemRequirements = new InspectorItem[0];
        public Item[] Requirements => itemRequirements.GetItems();


        [SerializeField]
        private Item result = null;

        public Item Result => result;

        public Recipe(Item[] requirements, Item result) {
            this.requirements = requirements;
            this.result = result;
        }

        public bool Craft(List<Item> inventory, out Item craftedItem) {
            if (CanCraftItem(inventory, true)) {
                craftedItem = result;
                return true;
            }
            craftedItem = null;
            return false;
        }

        public bool CanCraftItem(List<Item> availableItems, bool removeItems = false) {

            List<Item> itemsRemoved = new List<Item>();
            List<Item> itemsAdded = new List<Item>();

            foreach (var requirement in requirements) {
                // FirstOrDefault is another Linq method - this one 
                // returns the first element from a collection which
                // matches the criteria, or the default value 
                // otherwise (usually null)
                Item item = availableItems.FirstOrDefault(item => item != null && item.ItemInfo == requirement.ItemInfo);
                if (item == null || item.NumberOfItems < requirement.NumberOfItems) {
                    if (removeItems) {
                        // Item not found!  Put the other items back and return
                        foreach (var removedItem in itemsRemoved) {
                            availableItems.Add(removedItem);
                        }
                        foreach (var itemAdded in itemsAdded) {
                            availableItems.Remove(itemAdded);
                        }
                    }

                    return false;
                }
                if (removeItems) {
                    // The item was found - remember it for later, and remove it from the list
                    int itemsLeft = item.NumberOfItems - requirement.NumberOfItems;
                    if (itemsLeft > 0) {
                        Item reducedCountItem = new Item(item.ItemInfo, itemsLeft);
                        itemsRemoved.Add(item);
                        availableItems.Remove(item);

                        itemsAdded.Add(reducedCountItem);
                        availableItems.Add(reducedCountItem);
                    }
                    else {
                        itemsRemoved.Add(item);
                    }
                    availableItems.Remove(item);
                }

            }
            return true;

        }

    }
    public interface IRecipe {
        bool CanCraftItem(List<Item> availableItems, bool removeItems = false);
        bool Craft(List<Item> availableItems, out Item craftedItem);

        Item Result { get; }
        Item[] Requirements { get; }

    }
    public interface IHasRecipe {
        IRecipe CraftingRecipe { get; }
    }
}

