﻿namespace Domain.Entities
{
    public class Address : IComparable<Address>
    {
        public int Id { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int HouseNumber { get; set; }
        public DateTime CreatedDateTime { get; init; }

        public Address(int id, string city, string street, int houseNumber)
        {
            (Id, City, Street, HouseNumber) = (id, city, street, houseNumber);
            CreatedDateTime = DateTime.UtcNow;
        }

        public int CompareTo(Address? address)
        {
            if (address == null) return 1;
            return City == address.City && Street == address.Street && HouseNumber == address.HouseNumber ? 0 : -1;
        }
    }
}
