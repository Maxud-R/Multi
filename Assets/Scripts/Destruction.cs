using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destruction : MonoBehaviour {
    [SerializeField] private int destState = 0;
    private int destStatePrev;
    private const int MESH_PIECES = 5;
    private static Vector3Int slices = new Vector3Int(2, 1, 7);
    private Vector3[] splittedSize = new Vector3[slices.x*slices.y*slices.z];
    public GameObject expl;
    public Mesh[] states = new Mesh[MESH_PIECES]; 
    public GameObject partSys;
    void Start() {
        destStatePrev = destState;
        splittedSize = SplitSize(gameObject.GetComponent<BoxCollider>().bounds.size, slices); //перенести из цикла в пофреймовый спавн частиц по одной.
    }
    
    void FixedUpdate() {
        if (destStatePrev != destState) {
            GetComponent<MeshFilter>().sharedMesh = states[destState];
            destStatePrev = destState;
        }
    }
    void OnTriggerEnter(Collider shockwave) {
        if (shockwave.name == expl.transform.GetChild(0).name) {
            //Debug.DrawRay(other.ClosestPoint(transform.position), transform.position - other.ClosestPoint(transform.position), Color.red, 10f);
            //partSys.transform.rotation = Quaternion.LookRotation(-shockwave.transform.position, shockwave.transform.up);//??
            List<Vector3> points = new List<Vector3>();
            foreach(Vector3 position in splittedSize) {
                Vector3 a = shockwave.ClosestPoint(position);
                if (a == position) {
                    points.Add(a);
                }
            }
            Instantiate(partSys, shockwave.transform.position, Quaternion.identity).GetComponent<ParticleSpawnData>().Init(shockwave.transform.position, points);
        }
		if (shockwave.name == expl.transform.GetChild(0).name && destState < MESH_PIECES-1) {
            destState++;
        }
    }
    Vector3[] SplitSize(Vector3 size, Vector3Int pieces) {
        Vector3 corner = transform.position - size/2;
        Vector3[] pieceCentres = new Vector3[pieces.x*pieces.y*pieces.z];
        for(int x = 0; x < pieces.x; x++) {
                for(int z = 0; z < pieces.z; z++) {
                    Vector3 point = new Vector3(corner.x+(size.x/pieces.x)*x+(size.x/pieces.x)/2, transform.position.y ,corner.z+(size.z/pieces.z)*z+(size.z/pieces.z)/2);
                    pieceCentres[(x*pieces.z)+z] = point;
                }
        }
        return pieceCentres;
    }
}
