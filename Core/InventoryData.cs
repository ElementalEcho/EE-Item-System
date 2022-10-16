using System.Collections.Generic;

namespace EE.InventorySystem {
    public class InventoryData : IInventoryData {
        private int maxInventorySize = 5;
        public int MaxInventorySize => maxInventorySize;
        private List<Item> baseItems = new List<Item>();
        public List<Item> BaseItems => baseItems;

        public InventoryData(int maxInventorySize, List<Item> baseItems) {
            this.maxInventorySize = maxInventorySize;
            this.baseItems = baseItems;
        }

        private ItemDropInfoContainer itemDropInfoContainer = new ItemDropInfoContainer();
        public ItemDropInfoContainer ItemDropInfoContainer => itemDropInfoContainer;
    }
    public class ItemDropInfoContainer {
        public float dropForce = 10;
        public float dropRange = 3;
        public float dropRotationSpeed = 3;
        public float dropRotationAngle = 90;
        public float arcHight = 90;

        public ItemDropInfo GetDropPosition(EE.Core.IFacingDirection facingDirection) {
            ItemDropInfo itemDropInfo = new ItemDropInfo();
            var direction = Mathf.Sign(facingDirection.FacingDirection.x);

            float pointX = UnityEngine.Random.Range(dropRange / 2, dropRange);
            float pointY = UnityEngine.Random.Range(0, dropRange / 2);
            itemDropInfo.dropPosition = new Vector2(pointX * direction, pointY / 2);

            itemDropInfo.rotationAngle = UnityEngine.Random.Range(0, dropRotationAngle);

            itemDropInfo.dropDuration = dropRange / dropForce;
            itemDropInfo.arcHight = arcHight;
            itemDropInfo.dropRotationSpeed = dropRotationSpeed * direction;

            return itemDropInfo;
        }
    }
    public struct ItemDropInfo {
        public Vector2 dropPosition;
        public float arcHight;
        public float dropDuration;
        public float dropRotationSpeed;
        public float rotationAngle;
    }


    public interface IInventoryData {
        int MaxInventorySize { get; }
        List<Item> BaseItems { get; }
    }
}
