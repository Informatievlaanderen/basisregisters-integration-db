namespace Basisregisters.IntegrationDb.Gtmf.Meldingen.Api.Events
{
    using System.Collections.Generic;

    public static class MeldingEventTypes
    {
        public const string MeldingAangemaaktEvent = "MeldingAangemaaktEvent";
        public const string MeldingIngediendEvent = "MeldingIngediendEvent";
        public const string MeldingsobjectInitieelToegewezenAanBehandelaarEvent = "MeldingsobjectInitieelToegewezenAanBehandelaarEvent";
        public const string MeldingsobjectInitieelToegewezenAanBronhouderEvent = "MeldingsobjectInitieelToegewezenAanBronhouderEvent";
        public const string MeldingToegewezenEvent = "MeldingToegewezenEvent";
        public const string MeldingsobjectGeslotenNaToewijzingAlsBronhouderEvent = "MeldingsobjectGeslotenNaToewijzingAlsBronhouderEvent";
        public const string MeldingsobjectInOnderzoekEvent = "MeldingsobjectInOnderzoekEvent";
        public const string MeldingsobjectAfgewezenNaOnderzoekAlsBehandelaarEvent = "MeldingsobjectAfgewezenNaOnderzoekAlsBehandelaarEvent";
        public const string MeldingsobjectGeslotenNaOnderzoekAlsBehandelaarEvent = "MeldingsobjectGeslotenNaOnderzoekAlsBehandelaarEvent";
        public const string MeldingsobjectGeslotenNaOnderzoekAlsBronhouderEvent = "MeldingsobjectGeslotenNaOnderzoekAlsBronhouderEvent";
        public const string MeldingsobjectOpgelostEvent = "MeldingsobjectOpgelostEvent";
        public const string MeldingsobjectGeslotenEvent = "MeldingsobjectGeslotenEvent";
        public const string MeldingAfgerondEvent = "MeldingAfgerondEvent";

        public static IEnumerable<string> All =>
            [
                MeldingAangemaaktEvent,
                MeldingIngediendEvent,
                MeldingsobjectInitieelToegewezenAanBehandelaarEvent,
                MeldingsobjectInitieelToegewezenAanBronhouderEvent,
                MeldingToegewezenEvent,
                MeldingsobjectGeslotenNaToewijzingAlsBronhouderEvent,
                MeldingsobjectInOnderzoekEvent,
                MeldingsobjectAfgewezenNaOnderzoekAlsBehandelaarEvent,
                MeldingsobjectGeslotenNaOnderzoekAlsBehandelaarEvent,
                MeldingsobjectGeslotenNaOnderzoekAlsBronhouderEvent,
                MeldingsobjectOpgelostEvent,
                MeldingsobjectGeslotenEvent,
                MeldingAfgerondEvent
            ];

        public static IEnumerable<string> MeldingsObjectEvents =>
            [
                MeldingsobjectInitieelToegewezenAanBehandelaarEvent,
                MeldingsobjectInitieelToegewezenAanBronhouderEvent,
                MeldingsobjectGeslotenNaToewijzingAlsBronhouderEvent,
                MeldingsobjectInOnderzoekEvent,
                MeldingsobjectAfgewezenNaOnderzoekAlsBehandelaarEvent,
                MeldingsobjectGeslotenNaOnderzoekAlsBehandelaarEvent,
                MeldingsobjectGeslotenNaOnderzoekAlsBronhouderEvent,
                MeldingsobjectOpgelostEvent,
                MeldingsobjectGeslotenEvent
            ];
    }
}
