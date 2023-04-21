using System.Collections.Generic; 
namespace Social.Services.ModelView{ 

    public class ExternalLinks
    {
        public List<Youtube> youtube { get; set; }
        public List<Twitter> twitter { get; set; }
        public List<Itune> itunes { get; set; }
        public List<Lastfm> lastfm { get; set; }
        public List<Facebook> facebook { get; set; }
        public List<Wiki> wiki { get; set; }
        public List<Spotify> spotify { get; set; }
        public List<Musicbrainz> musicbrainz { get; set; }
        public List<Instagram> instagram { get; set; }
        public List<Homepage> homepage { get; set; }
    }

}