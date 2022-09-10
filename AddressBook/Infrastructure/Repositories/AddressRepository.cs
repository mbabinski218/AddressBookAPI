﻿using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        // Variables:
        private readonly IMongoCollection<Address> _addresses;
        // TODO: add lock

        // Constructors:
        public AddressRepository(IConfiguration config)
        {
            var MongoDbConfig = config.GetSection("MongoDb");
            var mongoClient = new MongoClient(MongoDbConfig.GetSection("ConnectionString").Value);

            _addresses = mongoClient
                .GetDatabase(MongoDbConfig.GetSection("DatabaseName").Value)
                .GetCollection<Address>(MongoDbConfig.GetSection("CollectionName").Value);
        }

        // Methods:
        public Address? GetLastAdded()
        {
            return _addresses
                .Find(new BsonDocument())
                .ToList()
                .MaxBy(address => address.Created);
        }

        public async Task<Address?> GetLastAddedAsync()
        {
            var addresses = await _addresses.FindAsync(new BsonDocument());
            var addressesList = await addresses.ToListAsync();
            
            return await Task.Run(() =>
            {
                return addressesList.AsParallel().MaxBy(address => address.Created);
            });
        }

        public IEnumerable<Address>? GetByCity(string city)
        {
            var addressesList = _addresses.Find(new BsonDocument()).ToList();

            if (!addressesList.Any())
            {
                return null;
            }

            var output = addressesList.Where(address => address.City == city).ToHashSet();
            
            return output.Any() ? output : null;
        }
        
        public async Task<IEnumerable<Address>?> GetByCityAsync(string city)
        {
            var addresses = await _addresses.FindAsync(new BsonDocument());
            var addressesList = await addresses.ToListAsync();
            
            if (!addressesList.Any())
            {
                return null;
            }

            var output =  await Task.Run(() =>
            {
                return addressesList
                    .AsParallel()
                    .Where(address => address.City == city)
                    .ToHashSet();
            });
            
            return output.Any() ? output : null;
        }

        public Address? Add(Address address)
        {
            var addressesList = _addresses.Find(new BsonDocument()).ToList();
            
            if (addressesList.Contains(address))
            {
                return null;
            }

            _addresses.InsertOne(address);
            return address;
        }
        
        public async Task<Address?> AddAsync(Address address)
        {
            var addresses = await _addresses.FindAsync(new BsonDocument());
            var addressesList = await addresses.ToListAsync();
            
            if (addressesList.Contains(address))
            {
                return null;
            }

            await _addresses.InsertOneAsync(address);
            return address;
        }
    }
}