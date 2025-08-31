using System;

using UnityEngine;

public class CameraController : MonoBehaviour {
    private GameObject player;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime = 0.3f;
    [SerializeField] private float cameraToPlayerDiff;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void LateUpdate() {
        if (!NeedToMoveCamera()) return;

        Vector3 targetPos = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            -10
        );
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref velocity,
            smoothTime
        );
    }

    private bool NeedToMoveCamera() {
        Vector3 playerPostion = player.transform.position;
        Vector3 cameraPosition = transform.position;
        float distance = Vector3.Distance(playerPostion, cameraPosition);

        return distance > cameraToPlayerDiff;
    }
}
