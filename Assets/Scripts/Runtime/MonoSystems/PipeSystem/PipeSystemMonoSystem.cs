using PlazmaGames.Core;
using PlazmaGames.ProGen.Sampling;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

namespace BeneathTheSurface.MonoSystems
{
    public class PipeSystemMonoSystem : MonoBehaviour, IPipeSystemMonoSystem
    {
        [SerializeField] private int _numberOfSectors;
        [SerializeField] private BoundsInt _bounds;
        [SerializeField] private float _height;
        [SerializeField] private float _minDistBetweenSectors = 10;

        private List<Vector2> _sectorLocations;
        private Vector2 _masterSector;

        private float GridSize => GameManager.GetMonoSystem<IBuildingMonoSystem>().GetGridSize();

        private void SpawnSectors()
        {
            PoissonSampler sampler = new PoissonSampler(_bounds.size.x, _bounds.size.z, _minDistBetweenSectors);

            List<Vector2> pts = sampler.Sample(_numberOfSectors + 1);

            _masterSector = pts.OrderBy(e => Random.value).First();
            pts.Remove(_masterSector);
            _sectorLocations = pts;

            GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foreach (Vector2 p in pts)
            {
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                obj.transform.position = new Vector3(Mathf.Floor(p.x / GridSize) * GridSize, Mathf.Floor(_height / GridSize) * GridSize, Mathf.Floor(p.y / GridSize) * GridSize);
                obj.GetComponent<MeshRenderer>().material.color = Color.blue;

            }
            GameObject master = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            master.transform.position = new Vector3(Mathf.Floor(_masterSector.x / GridSize) * GridSize, Mathf.Floor(_height / GridSize) * GridSize, Mathf.Floor(_masterSector.y / GridSize) * GridSize);
            master.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        private void GeneratePipes()
        {

        }

        private void Awake()
        {
            SpawnSectors();
        }
    }
}
