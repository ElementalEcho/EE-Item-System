using EE.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GrowingSector {

    public Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
    public Vector3 startScale = new Vector3(1, 1, 1);
    public Sprite sprite = null;
    public float speed = 1;

}
public class GrowingComponent : EEMonobehavior, IUpdater, IPoolable
{
    [SerializeField]  private Vector3 currentScale = new Vector3(1, 1, 1);


    public GrowingSector[] growingSectors = new GrowingSector[0];

    [SerializeField] 
    private GrowingSector[] fullyGrownActions = new GrowingSector[0];

    public bool FullyGrownFlag => currentIndex >= growingSectors.Length;
    public GrowingSector CurrentFullyGrown => fullyGrownActions[currentIndex];

    private int currentIndex = 0;

    public void Start() {
        transform.localScale = fullyGrownActions[0].startScale;
        currentScale = transform.localScale;
    }
    public void CustomUpdate(float tickSpeed) {
        if (!gameObject.activeSelf || FullyGrownFlag) {
            return;
        }
        var growSpeed = CurrentFullyGrown.speed * tickSpeed;
        currentScale = new Vector3(currentScale.x + growSpeed, currentScale.y + growSpeed, currentScale.z + growSpeed);
        transform.localScale = currentScale;
        if (currentScale.y >= CurrentFullyGrown.maxScale.y) { 
        
        }
    }
    public override void OnRelease() {
        transform.localScale = fullyGrownActions[0].startScale;
        currentScale = transform.localScale;
        currentIndex = 0;
    }

#if UNITY_EDITOR

    [Button]
    private void Reset() {
        transform.localScale = fullyGrownActions[0].startScale;
        currentScale = transform.localScale;
        currentIndex = 0;
    }
#endif

}
