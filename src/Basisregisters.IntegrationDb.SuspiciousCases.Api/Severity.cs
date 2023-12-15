namespace Basisregisters.IntegrationDb.SuspiciousCases.Api
{
    using System.Runtime.Serialization;

    /// <summary>
    /// De ernst van het verdacht geval.
    /// </summary>
    [DataContract(Name = "Ernst", Namespace = "")]
    public enum Severity
    {
        /// <summary>
        /// Foutief.
        /// </summary>
        [DataMember(Name = "foutief")]
        [EnumMember]
        Incorrect = 1,

        /// <summary>
        /// Verdacht.
        /// </summary>
        [DataMember(Name = "verdacht")]
        [EnumMember]
        Suspicious = 2,

        /// <summary>
        /// Verbeterbaar.
        /// </summary>
        [DataMember(Name = "verbeterbaar")]
        [EnumMember]
        Improveable = 3
    }
}
