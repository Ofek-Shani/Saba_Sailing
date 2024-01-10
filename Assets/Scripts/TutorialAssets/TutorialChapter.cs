using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

[Serializable]
public class TutorialChapter : MonoBehaviour // ScriptableObject
{
    [SerializeField] public string chapterName;
    public TargetObject[] targetObjects;
    public GameObject arrowPrefab;
    public string explanationText;
    public TutorialStep[] steps;
    private Transform arrowsContainer;
    private Text explanationT;
    private GameObject explanationP;
    private Image explanationI;

    private int stepIndex = -1;
    TutorialStep currentStep;
    TargetObject[] usedTargetObjects;
    LineRenderer[] arrowsLR;
    Vector3[] _points = null;
    List<GameObject> arrows = new List<GameObject>(); 
    int numArrows = 0;

    internal void clear(TutorialManager mgr)
    {
        Debug.Log("Clearing " + chapterName);
        CancelInvoke("switchStep");
        mgr.chapterNameT.text = "Select a lesson to view";
        mgr.explanationT.gameObject.SetActive(false);
        clearArrows();
        if (currentStep != null) currentStep.clear();
        currentStep = null;
        stepIndex = -1;

    }

    void clearArrows() {
        usedTargetObjects = null;
        foreach (GameObject arrow in arrows) arrow.SetActive(false);
        numArrows = 0;
        _points = new Vector3[0];
    }
    internal void load(TutorialManager mgr)
    {
        Debug.Log("Loading " + chapterName);
        mgr.chapterNameT.text = chapterName;
        mgr.explanationT.text = explanationText;
        mgr.explanationT.gameObject.SetActive(true);
        arrowsContainer = mgr.arrowsContainer;
        explanationT = mgr.explanationT;
        explanationP = mgr.explanationP;
        explanationI = explanationP.GetComponent<Image>();
        usedTargetObjects = targetObjects;

        drawArrows();

        Invoke("switchStep", 5f);
    }

    void switchStep() {
        if (steps.Length == 0) return;
        clearArrows();
        stepIndex = (stepIndex++)%steps.Length;
        if (currentStep != null) currentStep.clear();
        stepIndex = (++stepIndex)%steps.Length;
        TutorialStep step = steps[stepIndex];
        usedTargetObjects = step.targetObjects;
        explanationT.text = "";
        StartCoroutine(AnimateStepSwitch(step.explanationText));
        step.doAction();
        float duration = step.duration;
        Invoke("switchStep", duration);
        currentStep = step;
    }

    private IEnumerator AnimateStepSwitch(string explanationText, float duration = 0.5f)
    {
        float pos = 0;
        float elapsedTime = 0;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            pos = Mathf.Clamp01(elapsedTime / duration);
            explanationI.fillAmount = pos;
            if (pos < 1f) yield return null;
        }
        explanationT.text = explanationText;
    }

    /*
    widthCurve: [0,1] [0.3333333,0.1] [0.6666667,1] [1,0.1] [1.333333,1] 
    points: [-5.740664,-4.577865] [2.384186E-07,-2.5] [-5.740664,-4.577865] [13.2942,-6.647789] 
    */
    Vector3 worldPosition(GameObject obj, bool forCanvas = true) {
        Vector3 position = obj.transform.position;
        Vector3 result = forCanvas ? Camera.main.ScreenToWorldPoint(position) : position;
        result.z = 0;
        return result;
    }

    Vector3 lociFromTargetObject(TargetObject to) {
        Vector3 r = worldPosition(to.targetObject, to.targetIsCanvas);
        r.z = 0;
        return r;
    }

    List <LineRenderer> lrs = new List<LineRenderer>();
    void setArrows(int num) {
        numArrows = num;
        for (int i = arrows.Count; i < num; i++) {
            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            arrows.Add(arrow);
            lrs.Add(arrow.GetComponent<LineRenderer>());
        }
    }
    public void drawArrows() {
        if (usedTargetObjects == null || usedTargetObjects.Length == 0) return;
        //LineRenderer[] lrs = arrowsContainer.GetComponents<LineRenderer>();
        // Ensure we have enough line renderers for the arrows.
        //if (lrs.Length < usedTargetObjects.Length) {
        //    for (int lri = lrs.Length; lri < usedTargetObjects.Length; lri++) 
        //        lrs.Append(new LineRenderer());
        //        arrowsContainer.AddComponent<LineRenderer>();
        //    lrs = arrowsContainer.GetComponents<LineRenderer>();
        //}
        setArrows(usedTargetObjects.Length);
        Vector3 pivot = worldPosition(explanationT.gameObject, true);
        int i = -1; 
        while (++i < numArrows) { //Math.Min(lrs.Length, usedTargetObjects.Length)) {
            Vector3 endP = lociFromTargetObject(usedTargetObjects[i]);
            lrs[i].positionCount = 2;
            lrs[i].SetPosition(0, pivot);
            lrs[i].SetPosition(1, endP);
            lrs[i].startWidth = 1f;
            lrs[i].endWidth = 0.1f;
            arrows[i].SetActive(true);
        }
    }

    bool samePoints(TargetObject[] points) {
        if (points == null || _points == null || points.Length != _points.Length) return false;
        for (int i=0; i < points.Length; i++) if (_points[i] != lociFromTargetObject(points[i])) return false;
        return true;
    }

    void clearZ(Vector3[] points) {
        for (int i = 0; i < points.Length; i++)
            points[i].z = 0f;
    }
    void logVector<type>(type[] vals) {
        if (vals == null) {Debug.Log(new type[0].GetType().Name + " is null"); return; }
        String s = vals.GetType().Name + ": [";
        foreach (type v in vals) s += v + ", ";
        Debug.Log(s + "]");
    }
    void logPoints(Vector3[] points) { logVector<Vector3>(points); }
    //     if (points == null) {Debug.Log("")}
    //     String s = "float[]: [";
    //     foreach (Vector3 point in points) s += point + ", ";
    //     Debug.Log(s + "]");
    // }
    void setLineColor(LineRenderer lineRenderer, Color lineColor, Color fillColor) {
        Material lineMaterial = lineRenderer.material;
        // Set line color
        lineMaterial.SetColor("_Color", lineColor);
        // Set filling color
        lineMaterial.SetColor("_EmissionColor", fillColor);
        lineRenderer.material = lineMaterial;
    }

    void Update() {
        drawArrows();
    }

}
