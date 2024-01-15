using System;

using Dapr.Actors.Runtime;

using NamingLib;

namespace phone2Name
{
    public class NameGenerator : Actor, INameGen, IRemindable
    {
        private const string NameGeneratorSwitchReminder = @"NameGeneratorSwitchReminder";
        private readonly ILogger<NameGenerator> _logger;
        private readonly NumSystemEnum _startEnum;
        private readonly Func<NumSystemEnum, INumMapService> _numServFactory;
        private INumMapService _numService;
        private IActorReminder? _registeredReminder = null;

        public NameGenerator(ActorHost host, ILogger<NameGenerator> logger, Func<NumSystemEnum, INumMapService> numServFactory) : base(host)
        {
            _logger = logger;
            _startEnum = NumSystemEnum.English;
            _numServFactory = numServFactory;
            _numService = numServFactory(_startEnum);

        }

        protected override Task OnPreActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            _logger.LogInformation($"PreActorMethod {actorMethodContext.MethodName}, Id:{Id}");
            return base.OnPreActorMethodAsync(actorMethodContext);
        }
        protected override Task OnActivateAsync()
        {
            _logger.LogInformation($"{nameof(NameGenerator)} activated, Id:{Id}");
            return base.OnActivateAsync();
        }

        protected override Task OnDeactivateAsync()
        {
            _logger.LogInformation($"{nameof(NameGenerator)} Deactivated, Id:{Id}");
            return base.OnDeactivateAsync();
        }

        protected override Task OnPostActorMethodAsync(ActorMethodContext actorMethodContext)
        {
            _logger.LogInformation($"PostActorMethod {actorMethodContext.MethodName}, Id:{Id}");
            return base.OnPostActorMethodAsync(actorMethodContext);
        }

        protected override Task OnActorMethodFailedAsync(ActorMethodContext actorMethodContext, Exception e)
        {
            _logger.LogError(e, $"ActorMethodFailed {actorMethodContext.MethodName} , Id:{Id}");
            return base.OnActorMethodFailedAsync(actorMethodContext, e);
        }
        public async Task<IEnumerable<string>> GenerateNamesAsync(string numbers)
        {
            var chars = numbers.ToCharArray();
            List<string> final = new List<string> { "" };
            _logger.LogInformation($"received {numbers} to convert, Id:{Id}");
            foreach (var achar in chars)
            {
                var options = _numService.Map(achar);
                final = (from opt in options
                         from fin in final
                         select $"{fin}{opt}").ToList();
            }

            if (_registeredReminder == null)
                _registeredReminder = await RegisterReminderAsync(NameGeneratorSwitchReminder, null, TimeSpan.FromMinutes(0), TimeSpan.FromMinutes(2));
            _logger.LogInformation($"converted to {final.Count} names, Id:{Id}");

            return final;

        }

        public Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            var newEnum = (NumSystemEnum)((((int)_startEnum) + 1) % Enum.GetNames(typeof(NumSystemEnum)).Length);
            _numService = _numServFactory(newEnum);
            return Task.CompletedTask;
        }

    }
}
