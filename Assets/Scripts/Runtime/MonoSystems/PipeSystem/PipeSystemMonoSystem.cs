using BeneathTheSurface.Events;
using BeneathTheSurface.Helpers;
using BeneathTheSurface.Wielding;
using PlazmaGames.Attribute;
using PlazmaGames.Core;
using PlazmaGames.Core.Events;
using PlazmaGames.ProGen.Sampling;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace BeneathTheSurface.MonoSystems
{
	public class Sector
	{
		public SectorObject obj;
		public Vector3Int pos;

		public Sector(SectorObject obj, Vector3Int pos)
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

		[Header("Sector Prefabs")]
		[SerializeField] private SectorObject _sectorPrefab;
		[SerializeField] private SectorObject _mainSectorPrefab;

		[Header("Kelp Spawner")]
        [SerializeField] private float _oceanFloorHeight;
        [SerializeField] private GameObject _kelpPrefab;
        [SerializeField] private Vector2 _kelpHeightRange;
        [SerializeField] private float _distanceBetweenKelp;

        [Header("Pipes")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _numOfExtraPipesToSpawn;

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

		private bool _isSolved = false;

		[SerializeField, ReadOnly] private int _numberOfActiveSectors;
        [SerializeField, ReadOnly] private int _targetNumberOfActiveSectors;

        private void SpawnKelp()
		{
            PoissonSampler sampler = new PoissonSampler(_bounds.size.x, _bounds.size.z, _distanceBetweenKelp);

            foreach (Vector2 pt in sampler.Sample())
			{
				GameObject kelp = Instantiate(_kelpPrefab, new Vector3(pt.x - _bounds.size.x / 2.0f, _oceanFloorHeight, pt.y - _bounds.size.z / 2.0f), Quaternion.Euler(0, Random.Range(0f, 360f), 0));
				kelp.transform.localScale = new Vector3(kelp.transform.localScale.x, Random.Range(_kelpHeightRange.x, _kelpHeightRange.y), kelp.transform.localScale.z);
            }
        }

		public void CheckConnections()
		{
			foreach (Vector3Int loc in GameManager.GetMonoSystem<IBuildingMonoSystem>().GetAllPipesLocations())
			{
				GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(loc).DisableIcon();
                GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(loc).GetComponentInChildren<MeshRenderer>().material.color = Color.white;

            }
			foreach (Sector s in _sectors) s.obj.Disable();

            _targetNumberOfActiveSectors = _sectors.Count;
            _numberOfActiveSectors = 0;

            Stack <Vector3Int> positions = new Stack<Vector3Int>();
			List<Vector3Int> visted = new List<Vector3Int>();
			positions.Push(new Vector3Int(Mathf.FloorToInt(_masterSector.x / GridSize), Height, Mathf.FloorToInt(_masterSector.y / GridSize)));
			visted.Add(new Vector3Int(Mathf.FloorToInt(_masterSector.x / GridSize), Height, Mathf.FloorToInt(_masterSector.y / GridSize)));
			while (positions.Count > 0)
			{
				Vector3Int cur = positions.Pop();

				foreach (Vector2Int dir in DIRECTIONS)
				{
					Vector3Int next = new Vector3Int(cur.x + dir.x, cur.y, cur.z + dir.y);
					if (GameManager.GetMonoSystem<IBuildingMonoSystem>().HasPipeAt(next) && GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(next).IsWielded() && !visted.Contains(next))
					{
						positions.Push(next);
						visted.Add(next);

						if (_sectors.Where(e => e.pos == next).Count() > 0)
						{
                            _numberOfActiveSectors++;
							_sectors.Where(e => e.pos == next).First().obj.Enable();
						}
						else GameManager.GetMonoSystem<IBuildingMonoSystem>().GetPipeAt(next).EnableIcon();
                    }
				}
			}

			_isSolved = _numberOfActiveSectors == _targetNumberOfActiveSectors;
		}

		public bool CheckTutorialConnections()
		{
			Pipe t1 = GameObject.FindWithTag("PipeT1").GetComponent<Pipe>();
			Pipe t2 = GameObject.FindWithTag("PipeT2").GetComponent<Pipe>();
			Debug.Log(t1.name + " : " + t2.name);
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

			for (int i = 0; i < pts.Count; i++) pts[i] -= new Vector2(_bounds.size.x / 2.0f, _bounds.size.z / 2.0f);

			_masterSector = pts.OrderBy(e => Random.value).First();
			pts.Remove(_masterSector);
			_sectorLocations = pts;

			GameObject prefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			foreach (Vector2 p in pts)
			{
                SectorObject obj = Instantiate(_sectorPrefab);
				obj.transform.position = new Vector3(Mathf.Floor(p.x / GridSize) * GridSize, Mathf.Floor(_height / GridSize) * GridSize, Mathf.Floor(p.y / GridSize) * GridSize);
				_sectors.Add(new Sector(obj, new Vector3Int(Mathf.FloorToInt(p.x / GridSize), Mathf.FloorToInt(_height / GridSize), Mathf.FloorToInt(p.y / GridSize))));

			}
            SectorObject master = Instantiate(_mainSectorPrefab);
            master.transform.position = new Vector3(Mathf.Floor(_masterSector.x / GridSize) * GridSize, Mathf.Floor(_height / GridSize) * GridSize, Mathf.Floor(_masterSector.y / GridSize) * GridSize);
			master.Enable();
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

				pipe.transform.position = new Vector3(newPos.x, (_oceanFloorHeight + 2.5f) / GridSize, newPos.z) * GridSize;
			}
		}

		private void SpawnExtraPipe()
		{
            for (int i = 0; i < _numOfExtraPipesToSpawn; i++)
			{
				int key = Random.Range(1, _pipeVarients.Count);
				Vector3 position = new Vector3(Random.Range(-_bounds.size.x / 2.0f, _bounds.size.x / 2.0f), _oceanFloorHeight + 2.5f, Random.Range(-_bounds.size.z / 2.0f, _bounds.size.z / 2.0f));


                Pipe pipe = Instantiate(_pipeVarients[key], position, _pipeVarients[key].transform.rotation);
                pipe.SetWeldedState(false);
				pipe.gameObject.SetActive(true);
            }
        }

		private void Awake()
		{
			_isSolved = false;
            _sectors = new List<Sector>();
			GeneratePipeVarients();
			SpawnSectors();
			GeneratePipes();
			PlacePipes();

			CheckConnections();


            SpawnExtraPipe();

            SpawnKelp();
        }

        private void Start()
        {
            _tutorialDone = false;
        }

        private void Update()
		{
			if (_isSolved && BeneathTheSurfaceGameManager.eventProgresss == 7)
			{
                GameManager.EmitEvent(new BSEvents.FinishedPipes());
            }

			if (!_tutorialDone)
			{
				_tutorialDone = CheckTutorialConnections();
				if (_tutorialDone) GameManager.EmitEvent(new BSEvents.FinishedPipeTutorial());
			}

			if (!_isSolved) CheckConnections();
		}
	}
}
