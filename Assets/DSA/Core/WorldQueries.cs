#nullable enable

using System;
using System.Collections.Generic;

namespace FrontierDispatch.Core
{
    public sealed class WorldQueries
    {
        private readonly WorldData _world;
        private readonly Dictionary<int, Location> _locationsById;
        private readonly Dictionary<int, List<Connection>> _outgoingByLocationId;
        private static readonly IReadOnlyList<Connection> EmptyConnections = Array.Empty<Connection>();

        public WorldQueries(WorldData world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));

            _locationsById = new Dictionary<int, Location>(_world.Locations.Count);
            for (int i = 0; i < _world.Locations.Count; ++i)
            {
                Location location = _world.Locations[i];
                _locationsById[location.Id] = location;
            }

            _outgoingByLocationId = new Dictionary<int, List<Connection>>(_world.Locations.Count);
            for (int i = 0; i < _world.Connections.Count; ++i)
            {
                Connection connection = _world.Connections[i];
                if (!_outgoingByLocationId.TryGetValue(connection.FromLocationId, out List<Connection>? list))
                {
                    list = new List<Connection>();
                    _outgoingByLocationId[connection.FromLocationId] = list;
                }
                list.Add(connection);
            }
        }

        public Location? FindLocationById(int id)
        {
            return _locationsById.TryGetValue(id, out Location? location) ? location : null;
        }

        public IReadOnlyList<Connection> FindOutgoingConnections(int fromLocationId)
        {
            return _outgoingByLocationId.TryGetValue(fromLocationId, out List<Connection>? connections)
                ? connections
                : EmptyConnections;
        }
    }
}
