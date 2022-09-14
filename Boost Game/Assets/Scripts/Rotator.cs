using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Range (0, 5)][SerializeField] float rotationSpeed;
    [SerializeField] bool isLeft = true;

    int sideOfRotation = 1;
    Vector3 position =  new Vector3(0, 0, 1);

    private void Start()
    {
        if (!isLeft) sideOfRotation = -1; // установит сторону вращения вправо
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(position, sideOfRotation * (rotationSpeed * 100) * Time.deltaTime);
    }
}
