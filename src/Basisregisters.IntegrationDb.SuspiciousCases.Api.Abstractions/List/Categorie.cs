namespace Basisregisters.IntegrationDb.SuspiciousCases.Api.Abstractions.List
{
    using System.Runtime.Serialization;

    /// <summary>
    /// De categorie van het verdacht geval.
    /// </summary>
    [DataContract(Name = "Categorie", Namespace = "")]
    public enum Categorie
    {
        /// <summary>
        /// Adres.
        /// </summary>
        [EnumMember]
        Adres = 1,

        /// <summary>
        /// Perceel.
        /// </summary>
        [EnumMember]
        Perceel = 2,

        /// <summary>
        /// Wegverbinding.
        /// </summary>
        [EnumMember]
        Wegsegment = 3,

        /// <summary>
        /// Gebouw.
        /// </summary>
        [EnumMember]
        Gebouw = 4,

        /// <summary>
        /// Gebouweenheid.
        /// </summary>
        [EnumMember]
        Gebouweenheid = 5,

        /// <summary>
        /// Straatnaam.
        /// </summary>
        [EnumMember]
        Straatnaam = 6
    }
}
