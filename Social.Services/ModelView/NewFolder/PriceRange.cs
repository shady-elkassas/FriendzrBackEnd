namespace Social.Services.ModelView{ 

    public class PriceRange
    {
        public string type { get; set; }
        public string currency { get; set; }
        public double min { get; set; }
        public double max { get; set; }
    }

}