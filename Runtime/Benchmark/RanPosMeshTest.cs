using SoraCore;
using UnityEngine;

public class RanPosMeshTest : Benchmarkable {
    public Mesh mesh;
    public override void Action() {
        Vector3 ranPos = Math.GetRandomPointOnMesh(mesh);
        Debug.Log(ranPos);
    }
}
