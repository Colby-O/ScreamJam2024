using BeneathTheSurface.MonoSystems;
using PlazmaGames.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace BeneathTheSurface.Helpers
{
    public enum CellType
    {
        Empty,
        Used
    }

    public class PathFinder
    {
        private Dictionary<Vector2Int, Vector2Int> _cameFrom;

        private static readonly Vector2Int[] DIRECTIONS = new[]
        {
            new Vector2Int(0, 1),
            new Vector2Int(1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0)
        }; 
       

        private List<Vector2Int> GetNeighbors(Vector2Int current)
        {
            List<Vector2Int> neighbors = new();

            foreach (Vector2Int dir in DIRECTIONS)
            {
                Vector2Int newGridTile = current + dir;
                neighbors.Add(newGridTile);
            }

            return neighbors;
        }

        private float GetManhattanDistance(Vector2Int goal, Vector2Int next)
        {
            return Mathf.Abs(goal.x - next.x) + Mathf.Abs(goal.y - next.y);
        }

        private float GetCurrentCost(Vector2Int previous, Vector2Int next)
        {
            if (GameManager.GetMonoSystem<IBuildingMonoSystem>().HasPipeAt(new Vector3Int(next.x, GameManager.GetMonoSystem<IPipeSystemMonoSystem>().GetGridHeight(), next.y))) return 0.0f;
            return 10f;
        }

        public Dictionary<Vector2Int, Vector2Int> FindOptimalPath(Vector2Int start, Vector2Int goal)
        {
            PriorityQueue<Vector2Int, float> fontier = new();
            Dictionary<Vector2Int, float> costSoFar = new();
            Dictionary<Vector2Int, Vector2Int> _cameFrom = new();

            fontier.Enqueue(start, 0.0f);
            costSoFar[start] = 0.0f;
            while (fontier.Count != 0)
            {
                Vector2Int current = fontier.Dequeue();
                Vector2Int previous = (_cameFrom.ContainsKey(current)) ? _cameFrom[current] : current;

                if (current == goal) break;

                foreach (Vector2Int next in GetNeighbors(current))
                {
                    float newCost = costSoFar[current] + GetCurrentCost(previous, next);
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        Debug.Log("Here!");
                        costSoFar[next] = newCost;
                        float priority = newCost + GetManhattanDistance(goal, next);
                        fontier.Enqueue(next, priority);
                        _cameFrom[next] = current;
                    }
                }
            }

            this._cameFrom = _cameFrom;

            return _cameFrom;
        }

        public void DrawPath(Vector2Int start, Color? color = null, float duration = Mathf.Infinity)
        {
            Vector2Int pt = start;
            while (true)
            {
                if (!_cameFrom.ContainsKey(pt)) break;

                Vector3 worldPT = new Vector3(pt.x, GameManager.GetMonoSystem<IPipeSystemMonoSystem>().GetGridHeight(), pt.y) * GameManager.GetMonoSystem<IBuildingMonoSystem>().GetGridSize();
                Vector3 nextWorldPT = new Vector3(_cameFrom[pt].x, GameManager.GetMonoSystem<IPipeSystemMonoSystem>().GetGridHeight(), _cameFrom[pt].y) * GameManager.GetMonoSystem<IBuildingMonoSystem>().GetGridSize();

                Debug.DrawLine(worldPT, nextWorldPT, color ?? Color.magenta, duration);
                pt = _cameFrom[pt];
            }
        }
    }
}
