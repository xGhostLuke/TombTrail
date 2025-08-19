using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        bool moveCamera = NeedToMoveCamera();

        if (moveCamera) return;

        Vector3 playerPosition = player.transform.position;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(playerPosition.x, playerPosition.y, -10);
        float interpolationTime = interpolationTime.deltaTime * 10f;

        transform.position = Vector3.Lerp(startPosition, endPosition, interpolationTime);
    }

    private bool NeedToMoveCamera()
    {
        bool isOverThresholdForY = Math.abs(transform.position.y - player.transform.position.y) > 2;
        bool isOverThresholdForX = Math.abs(transform.position.x - player.transform.position.x) > 2;

        return isOverThresholdForY || isOverThresholdForX;
    }
}
