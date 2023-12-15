namespace Basisregisters.IntegrationDb.SuspiciousCases.Api
{
    using System.Runtime.Serialization;

    /// <summary>
    /// De categorie van het verdacht geval.
    /// </summary>
    [DataContract(Name = "Categorie", Namespace = "")]
    public enum Category
    {
        /// <summary>
        /// Adres.
        /// </summary>
        [DataMember(Name = "adres")]
        [EnumMember]
        Address = 1,

        /// <summary>
        /// Perceel.
        /// </summary>
        [DataMember(Name = "perceel")]
        [EnumMember]
        Parcel = 2,

        /// <summary>
        /// Wegverbinding.
        /// </summary>
        [DataMember(Name = "wegsegment")]
        [EnumMember]
        RoadSegment = 3,

        /// <summary>
        /// Gebouw.
        /// </summary>
        [DataMember(Name = "gebouw")]
        [EnumMember]
        Building = 4,

        /// <summary>
        /// Gebouweenheid.
        /// </summary>
        [DataMember(Name = "gebouweenheid")]
        [EnumMember]
        Buildingunit = 5,

        /// <summary>
        /// Straatnaam.
        /// </summary>
        [DataMember(Name = "straatnaam")]
        [EnumMember]
        StreetName = 6
    }
}
