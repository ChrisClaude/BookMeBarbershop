# Infrastructure

This folder contains the infrastructure code for the BookMe application. It uses Terraform to provision the necessary resources in Azure.

## Prerequisites

- [Terraform](https://www.terraform.io/downloads)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Azure subscription](https://azure.microsoft.com/en-us/free/)

## Usage

- Login to Azure using the Azure CLI

  ```bash
  az login
  ```

- Initialize Terraform

  ```bash
  terraform init
  ```

- Plan the infrastructure

  ```bash
  terraform plan
  ```

- Apply the infrastructure

  ```bash
  terraform apply
  ```

- Destroy the infrastructure

  ```bash
  terraform destroy
  ```

## Other commands

```bash
# Format the code
terraform fmt

# Validate the code
terraform validate

# Show the state
terraform show

# Show the list of resources
terraform state list

# Show the output
terraform output

# Show the resource
terraform state show <resource>
```

[Azure Estimation](https://azure.com/e/4aefaf11653e4e6894695f18fdee782b)
[Terraform - store remote state](https://developer.hashicorp.com/terraform/tutorials/azure-get-started/azure-remote)

## Provide current user/service principal access to Key Vault via an access policy

- Get your object id

  ```bash
  az ad signed-in-user show --query id -o tsv
  ```

- Add the access policy

  ```bash
  az keyvault set-policy --name <your-key-vault-name> --object-id <your-object-id> --secret-permissions get list set
  ```
