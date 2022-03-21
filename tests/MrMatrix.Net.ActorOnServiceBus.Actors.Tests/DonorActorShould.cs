using MrMatrix.Net.ActorOnServiceBus.Actors.Actors;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MrMatrix.Net.ActorOnServiceBus.Actors.Sagas;
using MrMatrix.Net.ActorOnServiceBus.ActorSystem.Interfaces;
using Xunit;

namespace MrMatrix.Net.ActorOnServiceBus.Actors.Tests
{
    public class DonorActorShould
    {
        private readonly ILogger<DonorActor> _logger;

        public DonorActorShould()
        {
            var loggerMock = new Mock<ILogger<DonorActor>>(MockBehavior.Loose);
            _logger = loggerMock.Object;
        }

        [Fact]
        public async Task StoreCurrentDonationInEmptySaga()
        {
            var saga = new DonationSaga();

            CancellationToken ctx = CancellationToken.None;

            var network = new Mock<IActorsNetwork<DonationSaga>>(MockBehavior.Loose);
            network
                .Setup(m => m.Saga)
                .Returns(saga);

            var sut = new DonorActor(network.Object, _logger);
            await sut.Handle(new DonationDto()
            {
                DonorId = "donor",
                Quantity = 2,
                Key = "t-shirt"
            }, ctx);

            saga.DonorId.Should().Be("donor");
            saga.Balance.Should().ContainKey("t-shirt");
            saga.Balance["t-shirt"].Donation.Should().Be(2);
            saga.Balance["t-shirt"].Necessity.Should().Be(0);
        }

        [Fact]
        public async Task IncreaseDonationQuantityForExistingItem()
        {
            // arrange
            var saga = new DonationSaga
            {
                Balance =
                {
                    ["socks"]=new NeedBalance { Necessity = 777, Donation =888 },
                    ["t-shirt"]=new NeedBalance { Necessity = 1000, Donation = 100 }
                }
            };

            CancellationToken ctx = CancellationToken.None;

            var network = new Mock<IActorsNetwork<DonationSaga>>(MockBehavior.Loose);
            network
                .Setup(m => m.Saga)
                .Returns(saga);

            var sut = new DonorActor(network.Object, _logger);

            // act
            await sut.Handle(new DonationDto
            {
                DonorId = "donor",
                Quantity = 2,
                Key = "t-shirt"
            }, ctx);

            // assert
            saga.DonorId.Should().Be("donor");
            saga.Balance.Should().ContainKey("t-shirt");
            saga.Balance.Should().ContainKey("socks");
            saga.Balance["t-shirt"].Donation.Should().Be(102);
            saga.Balance["t-shirt"].Necessity.Should().Be(1000);
            saga.Balance["socks"].Donation.Should().Be(888);
            saga.Balance["socks"].Necessity.Should().Be(777);
        }
    }
}
