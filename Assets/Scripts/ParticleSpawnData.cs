using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawnData : MonoBehaviour {
    private struct PartPiece {
        public Vector3 pos;
        public Vector3 dir;

        public PartPiece(Vector3 p, Vector3 d) {
            pos = p;
            dir = d;
        }
    }
    private List<PartPiece> partQueue = new List<PartPiece>();
    private bool first = true;
    private Vector3 startPos;
    private ParticleSystem particleSys;

    public void Init(Vector3 blastPos, List<Vector3> points) {
        foreach(Vector3 p in points) {
            partQueue.Add(new PartPiece(p, p - blastPos));
        }
        particleSys = GetComponent<ParticleSystem>();
    }
    void FixedUpdate() {
        if (partQueue.Count > 1) {
            if (!first) partQueue.RemoveAt(0);
            else first = false;
            transform.position = partQueue[0].pos;
            transform.rotation = Quaternion.LookRotation(partQueue[0].dir, transform.up);
            particleSys.Emit(1);
        } else {
            Destroy(gameObject, particleSys.main.startLifetime.constant);
        }
    }
}
