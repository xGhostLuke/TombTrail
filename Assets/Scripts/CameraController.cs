using System;
using UnityEngine;

public class CameraController : MonoBehaviour {
    GameObject player;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        if (NeedToMoveCamera()) {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }
    }

    private bool NeedToMoveCamera() {
        if (( Math.Abs(transform.position.y - player.transform.position.y) > 4 ) || ( Math.Abs(transform.position.x - player.transform.position.x) > 4 )) {
            return true;
        }
        return false;
    }
}
