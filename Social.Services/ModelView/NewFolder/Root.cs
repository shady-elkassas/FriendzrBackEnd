namespace Social.Services.ModelView{ 

    public class Root
    {
        public Embedded _embedded { get; set; }
        public Links _links { get; set; }
        public Page page { get; set; }
    }

}