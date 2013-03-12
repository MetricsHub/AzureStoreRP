certutil -f -addstore root Startup\Certificates\GTECyberTrustGlobalRoot.cer
certutil -f -addstore CA   Startup\Certificates\GTEMicrosoftInternetAuthority.cer
certutil -f -addstore CA   Startup\Certificates\GTEMicrosoftSecureServerAuthority.cer
certutil -f -addstore My   Startup\Certificates\GTERdfeExtensibilityClientProd.cer

certutil -f -addstore root Startup\Certificates\BaltimoreCyberTrustRoot.cer
certutil -f -addstore CA   Startup\Certificates\BaltimoreMicrosoftInternetAuthority.cer
certutil -f -addstore CA   Startup\Certificates\BaltimoreMSITMachineAuthCA2.cer
certutil -f -addstore My   Startup\Certificates\BaltimoreRdfeExtensibilityClientStage.cer
certutil -f -addstore My   Startup\Certificates\BaltimoreRdfeExtensibilityClientProd.cer

