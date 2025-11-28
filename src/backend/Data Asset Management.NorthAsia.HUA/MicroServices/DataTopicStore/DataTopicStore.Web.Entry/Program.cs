AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

Serve.Run(RunOptions.Default.ConfigureBuilder(builder =>
{
    builder.UseSerilogDefault();
}).WithArgs(args));