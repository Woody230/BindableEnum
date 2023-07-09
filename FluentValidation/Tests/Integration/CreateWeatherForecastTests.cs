﻿using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Woody230.FluentValidation.Web.Models;
using Xunit;

namespace Woody230.FluentValidation.Tests.Integration;

/// <summary>
/// Represents tests for the <see cref="WeatherForecastController"/> related to creating a new weather forecast.
/// </summary>
public class CreateWeatherForecastTests : IntegrationTests
{
    /// <summary>
    /// The serializer options.
    /// </summary>
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWeatherForecastTests"/> class.
    /// </summary>
    /// <param name="factory">The web application factory.</param>
    public CreateWeatherForecastTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    /// <summary>
    /// Verifies that validation passes when a valid model is used.
    /// </summary>
    [Fact]
    public async Task Create_WithValidForecast_ReturnsOk()
    {
        // Arrange
        var forecast = new WeatherForecast()
        {
            TemperatureC = 32,
            RequiredEvent = new Event()
            {
                Description = "* Foo",
                Name = "* Bar"
            },
            Events = new List<Event>()
            {
                new Event()
                {
                    Description = "* Fizz",
                    Name = "* Buzz"
                }
            }
        };

        var request = CreateRequest(forecast);

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        scope.AddReportable("Content", content);

        var model = JsonSerializer.Deserialize<WeatherForecast>(content, _options);
        model.Should().BeEquivalentTo(forecast);
    }

    /// <summary>
    /// Verifies that validation fails when an invalid model is used.
    /// </summary>
    [Fact]
    public async Task Create_WithInvalidForecast_ReturnsBadRequest()
    {
        // Arrange
        var forecast = new WeatherForecast()
        {
            TemperatureC = -1,
            OptionalEvent = new Event(),
            RequiredEvent = new Event()
            {
                Description = "Foo",
                Name = "* Bar"
            },
            Events = new List<Event>()
            {
                new Event()
                {
                    Description = "* Fizz",
                    Name = "Buzz"
                }
            }
        };

        var request = CreateRequest(forecast);

        // Act
        var response = await HttpClient.SendAsync(request);

        // Assert
        using var scope = new AssertionScope();
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var content = await response.Content.ReadAsStringAsync();
        scope.AddReportable("Content", content);

        content.Should().Be("'Description' must not be empty.\r\n'Description' must start with *\r\n'Name' must start with *\r\n'Temperature C' must be greater than '0'.");
    }

    /// <summary>
    /// Creates the request.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>The request.</returns>
    private HttpRequestMessage CreateRequest(WeatherForecast forecast)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "WeatherForecast");
        request.Content = new StringContent(JsonSerializer.Serialize(forecast, _options));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return request;
    }
}