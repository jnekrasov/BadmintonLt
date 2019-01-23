using System;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;

namespace BadmintonLt.Integration.Players.Crawler.Domain.Entities
{
    public class Player : IEquatable<Player>
    {
        public string ExternalId { get; }

        public Gender Gender { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string ProfileUrl { get; private set; }

        public PlayerClub ClubInformation { get; private set; }

        public (string, string) MergedExternalId => (ClubInformation.ExternalId, ExternalId);

        public string InternalId { get; private set; }

        public Player(
            string externalId,
            Gender gender, 
            string firstName, 
            string lastName, 
            string profileUrl,
            PlayerClub club,
            string internalId = null)
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

            ClubInformation = club ?? throw new ArgumentNullException(nameof(club));

            ExternalId = externalId;
            FirstName = firstName;
            LastName = lastName;
            ProfileUrl = profileUrl;
            Gender = gender;
            InternalId = internalId ?? Guid.NewGuid().ToString("D");
        }

        public void UpdateFrom(Player other)
        {
            InternalId = other.InternalId;
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

        public override string ToString()
            => $"{_fullName} {ProfileUrl}";
    }
}