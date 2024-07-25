namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection("SharedTestDbServer")]
public class RegisterTemplateRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task RegisterTemplateRepository_Upsert_returns_created()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var registerSet = await SeedRegisterSet(device, "RegisterSetIdentifier");

        var sut = new RegisterTemplateRepository(GetDbProvider());

        // Act
        var registerTemplate = await sut.Upsert(device.Id, "RegisterTemplateIdentifier");

        // Assert
        Assert.NotEqual(0, registerTemplate.Id);
        Assert.NotEqual(0, registerTemplate.RegisterSetId);
        Assert.Equal(registerSet.Id, registerTemplate.RegisterSetId);
        Assert.Equal("RegisterTemplateIdentifier", registerTemplate.Identifier);
    }

    [Fact]
    public async Task RegisterTemplateRepository_Get_returns_single()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var registerSet = await SeedRegisterSet(device, "RegisterSetIdentifier");
        await SeedRegisterTemplate(registerSet, "RegisterTemplateIdentifier");

        var sut = new RegisterTemplateRepository(GetDbProvider());

        // Act
        var registerTemplate = await sut.GetRegisterTemplate(registerSet.Id, "RegisterTemplateIdentifier");

        // Assert
        Assert.NotNull(registerTemplate);
        Assert.NotEqual(0, registerTemplate.Id);
        Assert.Equal(registerSet.Id, registerTemplate.RegisterSetId);
        Assert.Equal("RegisterTemplateIdentifier", registerTemplate.Identifier);
    }
}
