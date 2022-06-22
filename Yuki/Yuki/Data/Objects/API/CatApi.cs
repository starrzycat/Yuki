using Newtonsoft.Json;
using System;

namespace Yuki.Data.Objects.API
{
    public partial class CatJson
    {
        [JsonProperty("breeds")]
        public Breed[] Breeds { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }
    }

    public partial class Breed
    {
        [JsonProperty("weight")]
        public Weight Weight { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("cfa_url")]
        public Uri CfaUrl { get; set; }

        [JsonProperty("vetstreet_url")]
        public Uri VetstreetUrl { get; set; }

        [JsonProperty("vcahospitals_url")]
        public Uri VcahospitalsUrl { get; set; }

        [JsonProperty("temperament")]
        public string Temperament { get; set; }

        [JsonProperty("origin")]
        public string Origin { get; set; }

        [JsonProperty("country_codes")]
        public string CountryCodes { get; set; }

        [JsonProperty("country_code")]
        public string CountryCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("life_span")]
        public string LifeSpan { get; set; }

        [JsonProperty("indoor")]
        public long Indoor { get; set; }

        [JsonProperty("lap")]
        public long Lap { get; set; }

        [JsonProperty("alt_names")]
        public string AltNames { get; set; }

        [JsonProperty("adaptability")]
        public long Adaptability { get; set; }

        [JsonProperty("affection_level")]
        public long AffectionLevel { get; set; }

        [JsonProperty("child_friendly")]
        public long ChildFriendly { get; set; }

        [JsonProperty("dog_friendly")]
        public long DogFriendly { get; set; }

        [JsonProperty("energy_level")]
        public long EnergyLevel { get; set; }

        [JsonProperty("grooming")]
        public long Grooming { get; set; }

        [JsonProperty("health_issues")]
        public long HealthIssues { get; set; }

        [JsonProperty("intelligence")]
        public long Intelligence { get; set; }

        [JsonProperty("shedding_level")]
        public long SheddingLevel { get; set; }

        [JsonProperty("social_needs")]
        public long SocialNeeds { get; set; }

        [JsonProperty("stranger_friendly")]
        public long StrangerFriendly { get; set; }

        [JsonProperty("vocalisation")]
        public long Vocalisation { get; set; }

        [JsonProperty("experimental")]
        public long Experimental { get; set; }

        [JsonProperty("hairless")]
        public long Hairless { get; set; }

        [JsonProperty("natural")]
        public long Natural { get; set; }

        [JsonProperty("rare")]
        public long Rare { get; set; }

        [JsonProperty("rex")]
        public long Rex { get; set; }

        [JsonProperty("suppressed_tail")]
        public long SuppressedTail { get; set; }

        [JsonProperty("short_legs")]
        public long ShortLegs { get; set; }

        [JsonProperty("wikipedia_url")]
        public Uri WikipediaUrl { get; set; }

        [JsonProperty("hypoallergenic")]
        public long Hypoallergenic { get; set; }
    }

    public partial class Weight
    {
        [JsonProperty("imperial")]
        public string Imperial { get; set; }

        [JsonProperty("metric")]
        public string Metric { get; set; }
    }
}
