using BeneathTheSurface.Events;
using BeneathTheSurface.Helpers;
using BeneathTheSurface.Wielding;
using PlazmaGames.Core;
using PlazmaGames.Core.Events;
using PlazmaGames.ProGen.Sampling;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
    public class Sector
    {
        public GameObject obj;
        public Vector3Int pos;

        public Sector(GameObject obj, Vector3Int pos)
        {
            this.obj = obj;
            this.pos = pos;
        }
    }

    public class PipeSystemMonoSystem : MonoBehaviour, IPipeSystemMonoSystem
    {
        [SerializeField] private int _numberOfSectors;
        [SerializeField] private BoundsInt _bounds;
        [SerializeField] private float _height;
        [SerializeField] private float _minDistBetweenSectors = 10;
        [SerializeField] private float _amountOfBreaks = 5;
        [SerializeField] private int _borkenRange = 5;

        [Header("Pipe Prefabs")]
        [SerializeField] private Pipe _striaght;
        [SerializeField] private Pipe _corner;
        [SerializeField] private Pipe _threeway;
        [SerializeField] private Pipe _fourway;

        private List<Vector2> _sectorLocations;
        private Vector2 _masterSector;

        private List<Sector> _sectors;

        List<Pipe> _pipeVarients;

        private static readonly Vector2Int[] DIRECTIONS = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        };

        private static readonly Vector3Int[] DIRECTIONS3D = new[]
{
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        private float GridSize => GameManager.GetMonoSystem<IBuildingMonoSystem>().GetGridSize();
        private int Height => GetGridHeight();

        private bool _tutorialDone = false;

        public void CheckConnections()
        {
            Stack<Vector3Int> positions = new Stack<Vector3Int>();
            List<Vector3Int> visted = new List<Vector3Int>();
            positions.Push(new Vector3Int(Mathf.FloorToInt(_masterSector.x), Height, Mathf.FloorToInt(_masterSector.y)));
            visted.Add(new Vector3Int(Mathf.FloorToInt(_masterSector.x), Height, Mathf.FloorToInt(_masterSector.y)));
            while (positions.Count > 0)
            {
                Vector3Int cur = positions.Pop();

                foreach (Vector2Int dir in DIRECTIONS)
                {
                    Vector3Int next = new Vector3Int(cur.x + dir.x, cur.y, cur.z + dir.y);
                    if (GameManager.GetMonoSystem<IBuildingMonoSystem>().HasPipeAt(next) && !visted.Contains(next))
                    {
                        GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(next).GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                        positions.Push(next);
                        visted.Add(next);

                        if (_sectors.Where(e => e.pos == next).Count() > 0) _sectors.Where(e => e.pos == next).First().obj.GetComponent<MeshRenderer>().material.color = Color.green;
                    }
                }
            }
        }

        public bool CheckTutorialConnections()
        {

            Pipe t1 = GameObject.FindWithTag("PipeT1").GetComponent<Pipe>();
            Pipe t2 = GameObject.FindWithTag("PipeT2").GetComponent<Pipe>();

            Stack<Vector3Int> positions = new Stack<Vector3Int>();
            List<Vector3Int> visted = new List<Vector3Int>();
            positions.Push(t1.GetGridPosition());
            visted.Add(t1.GetGridPosition());
            while (positions.Count > 0)
            {
                Vector3Int cur = positions.Pop();

                foreach (Vector3Int dir in DIRECTIONS3D)
                {
                    Vector3Int next = cur + dir;
                    if (GameManager.GetMonoSystem<IBuildingMonoSystem>().HasPipeAt(next) && GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(next).IsWielded() && !visted.Contains(next))
                    {
                        positions.Push(next);
                        visted.Add(next);

                        if (next == t2.GetGridPosition()) return true && BeneathTheSurfaceGameManager.eventProgresss == 3;
                    }
                }
            }

            return false;
        }

        private Pipe InstantiatePipePrefab(Pipe obj, float y)
        {
            return Instantiate(obj, Vector3.zero, Quaternion.Euler(0f, y, 90f));
        }

        private void GeneratePipeVarients()
        {
            _pipeVarients = new()
            {
                null,
                InstantiatePipePrefab(_striaght, 180f),
                InstantiatePipePrefab(_striaght, 90f),
                InstantiatePipePrefab(_corner, 270f),
                InstantiatePipePrefab(_striaght, 0f),
                InstantiatePipePrefab(_striaght, 0f),
                InstantiatePipePrefab(_corner, 0f),
                InstantiatePipePrefab(_threeway, 0f),
                InstantiatePipePrefab(_striaght, 270f),
                InstantiatePipePrefab(_corner, 180f),
                InstantiatePipePrefab(_striaght, 90f),
                InstantiatePipePrefab(_threeway, 270f),
                InstantiatePipePrefab(_corner, 90f),
                InstantiatePipePrefab(_threeway, 180f),
                InstantiatePipePrefab(_threeway, 90f),
                InstantiatePipePrefab(_fourway, 0f)
            };

            // Hides each of the pipe vairents in the scene
            foreach (Pipe pipe in _pipeVarients)
            {
                if (pipe != null)
                {
                    pipe.transform.parent = transform;
                    pipe.gameObject.SetActive(false);
                }
            }
        }

        public float GetHeight()
        {
            return _height;
        }

        public int GetGridHeight()
        {
            return Mathf.FloorToInt(_height / GameManager.GetMonoSystem<IBuildingMonoSystem>().GetGridSize());
        }

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
                obj.transform.localScale *= GridSize;
                obj.GetComponent<MeshRenderer>().material.color = Color.blue;
                _sectors.Add(new Sector(obj, new Vector3Int(Mathf.FloorToInt(p.x / GridSize), Mathf.FloorToInt(_height / GridSize), Mathf.FloorToInt(p.y / GridSize))));

            }
            GameObject master = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            master.transform.position = new Vector3(Mathf.Floor(_masterSector.x / GridSize) * GridSize, Mathf.Floor(_height / GridSize) * GridSize, Mathf.Floor(_masterSector.y / GridSize) * GridSize);
            master.transform.localScale *= GridSize;
            master.GetComponent<MeshRenderer>().material.color = Color.red;
        }

        private int GetPipeOrention(Vector3Int pos)
        {
            int key = 0;
            int i = 0;

            foreach (Vector2Int dir in DIRECTIONS)
            {
                if (
                    GameManager.GetMonoSystem<IBuildingMonoSystem>().HasPipeAt(new Vector3Int(pos.x + dir.x, pos.y, pos.z + dir.y))
                ) key |= 1 << i;
                i += 1;
            }

            return key;
        }

        private void PlaceSector(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start)
        {
            Vector2Int gridPT = start;
            while (true)
            {
                GameManager.GetMonoSystem<IBuildingMonoSystem>().AddPipe(null, new Vector3Int(gridPT.x, Height, gridPT.y));
                if (!cameFrom.ContainsKey(gridPT)) break;
                else gridPT = cameFrom[gridPT];
            }
        }

        private void GeneratePipes()
        {
            PathFinder pf = new PathFinder();

            foreach (Vector2 p in _sectorLocations)
            {
                Dictionary<Vector2Int, Vector2Int> path = pf.FindOptimalPath(
                    new Vector2Int(Mathf.FloorToInt(_masterSector.x / GridSize), Mathf.FloorToInt(_masterSector.y / GridSize)),
                    new Vector2Int(Mathf.FloorToInt(p.x / GridSize), Mathf.FloorToInt(p.y / GridSize))
                );
                PlaceSector(path, new Vector2Int(Mathf.FloorToInt(p.x / GridSize), Mathf.FloorToInt(p.y / GridSize)));
                //pf.DrawPath(new Vector2Int(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y)));
            }
        }

        private void PlacePipes()
        {
            List<Vector3Int> locs = GameManager.GetMonoSystem<IBuildingMonoSystem>().GetAllPipesLocations();
            foreach (Vector3Int loc in locs)
            {
                int key = GetPipeOrention(loc);
                if (key == 0) continue;
                Pipe pipe = _pipeVarients[key];

                Vector3 position = new Vector3(loc.x, loc.y, loc.z) * GridSize;

                Pipe pipeObject = Instantiate(pipe, position, pipe.transform.rotation);
                pipeObject.SetWeldedState(true);

                if (_sectorLocations.Where(e => new Vector3Int(Mathf.FloorToInt(e.x / GridSize), Height, Mathf.FloorToInt(e.y / GridSize)) == loc).Count() > 0 || new Vector3Int(Mathf.FloorToInt(_masterSector.x / GridSize), Height, Mathf.FloorToInt(_masterSector.y / GridSize)) == loc)
                {
                    pipeObject.SetExemptState(true);
                }
                else
                {
                    pipeObject.SetExemptState(false);
                }

                pipeObject.gameObject.SetActive(true);
                GameManager.GetMonoSystem<IBuildingMonoSystem>().SetPipeAt(pipeObject, loc);
            }

            for (int i = 0; i < _amountOfBreaks; i++)
            {
                Vector3Int loc = locs.OrderByDescending(e => Mathf.Min(Vector3Int.Distance(e, _sectors.OrderBy(s => Vector3Int.Distance(s.pos, e)).First().pos), Vector3.Distance(e, new Vector3Int(Mathf.FloorToInt(_masterSector.x), Height, Mathf.FloorToInt(_masterSector.y)))) * Random.value).Where(e => GameManager.GetMonoSystem<IBuildingMonoSystem>().HasPipeAt(e) && _sectors.Where(s => s.pos == e).Count() == 0 && e != new Vector3Int(Mathf.FloorToInt(_masterSector.x), Height, Mathf.FloorToInt(_masterSector.y))).First();

                Pipe pipe = GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(loc);

                pipe.SetWeldedState(false);

                Vector3Int newPos = new Vector3Int(loc.x + Random.Range(-_borkenRange, _borkenRange), loc.y, loc.z + Random.Range(-_borkenRange, _borkenRange));

                while (GameManager.GetMonoSystem<IBuildingMonoSystem>().HasPipeAt(newPos))
                {
                    newPos = new Vector3Int(loc.x + Random.Range(-_borkenRange, _borkenRange), loc.y, loc.z + Random.Range(-_borkenRange, _borkenRange));
                }

                GameManager.GetMonoSystem<IBuildingMonoSystem>().RemovePipe(loc);

                pipe.transform.position = new Vector3(newPos.x, newPos.y, newPos.z) * GridSize;
            }
        }

        private void Awake()
        {
            _tutorialDone = false;
            _sectors = new List<Sector>();
            GeneratePipeVarients();
            SpawnSectors();
            GeneratePipes();
            PlacePipes();

            CheckConnections();
        }

        private void Update()
        {
            if (!_tutorialDone)
            {
                _tutorialDone = CheckTutorialConnections();
                if (_tutorialDone) GameManager.EmitEvent(new BSEvents.FinishedPipeTutorial());
            }
            CheckConnections();
        }
    }
}
