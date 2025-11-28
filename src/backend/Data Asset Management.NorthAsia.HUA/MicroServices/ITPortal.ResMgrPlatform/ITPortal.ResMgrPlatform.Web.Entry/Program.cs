Serve.Run(RunOptions.Default.WithArgs(args)
    .ConfigureInject((builder, options) =>
    {
        options.ConfigureAppConfiguration(ITPortal.Core.Extensions.AppApplicationBuilderExtensions.LoadConsulKeyValueConfig);
    }));