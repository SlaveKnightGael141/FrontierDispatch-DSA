using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    // Assignment Phase 5. Adapt shortest-path reasoning to a travel-cost budget.

    public sealed class ReachabilityQuery
    {
        private readonly WorldData _world;
        private readonly WorldQueries _queries;

        public ReachabilityQuery(WorldData world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _queries = new WorldQueries(_world);
        }

        public IReadOnlyDictionary<int, int> FindReachable(int startLocationId, int maxTravelCost)
        {
            if (maxTravelCost < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxTravelCost), "Maximum travel cost cannot be negative.");
            }

            var bestCost = new Dictionary<int, int>();

            // Verify the starting location exists
            if (_queries.FindLocationById(startLocationId) == null)
            {
                return bestCost;
            }

            var visited = new HashSet<int>();
            var frontier = new MinPriorityQueue<int>();

            bestCost[startLocationId] = 0;
            frontier.Enqueue(startLocationId, 0);

            while (frontier.Count > 0)
            {
                int currentId = frontier.Dequeue();

                if (!visited.Add(currentId))
                {
                    continue;
                }

                int currentCost = bestCost[currentId];

                IReadOnlyList<Connection> outgoing = _queries.FindOutgoingConnections(currentId);
                for (int i = 0; i < outgoing.Count; ++i)
                {
                    Connection edge = outgoing[i];
                    int neighborId = edge.ToLocationId;
                    int newCost = currentCost + edge.TravelCost;

                    // Only explore neighbors that fit within our travel-cost budget
                    if (newCost <= maxTravelCost &&
                        (!bestCost.TryGetValue(neighborId, out int existingCost) || newCost < existingCost))
                    {
                        bestCost[neighborId] = newCost;
                        frontier.Enqueue(neighborId, newCost);
                    }
                }
            }

            return bestCost;
        }
    }
}
