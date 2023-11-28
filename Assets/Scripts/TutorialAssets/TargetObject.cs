using UnityEngine;

[System.Serializable]
public struct TargetObject {
    public GameObject targetObject;
    public bool targetIsCanvas;
    public override string ToString() { return targetObject + ": " + (targetIsCanvas? "canvas" : "element");}
}