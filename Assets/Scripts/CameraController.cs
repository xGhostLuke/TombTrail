using System;

using UnityEngine;

public class CameraController : MonoBehaviour {
    GameObject player;
    Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate() {
        if (NeedToMoveCamera()) {
            Vector3 targetPos = new Vector3(player.transform.position.x, player.transform.position.y, -10);
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
        }
    }

    private bool NeedToMoveCamera() {
        if (( Math.Abs(transform.position.y - player.transform.position.y) > 2 ) || ( Math.Abs(transform.position.x - player.transform.position.x) > 2 )) {
            return true;
        }
        return false;
    }
}
