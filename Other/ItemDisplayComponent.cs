using EE.Core;
using EE.Core.PoolingSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace EE.InventorySystem.Impl {
    internal class ItemDisplayComponent : SerializedMonoBehaviour {
        public IInventoryComponent inventoryComponent;
        public PoolableComponent displayPrefab;
        public Collider2D displayArea;

        public Dictionary<IItemInfo, List<PoolableComponent>> spawnedItems = new Dictionary<IItemInfo, List<PoolableComponent>>();

        public Vector2 startbounds;
        public Vector2 maxbounds;
        public Vector2 currentbounds;

        public float xItemOffSet = 0.2f;
        public float yItemOffSet = 0.2f;

        private void Awake() {
            inventoryComponent = GetComponent<IInventoryComponent>();
        }
        private void Start() {
            inventoryComponent.AddInventoryAlteredEvent(DropInventoryItems);
            startbounds = new Vector2(displayArea.bounds.min.x - displayArea.transform.position.x + 0.1f, displayArea.bounds.min.y - displayArea.transform.position.y);
            maxbounds = new Vector2(displayArea.bounds.max.x - displayArea.transform.position.x - 0.1f, displayArea.bounds.max.y - displayArea.transform.position.y);
            currentbounds = startbounds;

            xItemOffSet = Mathf.Abs(displayArea.bounds.max.x - displayArea.bounds.min.x) * 0.35f;
            yItemOffSet = Mathf.Abs(displayArea.bounds.max.y - displayArea.bounds.min.y) * 0.35f;

        }
        private void DropInventoryItems() {
            for (int i = inventoryComponent.Items.Length - 1; i >= 0; i--) {
                var inventoryItem = inventoryComponent.Items[i];
                if (Item.IsNull(inventoryItem)) {
                    continue;
                }
                for (int g = 0; g < inventoryItem.NumberOfItems; g++) {
                    PoolManager.SpawnObjectAsChild(inventoryItem.ItemInfo.ItemToDrop, transform, currentbounds);
                    float xPosition = currentbounds.x + xItemOffSet;
                    float yPosition = currentbounds.y;

                    if (xPosition > maxbounds.x) {
                        xPosition = startbounds.x + Random.Range(xItemOffSet / 4, xItemOffSet);
                        yPosition += yItemOffSet;
                    }
                    if (yPosition > maxbounds.y) {
                        yPosition -= maxbounds.y;
                    }
                    currentbounds = new Vector2(xPosition, yPosition);

                }
                inventoryComponent.RemoveItem(inventoryItem, true);
            }

        }
        private void UpdateDisplayItems() {
            foreach (var item in inventoryComponent.Items) {
                if (spawnedItems.TryGetValue(item.ItemInfo, out var displayedItems)) {
                    if (item.NumberOfItems > displayedItems.Count) {
                        var NumberOfItemsToAdd = item.NumberOfItems - displayedItems.Count;
                        for (int i = 0; i < NumberOfItemsToAdd; i++) {
                            var display = PoolManager.SpawnObjectAsChild(displayPrefab, transform);
                            display.transform.position = RandomPointInBounds(displayArea.bounds);
                            display.GetComponent<SpriteRenderer>().sprite = item.ItemInfo.ItemToDrop.GetComponent<SpriteRenderer>().sprite;
                            displayedItems.Add(display);
                        }
                    }
                    else if (item.NumberOfItems < displayedItems.Count) {
                        var NumberOfItemsToRemove = displayedItems.Count - item.NumberOfItems;
                        for (int i = 0; i < NumberOfItemsToRemove; i++) {
                            var gm = displayedItems[0];
                            PoolManager.ReleaseObject(gm);
                            displayedItems.RemoveAt(0);
                        }
                    }
                }
                else {
                    displayedItems = new List<PoolableComponent>();
                    for (int i = 0; i < item.NumberOfItems; i++) {
                        var display = PoolManager.SpawnObjectAsChild(displayPrefab, transform);
                        display.transform.position = RandomPointInBounds(displayArea.bounds);
                        display.GetComponent<SpriteRenderer>().sprite = item.ItemInfo.ItemToDrop.GetComponent<SpriteRenderer>().sprite;
                        displayedItems.Add(display);
                    }
                    spawnedItems.Add(item.ItemInfo, displayedItems);
                }
            }
        }
        public static Vector2 RandomPointInBounds(Bounds bounds) {
            return new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );
        }
    }
}
