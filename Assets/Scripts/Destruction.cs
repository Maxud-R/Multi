using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour {
    [SerializeField] private int destState = 0;
    private const int MESH_PIECES = 5;
    public GameObject expl;
    public Mesh[] states = new Mesh[MESH_PIECES]; 
    private bool stateChanged = false;
    void Start() {
        
    }
    
    void Update() {
        if (stateChanged) {
            GetComponent<MeshFilter>().sharedMesh = states[destState];
            stateChanged = false;
        }
    }
    void OnTriggerEnter(Collider other) {
        if (other.name == expl.transform.GetChild(0).name) {
            Debug.DrawRay(other.ClosestPoint(transform.position), transform.position - other.ClosestPoint(transform.position), Color.red, 10f);
            Debug.Log(transform.position - other.ClosestPoint(transform.position));
        }
		if (other.name == expl.transform.GetChild(0).name && destState < MESH_PIECES-1) {
            destState++;
            stateChanged = true;
        }
    }
}
