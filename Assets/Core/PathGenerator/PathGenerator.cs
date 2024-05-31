//Copyright takiido. All Rights Reserved

using UnityEngine;
using System.Collections.Generic;

namespace Core.PathGenerator
{
    public class PathGenerator : MonoBehaviour
    {
        public GameObject[] pathPrefabs;
        public int initialSegments = 5;
        public Transform player;
        public float spawnDistance = 50.0f;

        private Queue<GameObject> _pathSegments = new Queue<GameObject>();
        private float _spawnZ = 0.0f;
        private float _segmentLength;

        private void Start()
        {
            if (pathPrefabs.Length == 0)
            {
                Debug.LogError("No prefabs assigned");
                return;
            }

            _segmentLength = pathPrefabs[0].GetComponent<Renderer>().bounds.size.z;

            for (int i = 0; i < initialSegments; i++)
                SpawnSegment();
        }

        private void Update()
        {
            if (player.position.z - spawnDistance > _spawnZ - (initialSegments * _segmentLength))
            {
                SpawnSegment();
                RemoveOldSegment();
            }
        }

        private void SpawnSegment()
        {
            GameObject segment = Instantiate(pathPrefabs[Random.Range(0, pathPrefabs.Length)],
                new Vector3(0.0f, 0.0f, _spawnZ), Quaternion.identity);
            _pathSegments.Enqueue(segment);
            _spawnZ += _segmentLength;
        }

        private void RemoveOldSegment()
        {
            if (_pathSegments.Count > initialSegments)
                Destroy(_pathSegments.Dequeue());
        }
    }
}
