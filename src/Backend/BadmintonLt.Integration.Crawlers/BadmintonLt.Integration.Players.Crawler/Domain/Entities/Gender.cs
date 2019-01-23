using System;

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
}