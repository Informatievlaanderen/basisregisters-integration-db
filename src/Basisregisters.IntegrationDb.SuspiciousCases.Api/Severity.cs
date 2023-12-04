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
        [EnumMember]
        Incorrect = 1,

        /// <summary>
        /// Verdacht.
        /// </summary>
        [EnumMember]
        Suspicious = 2,

        /// <summary>
        /// Verbeterbaar.
        /// </summary>
        [EnumMember]
        Improveable = 3
    }
}
