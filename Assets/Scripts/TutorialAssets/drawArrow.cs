using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawArrow : MonoBehaviour
{
    Vector3 start, stop;
    public float width;
    LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.TransformPoint(Vector3.zero);
        Debug.Log("start: " + start);
        lineRenderer = GetComponent<LineRenderer>(); 
        // makeArrow(start, stop, width, Color.white, Color.green);       
    }
    void logPoints(Vector3[] points) {
        foreach (Vector3 point in points) Debug.Log(point);
        Debug.Log("------------------");
    }
    void makeArrow(Vector3 startPoint, Vector3 endPoint, float w, Color line, Color fill) {
        float l = Vector2.Distance(startPoint, endPoint);
        Vector3[] points = {Camera.main.ScreenToWorldPoint(startPoint), Camera.main.ScreenToWorldPoint(endPoint) };
        logPoints(points);
        //     new Vector3(0,0,0),
        //     new Vector3(0,w/2,0),
        //     new Vector3(l, w/2,0),
        //     new Vector3(l,w,0),
        //     new Vector3(l + w, 0,0),
        //     new Vector3(l, -w,0),
        //     new Vector3(l, -w/2,0),
        //     new Vector3(0,-w/2,0),
        //     new Vector3(0,0,0)
        // };
        // Vector2 direction = endPoint - startPoint;
        // float rotation = Mathf.Atan2(direction.y, direction.x);
        // for (int i = 0; i < points.Length; i++) {
        //     Vector3 point = points[i]; 
        //     Debug.Log("rotate from: " + point + " by: " + rotation);
        //     point = rotateAndMove(point, rotation, start);
        //     Debug.Log(" ---> " + point);
        //     points[i] = point;

        // }
        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);
        setLineColor(line, fill);
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.1f;
    }

    void setLineColor(Color lineColor, Color fillColor) {
        Material lineMaterial = lineRenderer.material;
        // Set line color
        lineMaterial.SetColor("_Color", lineColor);
        // Set filling color
        lineMaterial.SetColor("_EmissionColor", fillColor);
        lineRenderer.material = lineMaterial;
    }
    // rotate a 2d vector around the 0,0, in place, returns the same vector as result.
    Vector3 rotateAndMove(Vector3 originalPoint, float angle, Vector3 translate) {
        float x = originalPoint.x, y = originalPoint.y;
        return new Vector3(
            x * Mathf.Cos(angle) - y * Mathf.Sin(angle),
            x * Mathf.Sin(angle) + y * Mathf.Cos(angle), 0) +
            translate;
    }
    // Update is called once per frame
    Vector3 _start, _stop;
    void Update()
    {
        if (_start != start || _stop != stop) {
            makeArrow(start, stop, width, Color.white, Color.green);        
            _start = start;
            _stop = stop;
        }
    }
}
