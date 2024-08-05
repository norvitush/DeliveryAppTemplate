using UnityEngine;

namespace GoldenSoft.Map.Common
{
    public class MapLocation
    {
        private const float EPSILON = 0.0005f;
        public double longitude;
        public double lattitude;

        public MapLocation() { }

        public MapLocation(double longitude, double latitude)
        {
            this.longitude = longitude;
            this.lattitude = latitude;
        }

        public bool IsEqual(double longitude, double latitude)
        {
            return Mathf.Abs((float)(this.longitude - longitude)) < EPSILON && Mathf.Abs((float)(this.lattitude - latitude)) < EPSILON;
        }
    }
}
