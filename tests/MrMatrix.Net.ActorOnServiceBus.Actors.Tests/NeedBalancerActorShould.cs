using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MrMatrix.Net.ActorOnServiceBus.Actors.Actors;
using MrMatrix.Net.ActorOnServiceBus.Actors.Sagas;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace MrMatrix.Net.ActorOnServiceBus.Actors.Tests;

public class NeedBalancerActorShould
{
    private readonly ILogger<NeedBalancerActor> _logger;
    private const string NecessityKey = "t-shirt";
    public NeedBalancerActorShould()
    {
        var loggerMock = new Mock<ILogger<NeedBalancerActor>>(MockBehavior.Loose);
        _logger = loggerMock.Object;
    }

    [Fact]
    public async Task RegisterNewDonationInEmptySaga()
    {
        var saga = new NeedsBalancerSaga();

        CancellationToken ctx = CancellationToken.None;

        var network = new Mock<IActorsNetwork<NeedsBalancerSaga>>(MockBehavior.Loose);
        network
            .Setup(m => m.Saga)
            .Returns(saga);

        var sut = new NeedBalancerActor(network.Object, _logger);
        await sut.Handle(new DonationDto()
        {
            DonorId = "donor",
            Quantity = 2,
            Key = NecessityKey
        }, ctx);

        saga.Key.Should().Be(NecessityKey);
        saga.Necessities.Should().HaveCount(0);
        saga.Donations.Should().HaveCount(1);
        saga.Donations[0].Quantity.Should().Be(2);
        saga.Donations[0].PersonId.Should().Be("donor");
    }

    [Fact]
    public async Task RegisterNewNecessityInEmptySaga()
    {
        var saga = new NeedsBalancerSaga();

        CancellationToken ctx = CancellationToken.None;

        var network = new Mock<IActorsNetwork<NeedsBalancerSaga>>(MockBehavior.Loose);
        network
            .Setup(m => m.Saga)
            .Returns(saga);

        var sut = new NeedBalancerActor(network.Object, _logger);
        await sut.Handle(new NecessityDto()
        {
            NecessitousId = "necessitous",
            Quantity = 2,
            Key = NecessityKey
        }, ctx);

        saga.Key.Should().Be(NecessityKey);
        saga.Necessities.Should().HaveCount(1);
        saga.Donations.Should().HaveCount(0);
        saga.Necessities[0].Quantity.Should().Be(2);
        saga.Necessities[0].PersonId.Should().Be("necessitous");
    }

    [Fact]
    public async Task RemoveExistingDonationWhenMatchingNecessityArrives()
    {
        var saga = new NeedsBalancerSaga();

        CancellationToken ctx = CancellationToken.None;

        var network = new Mock<IActorsNetwork<NeedsBalancerSaga>>(MockBehavior.Loose);
        network
            .Setup(m => m.Saga)
            .Returns(saga);

        var sut = new NeedBalancerActor(network.Object, _logger);
        await sut.Handle(new DonationDto()
        {
            DonorId = "donor",
            Quantity = 2,
            Key = NecessityKey
        }, ctx);
        await sut.Handle(new NecessityDto()
        {
            NecessitousId = "necessitous",
            Quantity = 2,
            Key = NecessityKey
        }, ctx);

        saga.Key.Should().Be(NecessityKey);
        saga.Necessities.Should().HaveCount(0);
        saga.Donations.Should().HaveCount(0);
    }

    [Fact]
    public async Task RemoveExistingNecessityWhenMatchingDonationArrives()
    {
        var saga = new NeedsBalancerSaga();

        CancellationToken ctx = CancellationToken.None;

        var network = new Mock<IActorsNetwork<NeedsBalancerSaga>>(MockBehavior.Loose);
        network
            .Setup(m => m.Saga)
            .Returns(saga);

        var sut = new NeedBalancerActor(network.Object, _logger);
        await sut.Handle(new NecessityDto()
        {
            NecessitousId = "necessitous",
            Quantity = 2,
            Key = NecessityKey
        }, ctx);
        await sut.Handle(new DonationDto()
        {
            DonorId = "donor",
            Quantity = 2,
            Key = NecessityKey
        }, ctx);

        saga.Key.Should().Be(NecessityKey);
        saga.Necessities.Should().HaveCount(0);
        saga.Donations.Should().HaveCount(0);
    }

    [Fact]
    public async Task DecrementExistingNecessityWhenDonationArrivesWithLowerQuantity()
    {
        var saga = new NeedsBalancerSaga();

        CancellationToken ctx = CancellationToken.None;

        var network = new Mock<IActorsNetwork<NeedsBalancerSaga>>(MockBehavior.Loose);
        network
            .Setup(m => m.Saga)
            .Returns(saga);

        var sut = new NeedBalancerActor(network.Object, _logger);
        await sut.Handle(new NecessityDto()
        {
            NecessitousId = "necessitous",
            Quantity = 2,
            Key = NecessityKey
        }, ctx);
        await sut.Handle(new DonationDto()
        {
            DonorId = "donor",
            Quantity = 1,
            Key = NecessityKey
        }, ctx);

        saga.Key.Should().Be(NecessityKey);
        saga.Necessities.Should().HaveCount(1);
        saga.Donations.Should().HaveCount(0);
        saga.Necessities[0].Quantity.Should().Be(1);
        saga.Necessities[0].PersonId.Should().Be("necessitous");
    }

    [Fact]
    public async Task DecrementExistingDonationWhenMatchingNecessityArrivesWithLowerQuantity()
    {
        var saga = new NeedsBalancerSaga();

        CancellationToken ctx = CancellationToken.None;

        var network = new Mock<IActorsNetwork<NeedsBalancerSaga>>(MockBehavior.Loose);
        network
            .Setup(m => m.Saga)
            .Returns(saga);

        var sut = new NeedBalancerActor(network.Object, _logger);
        await sut.Handle(new DonationDto()
        {
            DonorId = "donor",
            Quantity = 2,
            Key = NecessityKey
        }, ctx);
        await sut.Handle(new NecessityDto()
        {
            NecessitousId = "necessitous",
            Quantity = 1,
            Key = NecessityKey
        }, ctx);

        saga.Key.Should().Be(NecessityKey);
        saga.Necessities.Should().HaveCount(0);
        saga.Donations.Should().HaveCount(1);
        saga.Donations[0].Quantity.Should().Be(1);
        saga.Donations[0].PersonId.Should().Be("donor");
    }
}