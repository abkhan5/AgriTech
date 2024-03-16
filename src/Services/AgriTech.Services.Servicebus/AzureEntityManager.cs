

namespace AgriTech.Services.Servicebus;

internal class AzureEntityManager : IQueueOperations
{
    private readonly IConfiguration configuration;
    private readonly ILogger logger;
    private readonly IOptions<AzureServiceBusSettings> serviceBusSettings;

    public AzureEntityManager(IOptions<AzureServiceBusSettings> serviceBusSettings, IConfiguration configuration,
        ILogger<AzureEntityManager> logger)
    {
        this.serviceBusSettings = serviceBusSettings;
        this.configuration = configuration;
        this.logger = logger;
    }


    private string Envname => configuration.GetSection("aksenvname").Value ?? "local";

    private bool UseMachineName
    {
        get
        {
            _ = bool.TryParse(configuration.GetSection("useMachineName").Value, out var result);
            return result;
        }
    }

    public async Task DeleteAll(CancellationToken cancellation)
    {
        var serviceBusAdministrationClient = GetClient();
        var allQueues = serviceBusAdministrationClient.GetQueuesAsync(cancellation);

        var ctr = 1;
        await foreach (var item in allQueues)
        {
            logger.LogWarning($"{ctr++}.Deleting {item.Name}");
            var response = await serviceBusAdministrationClient.DeleteQueueAsync(item.Name, cancellation);
            logger.LogWarning(response.Status.ToString());
        }
    }

    public string GetLocalizedName(string queueName)
    {
        queueName = queueName.ToLower();
        if (queueName.StartsWith(Envname + "-"))
            return queueName;
        queueName = Envname + "-" + queueName;

        if (UseMachineName)
            queueName = queueName + "-" + Environment.MachineName.ToLower();

        return queueName;
    }

    private ServiceBusAdministrationClient GetClient() =>
        new(serviceBusSettings.Value.ConnectionString);



    public async Task CreateQueue(string queueName, CancellationToken cancellation)
    {
        queueName = GetLocalizedName(queueName);
        var serviceBusAdministrationClient = GetClient();
        var queueExists = await serviceBusAdministrationClient.QueueExistsAsync(queueName, cancellation);
        if (!queueExists)
            await serviceBusAdministrationClient.CreateQueueAsync(new CreateQueueOptions(queueName)
            {
                AutoDeleteOnIdle = new TimeSpan(1, 0, 0, 0),
                DeadLetteringOnMessageExpiration = true,
                DefaultMessageTimeToLive = new TimeSpan(24, 0, 0),
                Status = EntityStatus.Active,
                //EnableBatchedOperations = true,
                UserMetadata = Envname
            }, cancellation);
    }

    public async Task CreateTopic(string topicName, CancellationToken cancellationToken)
    {
        var client = GetClient();
        var topicExists = await client.TopicExistsAsync(topicName, cancellationToken);
        if (topicExists)
            return;
        var topicOptions = new CreateTopicOptions(topicName)
        {
            AutoDeleteOnIdle = TimeSpan.FromDays(7),
            DefaultMessageTimeToLive = TimeSpan.FromDays(2),
            DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(1),
            EnableBatchedOperations = true,
            EnablePartitioning = false,
            MaxSizeInMegabytes = 2048,
            RequiresDuplicateDetection = true,
            Name = topicName,
            Status = EntityStatus.Active
        };
        await client.CreateTopicAsync(topicOptions, cancellationToken);

    }
    public async Task CreateTopicSubscription(string topicName, string subscriptionName, string forwardingQueueName, IDictionary<string, string> customRules, CancellationToken cancellationToken)
    {
        var client = GetClient();
        subscriptionName = GetLocalizedName(subscriptionName);
        var subscriptionOptions = new CreateSubscriptionOptions(topicName, subscriptionName)
        {
            AutoDeleteOnIdle = TimeSpan.FromDays(7),
            DefaultMessageTimeToLive = TimeSpan.FromDays(2)
        };

        if (!string.IsNullOrEmpty(forwardingQueueName))
            subscriptionOptions.ForwardTo = forwardingQueueName;

        var topicSubExists = await client.SubscriptionExistsAsync(topicName, subscriptionName, cancellationToken);
        if (topicSubExists)
            await client.DeleteSubscriptionAsync(topicName, subscriptionName, cancellationToken);

        await client.CreateSubscriptionAsync(subscriptionOptions, cancellationToken);

        if (customRules is { Count: > 0 })
            await CreateRules(topicName, subscriptionName, customRules, cancellationToken);
    }

    private async Task CreateRules(string topicName, string subscriptionName, IDictionary<string, string> custumRules, CancellationToken cancellationToken)
    {
        try
        {
            var client = GetClient();

            var rules = client.GetRulesAsync(topicName, subscriptionName, cancellationToken);
            var ruleProperties = new List<RuleProperties>();
            await foreach (var rule in rules)
                ruleProperties.Add(rule);


            foreach (var rule in ruleProperties)
                await client.DeleteRuleAsync(topicName, subscriptionName, rule.Name, cancellationToken);

            foreach (var rule in custumRules)
                await client.CreateRuleAsync(topicName, subscriptionName, new CreateRuleOptions
                {
                    Name = rule.Key,
                    Filter = new SqlRuleFilter(rule.Value),
                }, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex.ToString());
        }
    }
}