namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection(CollectionState.SharedTestDbServer)]
public class RegisterRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task RegisterRepository_Upsert_returns_created()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var registerSet = await SeedRegisterSet(device, "TestRegisterSet");
        var registerTemplate = await SeedRegisterTemplate(registerSet, "RegisterTemplateIdentifier");

        var sut = new RegisterRepository(GetDbProvider());

        // Act
        var register = await sut.Upsert(registerTemplate.Id, "Subset");

        // Assert
        Assert.NotEqual(0, register.Id);
        Assert.NotEqual(0, register.RegisterTemplateId);
        Assert.Equal("Subset", register.Subset);
        Assert.Equal(registerTemplate.Id, register.RegisterTemplateId);
    }

    [Fact]
    public async Task RegisterRepository_Get_returns_single()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var registerSet = await SeedRegisterSet(device, "TestRegisterSet");
        var registerTemplate = await SeedRegisterTemplate(registerSet, "RegisterTemplateIdentifier");
        await SeedRegister(registerTemplate, "Subset");

        var sut = new RegisterRepository(GetDbProvider());

        // Act
        var register = await sut.GetByTemplateAndSubset(registerTemplate.Id, "Subset");

        // Assert
        Assert.NotNull(register);
        Assert.NotEqual(0, register.Id);
        Assert.Equal(registerTemplate.Id, register.RegisterTemplateId);
        Assert.Equal("Subset", register.Subset);
    }
}
