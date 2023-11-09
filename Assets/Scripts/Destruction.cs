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
    public GameObject partSysPref;
    private ParticleSystem partSys;
    void Start() {
        destStatePrev = destState;
        splittedSize = SplitSize(gameObject.GetComponent<BoxCollider>().bounds.size, slices);
        partSys = GameObject.Find("StabPieces").GetComponent<ParticleSystem>();
    }
    
    void FixedUpdate() {
        if (destStatePrev != destState) {
            GetComponent<MeshFilter>().sharedMesh = states[destState];
            destStatePrev = destState;
        }
    }
    void OnTriggerEnter(Collider shockwave) {
        if (shockwave.name == expl.transform.GetChild(0).name) {
            int pointsCount = 0;
            foreach(Vector3 position in splittedSize) {
                Vector3 a = shockwave.ClosestPoint(position);
                if (a == position) {
                    partSys.transform.position = a;
                    partSys.transform.rotation = Quaternion.LookRotation(a - shockwave.transform.position, transform.up);
                    partSys.Emit(1);
                    pointsCount++;
                    Debug.DrawRay(a, transform.TransformPoint(Vector3.up), Color.red, 1.5f);
                }
            }
            destState = (int)Mathf.Ceil((MESH_PIECES-1)*(Mathf.Round(pointsCount/((float)splittedSize.Length/100))/100));
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
