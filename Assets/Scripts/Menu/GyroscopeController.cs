using System;
using UnityEngine;

public class GyroscopeController : MonoBehaviour
{
    [SerializeField] private float _speed = 0f;

    private Gyroscope _gyroscope = null;

    private void Start()
    {
        _gyroscope = Input.gyro;
        _gyroscope.enabled = true;
    }

    private void Update()
    {
        transform.Translate((float)Math.Round(_gyroscope.rotationRateUnbiased.y, 1) * _speed * Time.deltaTime,
            (float)Math.Round(_gyroscope.rotationRateUnbiased.x, 1) * _speed * Time.deltaTime, 0f);

        if (Mathf.Abs(transform.position.x) > 4.75)
        {
            var posX = transform.position;
            posX.x = 0;
            transform.position = posX;
        }

        if (Mathf.Abs(transform.position.y) > 3)
        {
            var posY = transform.position;
            posY.y = 0;
            transform.position = posY;
        }
    }
}