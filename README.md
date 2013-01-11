# Windows Azure Store integration in CSharp #

This is sample of how to integrate with the [Windows Azure Store](http://www.windowsazure.com/en-us/store/overview/) from MetricsHub, Inc.

- Official documentation from Microsoft is [here](https://github.com/WindowsAzure/azure-resource-provider-sdk)
- You need SQL Azure database. You can create new database, schema will be created automatically. Please check 'AzureStoreRP.Configuration.AccountDataProviderConnectionString' setting.
- For production environment you need to use https and validate client certificaete. Please check 'AzureStoreRP.Configuration.AzureStoreRequestAuthorization' setting.
- In order to validate client certificate you need to accept it on IIS level. Please check web.config for more details.

Resource manifest for this sample:

```xml
<ResourceManifest>
    <Test>
        <ResourceProviderEndpoint>http://127.0.0.1:8080/api/azurestore/</ResourceProviderEndpoint>
        <ResourceProviderSsoEndpoint>http://127.0.0.1:81/azurestore/SingleSignOn</ResourceProviderSsoEndpoint>
    </Test>
    <Prod>
        <ResourceProviderEndpoint>http://azurestore-rp.cloudapp.net:8080/api/azurestore/</ResourceProviderEndpoint>
        <ResourceProviderSsoEndpoint>http://azurestore-rp.cloudapp.net:80/azurestore/SingleSignOn</ResourceProviderSsoEndpoint>
    </Prod>
    <OutputKeys>
    </OutputKeys>
</ResourceManifest>
```

