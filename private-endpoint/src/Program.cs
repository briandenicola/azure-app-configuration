 void RunMain (Uri appConfigUri)
 {   
    var credential = new ChainedTokenCredential(new EnvironmentCredential(), new AzureCliCredential());
    var client = new ConfigurationClient(appConfigUri, credential);
    ConfigurationSetting setting = client.GetConfigurationSetting("sample");
    System.Console.WriteLine($"The value of \"sample\" retrieved from {appConfigUri} is \"{setting.Value}\"");
}

RootCommand command = new RootCommand("A basic example using Azure AppConfig")
{
    new Option<Uri>(
        aliases: new [] {"--app-config", "-a"},
        description: "App Config Uri or Name, e.g. my-appconfig or https://my-appconfig.azconfig.io",
        parseArgument: result =>
        {
            string value = result.Tokens.Single().Value;
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri appConfigUri) ||
                Uri.TryCreate($"https://{value}.azconfig.io", UriKind.Absolute, out appConfigUri))
            {
                return appConfigUri;
            }

            result.ErrorMessage = "Must specify a vault name or URI";
            return null!;
        }
    )
    {
        Name = "appConfigUri",
        IsRequired = true,
    }
};

command.Handler = CommandHandler.Create<Uri>(RunMain);
return command.Invoke(args);