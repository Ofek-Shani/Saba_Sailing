using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // Start is called before the first frame update
    LineRenderer lr;
    Vector2 start, end, start0 = Vector2.zero, end0 = Vector2.zero;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetEndpoints(Vector2 start, Vector2 end) {
        this.start = start;
        this.end = end;
    }

    // Update is called once per frame
    void Update()
    {
        if (start == start0 && end == end0) return;

    }
}
