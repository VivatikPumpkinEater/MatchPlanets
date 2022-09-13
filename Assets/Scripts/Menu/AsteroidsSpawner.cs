using UnityEngine;

public class AsteroidsSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject[] _meteors;

    [SerializeField] private float _speed;

    private GameObject _currentAsteroid;
    private Vector3 _direction = Vector3.zero;
    private void Update()
    {
        if (!_currentAsteroid)
        {
            _currentAsteroid =
                Instantiate(_meteors[Random.Range(0, _meteors.Length)],
                    _spawnPoints[Random.Range(0, _spawnPoints.Length)].position,
                    Quaternion.identity);

            if (_currentAsteroid.transform.position.x > 0)
            {
                _direction = Vector3.down - Vector3.right;
            }
            else
            {
                _direction = Vector3.down + Vector3.right;
            }
            
            Destroy(_currentAsteroid, Random.Range(1f,2f));
        }
        else
        {
            _currentAsteroid.transform.Translate(_direction * (_speed * Time.deltaTime));
        }
    }
}