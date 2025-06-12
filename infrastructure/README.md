# Infrastructure

This folder contains the infrastructure code for the BookMe application. It uses Terraform to provision the necessary resources in Azure.

## Prerequisites

- [Terraform](https://www.terraform.io/downloads)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [Azure subscription](https://azure.microsoft.com/en-us/free/)

## Usage

1. Login to Azure using the Azure CLI

```bash
az login
```

2. Initialize Terraform

```bash
terraform init
```

3. Plan the infrastructure

```bash
terraform plan
```

4. Apply the infrastructure

```bash
terraform apply
```

5. Destroy the infrastructure

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
