using System;
using System.Linq;

namespace BadmintonLt.Integration.Players.Crawler.Domain.Entities
{
    public class Gender
    {
        public int NumericEquivalent { get; }

        public static Gender Male = new Gender(1);

        public static Gender Female = new Gender(2);

        private Gender(int numericEquivalent)
        {
            NumericEquivalent = numericEquivalent;
        }

        public static Gender CreateFrom(int numericEquivalent)
        {
            if (numericEquivalent < 1 || numericEquivalent > 2)
            {
                throw new ArgumentException(
                    "Not supported gender, should be value [1, 2].", 
                    nameof(numericEquivalent));
            }

            return numericEquivalent == 1 ? Male : Female;
        }
    }

    public class Player : IEquatable<Player>
    {
        public string ExternalId { get; }

        public Gender Gender { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string ProfileUrl { get; }

        public PlayerClub Club { get; }

        public Player(
            string externalId,
            Gender gender, 
            string firstName, 
            string lastName, 
            string profileUrl,
            PlayerClub club)
        {
            if (string.IsNullOrWhiteSpace(externalId))
            {
                throw new ArgumentNullException(nameof(externalId));
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (string.IsNullOrWhiteSpace(profileUrl))
            {
                throw new ArgumentNullException(nameof(profileUrl));
            }

            Club = club ?? throw new ArgumentNullException(nameof(club));

            FirstName = firstName;
            LastName = lastName;
            ProfileUrl = profileUrl;
            Gender = gender;
            ExternalId = externalId;

        }

        public override int GetHashCode()
        {
            return _fullName.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as Player);
        }

        public bool Equals(Player other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return false;
            if (GetType() != other.GetType()) return false;

            return FirstName == other.FirstName
                   && LastName == other.LastName
                   && ProfileUrl == other.ProfileUrl;
        }

        public static bool operator ==(Player current, Player other)
        {
            if (ReferenceEquals(null, current)) return false;

            return current.Equals(other);
        }

        public static bool operator !=(Player current, Player other)
        {
            return !(current == other);
        }

        private string _fullName
            => $"{FirstName} {LastName}";

        private string[] ParseNameEntry(string name)
            => name.Split(new[] {' '}, 2);


        public override string ToString()
            => $"{_fullName} {ProfileUrl}";
    }
}