certutil -f -addstore root Startup\Certificates\GTECyberTrustGlobalRoot.cer
certutil -f -addstore CA   Startup\Certificates\MicrosoftInternetAuthority.cer
certutil -f -addstore CA   Startup\Certificates\MicrosoftSecureServerAuthority.cer
certutil -f -addstore My   Startup\Certificates\AzureStoreClientCertificate.cer
