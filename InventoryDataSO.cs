using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EE.InventorySystem.Impl {
    [InlineEditor]
    [Serializable]
    [HideLabel]
    internal class InventoryDataSO : ScriptableObject, IHasInventoryData {
        [SerializeField, PropertyOnly]
        private InventoryData inventoryData = null;
        public IInventoryData InventoryData => inventoryData;
    }

    [Serializable]
    internal class InventoryData : IInventoryData {
        [SerializeField]
        private int maxInventorySize = 5;
        public int MaxInventorySize => maxInventorySize;
        [SerializeField]
        private List<Item> baseItems = new List<Item>();
        public List<Item> BaseItems => baseItems;

        [SerializeField]
        private ItemDropInfoContainer itemDropInfoContainer = new ItemDropInfoContainer();
        public ItemDropInfoContainer ItemDropInfoContainer => itemDropInfoContainer;
        [SerializeField]
        private int currentItem = 0;
        public int CurrentItem => currentItem;
    }

    internal class ItemDropInfoContainer {
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
    internal struct ItemDropInfo {
        public Vector2 dropPosition;
        public float arcHight;
        public float dropDuration;
        public float dropRotationSpeed;
        public float rotationAngle;
    }

    [Serializable]
    internal class DefaultInventoryData : IInventoryData {
        public int MaxInventorySize => 1;
        public List<Item> BaseItems => new List<Item>();
    }
}

namespace EE.InventorySystem {
    public interface IHasInventoryData {
        IInventoryData InventoryData { get; }
    }

    public interface IInventoryData {
        int MaxInventorySize { get; }
        List<Item> BaseItems { get; }
    }
}