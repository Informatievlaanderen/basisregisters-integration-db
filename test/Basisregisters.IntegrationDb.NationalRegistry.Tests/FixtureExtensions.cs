namespace Basisregisters.IntegrationDb.NationalRegistry.Tests
{
    using AutoFixture;
    using NetTopologySuite.Geometries;

    public static class FixtureExtensions
    {
        public static Fixture CustomizePoint(this Fixture fixture)
        {
            fixture.Customize<Point>(c =>
                c.FromFactory<object>(_ =>
                    new Point(
                        fixture.Create<double>(),
                        fixture.Create<double>()
                    )
                ).OmitAutoProperties());

            return fixture;
        }
    }
}
