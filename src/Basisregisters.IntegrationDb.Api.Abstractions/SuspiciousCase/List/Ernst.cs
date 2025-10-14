namespace Basisregisters.IntegrationDb.Api.Abstractions.SuspiciousCase.List
{
    using System.Runtime.Serialization;

    /// <summary>
    /// De ernst van het verdacht geval.
    /// </summary>
    [DataContract(Name = "Ernst", Namespace = "")]
    public enum Ernst
    {
        /// <summary>
        /// Foutief.
        /// </summary>
        [EnumMember]
        Foutief = 1,

        /// <summary>
        /// Verdacht.
        /// </summary>
        [EnumMember]
        Verdacht = 2
    }
}
