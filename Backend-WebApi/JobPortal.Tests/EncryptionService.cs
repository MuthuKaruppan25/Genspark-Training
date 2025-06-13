using Xunit;
using System.Text;
using System.Threading.Tasks;
using JobPortal.Models;

public class EncryptionServiceTests
{
    [Fact]
    public async Task EncryptData_WithKey_ProducesConsistentHash()
    {
        // Arrange
        var service = new EncryptionService();
        var key = Encoding.UTF8.GetBytes("test-key-1234567890");
        var model = new EncryptModel
        {
            Data = "password",
            HashKey = key
        };

        // Act
        var result1 = await service.EncryptData(new EncryptModel { Data = model.Data, HashKey = key });
        var result2 = await service.EncryptData(new EncryptModel { Data = model.Data, HashKey = key });

        // Assert
        Assert.NotNull(result1.EncryptedData);
        Assert.Equal(result1.EncryptedData, result2.EncryptedData);
        Assert.Equal(key, result1.HashKey);
    }

    [Fact]
    public async Task EncryptData_WithoutKey_GeneratesKeyAndHash()
    {
        // Arrange
        var service = new EncryptionService();
        var model = new EncryptModel
        {
            Data = "password",
            HashKey = null
        };

        // Act
        var result = await service.EncryptData(model);

        // Assert
        Assert.NotNull(result.EncryptedData);
        Assert.NotNull(result.HashKey);
        Assert.NotEmpty(result.HashKey);
    }

    [Fact]
    public async Task EncryptData_DifferentData_ProducesDifferentHash()
    {
        // Arrange
        var service = new EncryptionService();
        var key = Encoding.UTF8.GetBytes("test-key-1234567890");

        // Act
        var result1 = await service.EncryptData(new EncryptModel { Data = "password1", HashKey = key });
        var result2 = await service.EncryptData(new EncryptModel { Data = "password2", HashKey = key });

        // Assert
        Assert.NotEqual(result1.EncryptedData, result2.EncryptedData);
    }
}