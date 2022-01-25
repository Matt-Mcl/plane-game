using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    public Vector3 target;
    public Quaternion rotation;
    public Transform planeTransform;

    // Use this for initialization
    void Awake () {
        target = transform.TransformPoint(0, 0, 0);
        rotation = transform.rotation;
        planeTransform = transform;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        float step = 100.0f * Time.deltaTime;
        float rot = 5.0f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rot);
    }
}
