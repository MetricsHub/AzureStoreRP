# Windows Azure Store integration in CSharp #

This is sample of how to integrate with the [Windows Azure Store](http://www.windowsazure.com/en-us/store/overview/) from MetricsHub, Inc.

- Official documentation from Microsoft is [here](https://github.com/WindowsAzure/azure-resource-provider-sdk)
- You need SQL Database. You can provision new database via Azure portal, schema will be created automatically. Please update connection string 'AzureStoreRP.Configuration.AccountDataProviderConnectionString'.
- For production environment you need to use https and validate client certificate. You can enable it via 'AzureStoreRP.Configuration.AzureStoreRequestAuthorization' setting.

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
