﻿using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using BusV.Data.Entities;
using MongoDB.Driver;

namespace BusV.Data
{
    /// <inheritdoc />
    public class BusStopRepo : IBusStopRepo
    {
        private FilterDefinitionBuilder<BusStop> Filter => Builders<BusStop>.Filter;

        private readonly IMongoCollection<BusStop> _collection;

        /// <inheritdoc />
        public BusStopRepo(
            IMongoCollection<BusStop> collection
        )
        {
            _collection = collection;
        }

        /// <inheritdoc />
        public async Task<Error> AddAsync(
            BusStop busStop,
            CancellationToken cancellationToken = default
        )
        {
            Error error;
            try
            {
                await _collection.InsertOneAsync(busStop, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                error = null;
            }
            catch (MongoWriteException e)
                when (e.WriteError.Category == ServerErrorCategory.DuplicateKey &&
                      e.WriteError.Message.Contains($" index: ")
                )
            {
                string index = Regex.Match(e.WriteError.Message, @" index: (\w+) ", RegexOptions.IgnoreCase).Value;
                error = new Error("data.duplicate_key", $@"Duplicate key: ""{index}""");
            }

            return error;
        }

        /// <inheritdoc />
        public async Task<Error[]> AddAllAsync(
            IEnumerable<BusStop> busStops,
            CancellationToken cancellationToken = default
        )
        {
            var errors = new List<Error>();
            try
            {
                await _collection.InsertManyAsync(busStops, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (MongoBulkWriteException e)
            {
                foreach (var writeError in e.WriteErrors)
                {
                    Error error;
                    if (writeError.Category == ServerErrorCategory.DuplicateKey)
                    {
                        error = new Error("data.duplicate_key", writeError.Message);
                    }
                    else
                    {
                        error = new Error("data", writeError.Message);
                    }

                    errors.Add(error);
                }
            }

            return errors.Any() ? errors.ToArray() : null;
        }

        /// <inheritdoc />
        public async Task<BusStop> FindClosestToLocationAsync(
            double longitude,
            double latitude,
            string[] busStopTags,
            CancellationToken cancellationToken = default
        )
        {
            var filter = Filter.And(
                Filter.In(s => s.Tag, busStopTags),
                Filter.NearSphere(s => s.Location, longitude, latitude)
            );

            var closestStop = await _collection
                .Find(filter)
                .Limit(1)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return closestStop;
        }
    }
}
