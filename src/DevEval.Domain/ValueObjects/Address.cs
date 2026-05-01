using DevEval.Common;

namespace DevEval.Domain.ValueObjects
{
    /// <summary>
    /// Represents the address details of a user as a value object.
    /// </summary>
    public class Address : ValueObject
    {
        /// <summary>
        /// Gets the city of the address.
        /// </summary>
        public string City { get; }

        /// <summary>
        /// Gets the street of the address.
        /// </summary>
        public string Street { get; }

        /// <summary>
        /// Gets the house or building number of the address.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Gets the ZIP code of the address.
        /// </summary>
        public string ZipCode { get; }

        /// <summary>
        /// Gets the geolocation details of the address.
        /// </summary>
        public Geolocation? Geolocation { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class.
        /// </summary>
        /// <param name="city">The city of the address.</param>
        /// <param name="street">The street of the address.</param>
        /// <param name="number">The house or building number of the address.</param>
        /// <param name="zipCode">The ZIP code of the address.</param>
        /// <param name="geolocation">The geolocation of the address.</param>
        public Address(string city, string street, int number, string zipCode, Geolocation geolocation)
        {
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty.", nameof(city));

            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street cannot be null or empty.", nameof(street));

            if (number <= 0)
                throw new ArgumentException(nameof(number), "Number must be greater than zero.");

            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("ZIP code cannot be null or empty.", nameof(zipCode));

            Geolocation = geolocation ?? throw new ArgumentNullException(nameof(geolocation));

            City = city;
            Street = street;
            Number = number;
            ZipCode = zipCode;
        }

        private Address()
        {
            City = string.Empty; 
            Street = string.Empty; 
            ZipCode = string.Empty; 
            Geolocation = Geolocation.Empty;
        }

        /// <summary>
        /// Represents an empty address instance.
        /// </summary>
        public static Address Empty => new Address("N/A", "N/A", 1, "N/A", Geolocation.Empty);

        /// <summary>
        /// Overrides the ToString method to display the address as a formatted string.
        /// </summary>
        public override string ToString() =>
            $"{Street}, {Number}, {City} - {ZipCode} (Geolocation: {Geolocation?.Lat}, {Geolocation?.Long})";

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Address other)
            {
                return City == other.City &&
                       Street == other.Street &&
                       Number == other.Number &&
                       ZipCode == other.ZipCode &&
                       Equals(Geolocation, other.Geolocation);
            }

            return false;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        public override int GetHashCode() => HashCode.Combine(City, Street, Number, ZipCode, Geolocation);

        /// <summary>
        /// Defines the components of equality for the value object.
        /// </summary>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return City;
            yield return Street;
            yield return Number;
            yield return ZipCode;
            yield return Geolocation ?? new Geolocation(0.0, 0.0);
        }
    }
}
