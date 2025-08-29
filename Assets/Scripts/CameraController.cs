using System;

using UnityEngine;

public class CameraController : MonoBehaviour {
    GameObject player;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update() {
        if (NeedToMoveCamera()) {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -10), Time.deltaTime * 10f);
        }
    }

    private bool NeedToMoveCamera() {
        if (( Math.Abs(transform.position.y - player.transform.position.y) > 2 ) || ( Math.Abs(transform.position.x - player.transform.position.x) > 2 )) {
            return true;
        }
        return false;
    }
}
