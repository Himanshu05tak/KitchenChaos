using System;
using System.Collections.Generic;
using UnityEngine;

namespace Counters.Plate
{
    public class PlateCounterVisual : MonoBehaviour
    {
        [SerializeField] private Transform counterTopPoint;
        [SerializeField] private Transform plateVisualPrefab;
        [SerializeField] private PlatesCounter platesCounter;


        private List<GameObject> _plateVisualGameObjectList;

        private void Awake()
        {
            _plateVisualGameObjectList = new List<GameObject>();
        }

        private void Start()
        {
            platesCounter.OnPlateSpawned += PlatesCounterOnOnPlateSpawned;
            
            platesCounter.OnPlateRemoved += PlatesCounterOnOnPlateRemoved;
        }

        private void PlatesCounterOnOnPlateRemoved(object sender, EventArgs e)
        {
            var plateGameObject = _plateVisualGameObjectList[^1];
            _plateVisualGameObjectList.Remove(plateGameObject);
            Destroy(plateGameObject);
        }

        private void PlatesCounterOnOnPlateSpawned(object sender, EventArgs e)
        {
            const float plateOffsetY = .1f;
            var plateTransformVisual = Instantiate(plateVisualPrefab, counterTopPoint);
            plateTransformVisual.localPosition = new Vector3(0, plateOffsetY * _plateVisualGameObjectList.Count , 0);
            _plateVisualGameObjectList.Add(plateTransformVisual.gameObject);
        }
    }
}
