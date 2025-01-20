using System;
using Newtonsoft.Json;

public class Passport
{
    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("divCode")]
    public string DivisionCode { get; set; }

    [JsonProperty("series")]
    public int Series { get; set; }

    [JsonProperty("number")]
    public int Number { get; set; }

    [JsonProperty("sex")]
    public bool Sex { get; set; }

    [JsonProperty("givenBy")]
    public string GivenBy { get; set; }

    [JsonProperty("givenDate")]
    public DateTime GivenDate { get; set; }

    public static Passport Invalid()
    {
        var p = new Passport();
        p.Id = -1;

        return p;
    }

    public static bool IsInvalid(Passport passport)
    {
        return passport.Id == -1;
    }
}
