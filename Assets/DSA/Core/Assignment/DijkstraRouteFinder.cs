using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    // Assignment Phase 4.

    public sealed class DijkstraRouteFinder
    {
        private readonly WorldData _world;
        private readonly WorldQueries _queries;

        public DijkstraRouteFinder(WorldData world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _queries = new WorldQueries(_world);
        }

        public RouteResult FindRoute(int startLocationId, int goalLocationId)
        {
            // Edge case: verify both locations exist in the world
            if (_queries.FindLocationById(startLocationId) == null ||
                _queries.FindLocationById(goalLocationId) == null)
            {
                return new RouteResult(false, Array.Empty<int>(), 0, 0);
            }

            var bestCost = new Dictionary<int, int>();
            var cameFrom = new Dictionary<int, int>();
            var visited = new HashSet<int>();
            var frontier = new MinPriorityQueue<int>();

            bestCost[startLocationId] = 0;
            frontier.Enqueue(startLocationId, 0);

            int expandedCount = 0;

            while (frontier.Count > 0)
            {
                int currentId = frontier.Dequeue();

                // Skip if we already processed this location via a cheaper path
                if (!visited.Add(currentId))
                {
                    continue;
                }

                int currentCost = bestCost[currentId];

                // Check if we reached the goal
                if (currentId == goalLocationId)
                {
                    List<int> path = ReconstructPath(cameFrom, startLocationId, goalLocationId);
                    return new RouteResult(true, path, currentCost, expandedCount);
                }

                expandedCount++;

                IReadOnlyList<Connection> outgoing = _queries.FindOutgoingConnections(currentId);
                for (int i = 0; i < outgoing.Count; ++i)
                {
                    Connection edge = outgoing[i];
                    int neighborId = edge.ToLocationId;
                    int newCost = currentCost + edge.TravelCost;

                    if (!bestCost.TryGetValue(neighborId, out int existingCost) || newCost < existingCost)
                    {
                        bestCost[neighborId] = newCost;
                        cameFrom[neighborId] = currentId;
                        frontier.Enqueue(neighborId, newCost);
                    }
                }
            }

            // Queue emptied without reaching the goal
            return new RouteResult(false, Array.Empty<int>(), 0, expandedCount);
        }

        private static List<int> ReconstructPath(Dictionary<int, int> cameFrom, int startId, int goalId)
        {
            var path = new List<int>();
            int current = goalId;

            while (current != startId)
            {
                path.Add(current);
                current = cameFrom[current];
            }

            path.Add(startId);
            path.Reverse();
            return path;
        }
    }
}
