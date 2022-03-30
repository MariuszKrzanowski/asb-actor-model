using Microsoft.Extensions.DependencyInjection;
using MrMatrix.Net.ActorOnServiceBus.Conventions;
using MrMatrix.Net.ActorOnServiceBus.Messages.Dtos;
using MrMatrix.Net.ActorOnServiceBus.Messages.Transforms;

namespace MrMatrix.Net.ActorOnServiceBus.Messages
{
    public static class MessagesBootstrap
    {
        public static IServiceCollection ConfigureMessages(this IServiceCollection services)
        {
            services.AddSingleton<IMessageSerializationHelper, MessageSerializationHelper>();
            return services;
        }

        public static IServiceCollection RegisterMessage<TMessage>(this IServiceCollection services)
        {
            services.AddTransient<IMessageDescriptor, MessageDescriptor<TMessage>>();
            return services;
        }

        public static IServiceCollection ConfigureAllMessages(this IServiceCollection services)
        {
            services
                .ConfigureMessages()
                .RegisterMessage<NecessityDto>()
                .RegisterMessage<DonationDto>()
                .RegisterMessage<BalanceDto>()
                .RegisterMessage<BalanceQueryDto>()
                .RegisterMessage<BalanceResponseDto>()
                .RegisterMessage<BalancedNeedDto>()
                ;
            return services;
        }
    }
}