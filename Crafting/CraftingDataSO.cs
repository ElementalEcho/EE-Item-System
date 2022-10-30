using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EE.ItemSystem.Crafting.Impl {

    public class CraftingDataSO : ScriptableObject,IHasCraftingData {
        [SerializeField]
        private List<RecipeSO> availableRecipes = new List<RecipeSO>();
        public ICraftingData CraftingData { get {
                List<IHasRecipe> recipies = availableRecipes.Cast<IHasRecipe>().ToList();
                return new CraftingData(recipies);
            }
        } 
    }

    public class CraftingData : ICraftingData {

        public CraftingData(List<IHasRecipe> recipes) {
            this.recipes = recipes;
        }
        private List<IHasRecipe> recipes = new List<IHasRecipe>();
        public List<IRecipe> Recipes => recipes.Select(recipe => recipe.CraftingRecipe).ToList();
    }
}

namespace EE.ItemSystem.Crafting {
    public interface ICraftingData {
        List<IRecipe> Recipes { get; }
    }
    public interface IHasCraftingData {
        ICraftingData CraftingData { get; }
    }
}

