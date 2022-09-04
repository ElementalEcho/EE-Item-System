using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EE.InventorySystem.Crafting {
    public interface ICrafter {
        bool Craft(List<Item> items, int recipeIndex, out Item item);
    }
}
namespace EE.InventorySystem.Crafting.Impl {
    public class Crafter : ICrafter {
        private ICraftingData craftingData;

        public Crafter(ICraftingData craftingData) {
            this.craftingData = craftingData;
        }

        public bool Craft(List<Item> items, int recipeIndex, out Item item) {
            item = null;
            return craftingData.Recipes.Count > recipeIndex && craftingData.Recipes[recipeIndex].Craft(items, out item);
        }
    }
}
