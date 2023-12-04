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
        [EnumMember]
        Address = 1,

        /// <summary>
        /// Perceel.
        /// </summary>
        [EnumMember]
        Parcel = 2,

        /// <summary>
        /// Wegverbinding.
        /// </summary>
        [EnumMember]
        RoadSegment = 3,

        /// <summary>
        /// Gebouw.
        /// </summary>
        [EnumMember]
        Building = 4,

        /// <summary>
        /// Gebouweenheid.
        /// </summary>
        [EnumMember]
        Buildingunit = 5
    }
}
