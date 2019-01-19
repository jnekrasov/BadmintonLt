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
    }

    public class Player: IEquatable<Player>
    {
        public Gender Gender { get; }

        public string Name { get; }

        public string Surname { get; }

        public string ProfileUrl { get; }

        public string FullName => $"{Name} {Surname}";

        public Player(
            Gender gender,
            string name,
            string profileUrl)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(profileUrl))
            {
                throw new ArgumentNullException(nameof(profileUrl));
            }

            Gender = gender;

            var nameEntries = ParseNameEntry(name);
            Name = nameEntries.ElementAtOrDefault(0) ?? string.Empty;
            Surname = nameEntries.ElementAtOrDefault(1) ?? string.Empty;

            ProfileUrl = profileUrl;
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
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

            return Name == other.Name 
                   && Surname == other.Surname 
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

        private string[] ParseNameEntry(string name)
            => name.Split(new[] { ' ' }, 2);
    }
}