namespace Covid19.Library
{
    public class Coordinates
    {
        public string LocationName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"{LocationName}: {Latitude}, {Longitude}";
        }
    }
}
