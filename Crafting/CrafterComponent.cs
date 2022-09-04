using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace EE.InventorySystem.Crafting {
    public interface ICrafterComponent {

    }
}
namespace EE.InventorySystem.Crafting.Impl {
    public class CrafterComponent : MonoBehaviour, ICrafterComponent {
        [SerializeField]
        private CraftingDataSO craftingDataSO;


        private ICrafter crafter;

        private IInventoryComponent inventory;

        [SerializeField]
        private int index = 0;

        private void Awake() {
            inventory = GetComponent<IInventoryComponent>();

            ICraftingData craftingData = craftingDataSO != null ? craftingDataSO.CraftingData : null;
            crafter = new Crafter(craftingData);
        }
        [Button]
        public void Craft() {
            if (crafter.Craft(inventory.Items.ToList(), index, out Item item)) {
                inventory.AddItem(new Item(item.ItemInfo, item.NumberOfItems));
                Debug.Log("Item crafted");
            }
            else {
                Debug.Log("Crafting failed");
            }
        }
    }
}