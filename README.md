# new8

Brief description of purpose of microservice.

## How to run MS locally

### Prerequisites
* Perform minikube environment initialization, please refer to files sitting here:
	* https://github.com/BHFDigital/BHF.Microservices/tree/main/local-development/Install-minikube.ps1
	* https://github.com/BHFDigital/BHF.Microservices/tree/main/local-development/start-minikube.ps1
	* https://github.com/BHFDigital/BHF.Microservices/blob/main/README.md

### To run only this microservice manually
* Copy code\BHF.MS.new8\yaml\configMap-development.yml.example as "configMap-development.yml"
* Fill in the inner variables with relevant secrets located inside "uks-local-dev-kv" KeyVault on Azure portal, secret names to look for:
	* DEV--ConnectionStrings--ClientId
	* DEV--ConnectionStrings--TenantId
	* DEV--ConnectionStrings--ClientSecret

### To run all microservices in bulk (includes configMap bulk auto population)
* Please refer to bulk deployment script sitting here:
	* https://github.com/BHFDigital/BHF.Microservices/tree/main/local-development/bulk-deploy.ps1
	* https://github.com/BHFDigital/BHF.Microservices/tree/main/local-development/bulk-deploy-variables.json.example
	* https://github.com/BHFDigital/BHF.Microservices/blob/main/README.md

### How to work with migrations
* Every single migration command requires Args parameter containing full query string to the databse, sample command below:
	* Add-Migration InitialMigration -Context CustomDbContext -Project BHF.MS.new8.Database -StartupProject BHF.MS.new8.Database -Args "Data Source=(local);Initial Catalog=new8;User ID=[user];Password=[passwsord];Trust Server Certificate=True"
	* Context - should be set to the name od DbContext class you want to execute your migration command
	* Project and StartupProject - those should match the name of the project containing your context class
	* Args - should contain a single parameter, which equals full query string (all of those parameters are supported for every migration command)
* Further reading: https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs

## How to override my config entries if I'm not using minikube
* Please just duplicate appsettings.json as "appsettings.Development.json" and override any settings here. This file is added to gitignore, so you won't push it by mistake.

## Further references

Links to related Confluence pages/related third-party documentation.
