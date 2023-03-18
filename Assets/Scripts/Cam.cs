using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cam : MonoBehaviour {
    public void ZoomOutCamera() {
        var pos = new Vector3(transform.position.x, transform.position.y + 0.30f, transform.position.z - 0.65f);
        transform.position = Vector3.Lerp(transform.position, pos, 0.45f);
    }
    public void CrashZoomOut() {
        var pos = new Vector3(transform.position.x, transform.position.y + 0.0075f, transform.position.z - 0.025f);
        transform.position = Vector3.MoveTowards(transform.position, pos,1f);
    }
}
