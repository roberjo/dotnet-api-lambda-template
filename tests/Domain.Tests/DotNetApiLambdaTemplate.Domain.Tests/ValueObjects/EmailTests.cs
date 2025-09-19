using DotNetApiLambdaTemplate.Domain.ValueObjects;
using FluentValidation;
using Xunit;

namespace DotNetApiLambdaTemplate.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user+tag@example.org")]
    [InlineData("user123@test-domain.com")]
    public void From_WithValidEmail_ReturnsEmailObject(string validEmail)
    {
        // Act
        var email = Email.Create(validEmail);

        // Assert
        Assert.Equal(validEmail, email.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user@.com")]
    [InlineData("user..name@example.com")]
    [InlineData("user@example..com")]
    public void From_WithInvalidEmail_ThrowsValidationException(string invalidEmail)
    {
        // Act & Assert
        Assert.Throws<ValidationException>(() => Email.Create(invalidEmail));
    }

    [Fact]
    public void From_WithNullEmail_ThrowsValidationException()
    {
        // Act & Assert
        Assert.Throws<ValidationException>(() => Email.Create(null!));
    }

    [Fact]
    public void From_WithEmailTooLong_ThrowsValidationException()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@example.com"; // Over 255 characters

        // Act & Assert
        Assert.Throws<ValidationException>(() => Email.Create(longEmail));
    }

    [Fact]
    public void ImplicitConversion_FromString_ReturnsEmailObject()
    {
        // Arrange
        string emailString = "test@example.com";

        // Act
        Email email = Email.Create(emailString);

        // Assert
        Assert.Equal(emailString, email.Value);
    }

    [Fact]
    public void ImplicitConversion_FromEmail_ReturnsString()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        string emailString = email;

        // Assert
        Assert.Equal("test@example.com", emailString);
    }

    [Fact]
    public void ToString_ReturnsEmailValue()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        // Act
        var result = email.ToString();

        // Assert
        Assert.Equal("test@example.com", result);
    }

    [Fact]
    public void Equality_TwoEmailsWithSameValue_AreEqual()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Act & Assert
        Assert.Equal(email1, email2);
        Assert.True(email1 == email2);
        Assert.False(email1 != email2);
    }

    [Fact]
    public void Equality_TwoEmailsWithDifferentValues_AreNotEqual()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");

        // Act & Assert
        Assert.NotEqual(email1, email2);
        Assert.False(email1 == email2);
        Assert.True(email1 != email2);
    }

    [Fact]
    public void GetHashCode_TwoEmailsWithSameValue_HaveSameHashCode()
    {
        // Arrange
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("test@example.com");

        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_TwoEmailsWithDifferentValues_HaveDifferentHashCodes()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com");
        var email2 = Email.Create("test2@example.com");

        // Act
        var hashCode1 = email1.GetHashCode();
        var hashCode2 = email2.GetHashCode();

        // Assert
        Assert.NotEqual(hashCode1, hashCode2);
    }
}