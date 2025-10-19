using System;

using UnityEngine;

public class CameraController : MonoBehaviour {
    private GameObject player;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;
    [SerializeField] private float diff;

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
        if (( Math.Abs(transform.position.y - player.transform.position.y) > diff ) || ( Math.Abs(transform.position.x - player.transform.position.x) > diff )) {
            return true;
        }
        return false;
    }
}
