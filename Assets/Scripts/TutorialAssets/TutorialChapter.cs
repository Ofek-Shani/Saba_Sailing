using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TutorialChapter : MonoBehaviour // ScriptableObject
{
    [SerializeField] public string chapterName;
    public TargetObject[] targetObjects;
    public string explanationText;
    public TutorialStep[] steps;
    private LineRenderer arrowLR;
    private Text explanationT;

    private int stepIndex = -1;
    TutorialStep currentStep;
    TargetObject[] usedTargetObjects;
    internal void clear(TutorialManager mgr)
    {
        Debug.Log("Clearing " + chapterName);
        CancelInvoke("ExecuteStep");
        mgr.chapterNameT.text = "Select a lesson to view";
        mgr.explanationT.gameObject.SetActive(false);
        usedTargetObjects = null;
        mgr.arrowLR.positionCount = 0;
        _points = new Vector3[0];
        mgr.arrowLR.SetPositions(_points);
    }

    internal void load(TutorialManager mgr)
    {
        Debug.Log("Loading " + chapterName);
        mgr.chapterNameT.text = chapterName;
        mgr.explanationT.text = explanationText;
        mgr.explanationT.gameObject.SetActive(true);
        arrowLR = mgr.arrowLR;
        explanationT = mgr.explanationT;
        usedTargetObjects = targetObjects;

        drawArrows();

        switchStep();
    }

    void switchStep() {
        if (steps.Length == 0) return;
        stepIndex = (stepIndex++)%steps.Length;
        if (currentStep != null) currentStep.clear();
        TutorialStep step = steps[stepIndex];
        usedTargetObjects = step.targetObjects;
        float duration = step.duration;
        Invoke("switchStep", duration);
    }

    /*
    widthCurve: [0,1] [0.3333333,0.1] [0.6666667,1] [1,0.1] [1.333333,1] 
    points: [-5.740664,-4.577865] [2.384186E-07,-2.5] [-5.740664,-4.577865] [13.2942,-6.647789] 
    */
    Vector3 lociFromTargetObject(TargetObject to) {
        Vector3 r = worldPosition(to.targetObject, to.targetIsCanvas);
        r.z = 0;
        return r;
    }

    public void drawArrows() {
        // logVector<TargetObject>(usedTargetObjects);
        if (usedTargetObjects == null || usedTargetObjects.Length == 0) return;
        List<Vector3> pointsL = new List<Vector3>();
        List<float> lengthsL = new List<float>();
        Vector3 pivot = worldPosition(explanationT.gameObject, true);
        AnimationCurve widthCurve = new AnimationCurve();
        // Set the curve keys
        String r = "widthCurve: ", s = "points: ";
        float length = 0;
        int i = 0; 
        bool usePivot = false;
        Vector3 lastP = Vector3.zero;
        bool starting = true;
        while (i < usedTargetObjects.Length) {
            Vector3 newP = usePivot ? pivot : lociFromTargetObject(usedTargetObjects[i]);
            if (!usePivot) {i++;}
            usePivot = !usePivot;
            pointsL.Add(newP);
            if (!starting) {
                float l = Vector3.Distance(newP, lastP);
                lengthsL.Add(l);
                length += l;
                lastP = newP;
            }
            starting = false;
        }
        Vector3[] points = pointsL.ToArray();
        if (samePoints(points)) return;
        _points = points;

        float[] lengths = lengthsL.ToArray();
        widthCurve.AddKey(0, 0.1f);
        usePivot = true;
        float stretch = 0;
        logVector(lengths);
        r+= "[0, 0.1f] ";
        s+= "[" + points[0].x + ", " + points[0].y + "]";
        for (i=1; i < points.Length; i++) {
            stretch += lengths[i-1];
            float f = stretch/length;
            float w = usePivot? 1f : 0.1f;
            widthCurve.AddKey(f, w);
            r+= "[" + f + "," + w + "] ";
            s += "["+points[i].x+ "," + points[i].y + "] ";
            usePivot = !usePivot;
        }
        // void makeArrow(LineRenderer lineRenderer, Vector3 startPoint, Vector3 endPoint, float w, Color line, Color fill) {
        // logVector<Vector3>(points);
        // Debug.Log(r + "\n" + s + "\n==================");
        arrowLR.positionCount = points.Length;
        arrowLR.SetPositions(points);
        arrowLR.widthCurve = widthCurve; 
    }

    Vector3 worldPosition(GameObject obj, bool forCanvas = true) {
        Vector3 position = obj.transform.position;
        Vector3 result = forCanvas ? Camera.main.ScreenToWorldPoint(position) : position;
        result.z = 0;
        return result;
    }
    Vector3[] _points = null;
    bool samePoints(Vector3[] points) {
        if (points == null || _points == null || points.Length != _points.Length) return false;
        for (int i=0; i < points.Length; i++) if (_points[i] != points[i]) return false;
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
        // drawArrows();
    }

}
