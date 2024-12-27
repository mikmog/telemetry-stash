namespace TelemetryStash.Database.Tests.Ts.TelemetryDatabase;

[Collection(CollectionState.SharedTestDbServer)]
public class RegisterRepositoryTests(SharedTestDbFixture dbFixture) : TelemetryDbSeeder(dbFixture)
{
    [Fact]
    public async Task RegisterRepository_GetOrCreate_creates_single_register()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var sut = new RegisterRepository(GetDbProvider());

        var registerSet = $"Registerset1{Guid.NewGuid()}";
        var registers = new HashSet<string> { $"Register1{Guid.NewGuid()}" };

        // Act
        var registersRows = await sut.GetOrCreate(device.Id, registerSet, registers);

        // Assert
        Assert.Single(registersRows);
        foreach (var row in registersRows)
        {
            Assert.NotEqual(0, row.Id);
            Assert.Equal(device.Id, row.DeviceId);
            Assert.Contains(row.Register, registers);
            Assert.Equal(registerSet, row.RegisterSet);
        }
    }

    [Fact]
    public async Task RegisterRepository_GetOrCreate_creates_multiple_registers()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var sut = new RegisterRepository(GetDbProvider());

        var registerSet = $"Registerset1{Guid.NewGuid()}";
        var registers = new HashSet<string> { $"Register1{Guid.NewGuid()}", $"Register2{Guid.NewGuid()}" };

        // Act
        var registersRows = await sut.GetOrCreate(device.Id, registerSet, registers);

        // Assert
        Assert.Equal(registers.Count, registersRows.Count);
        foreach (var row in registersRows)
        {
            Assert.NotEqual(0, row.Id);
            Assert.Equal(device.Id, row.DeviceId);
            Assert.Contains(row.Register, registers);
            Assert.Equal(registerSet, row.RegisterSet);
        }
    }

    [Fact]
    public async Task RegisterRepository_GetOrCreate_gets_multiple_registers()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var sut = new RegisterRepository(GetDbProvider());

        var registerSet = $"Registerset1{Guid.NewGuid()}";
        var registers = new HashSet<string> { $"Register1{Guid.NewGuid()}", $"Register2{Guid.NewGuid()}" };

        // Act
        var registersRowsCreate = await sut.GetOrCreate(device.Id, registerSet, registers);
        var registersRowsGet = await sut.GetOrCreate(device.Id, registerSet, registers);

        // Assert
        Assert.Equal(registersRowsCreate.Count, registersRowsGet.Count);

        foreach (var rowGet in registersRowsGet)
        {
            var rowCreate = registersRowsCreate.Single(r => r.Id == rowGet.Id);
            Assert.Equal(rowCreate.DeviceId, rowGet.DeviceId);
            Assert.Equal(rowCreate.Register, rowGet.Register);
            Assert.Equal(rowCreate.RegisterSet, rowGet.RegisterSet);
        }
    }

    [Fact]
    public async Task RegisterRepository_GetOrCreate_creates_handles_zero_registers()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var sut = new RegisterRepository(GetDbProvider());

        var registerSet = $"Registerset1{Guid.NewGuid()}";

        // Act
        var registersRows = await sut.GetOrCreate(device.Id, registerSet, []);

        // Assert
        Assert.Empty(registersRows);
    }

    [Fact]
    public async Task RegisterRepository_Get_returns_register()
    {
        // Arrange
        var device = await SeedDevice("TestDeviceId");
        var sut = new RegisterRepository(GetDbProvider());

        var registerSet = $"Registerset1{Guid.NewGuid()}";
        var registers = new HashSet<string> { $"Register1{Guid.NewGuid()}" };
        await sut.GetOrCreate(device.Id, registerSet, registers);

        // Act
        var register = await sut.GetSingle(device.Id, registerSet, registers.Single());

        // Assert
        Assert.NotNull(register);
        Assert.NotEqual(0, register.Id);
        Assert.Equal(device.Id, register.DeviceId);
        Assert.Contains(register.Register, registers);
        Assert.Equal(registerSet, register.RegisterSet);
    }
}
