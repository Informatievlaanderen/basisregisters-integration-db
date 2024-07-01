﻿namespace Basisregisters.IntegrationDb.Meldingen.Notifications
{
    using Newtonsoft.Json;

    public class NotificationMessage
    {
        [JsonIgnore]
        public string MessageType { get; }
        public string BasisregistersError { get; }
        public string Service { get; }
        public string Warning { get; }

        public NotificationMessage(string messageType, string basisregistersError, string service, NotificationSeverity warning)
        {
            MessageType = messageType;
            BasisregistersError = basisregistersError;
            Service = service;
            Warning = warning.ToString().ToLowerInvariant();
        }
    }

    public enum NotificationSeverity
    {
        Good,
        Low,
        Danger
    }
}
