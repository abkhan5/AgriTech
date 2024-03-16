Create a vm for build following this
https://github.com/actions/runner-images/blob/main/docs/create-image-and-azure-resources.md


#Provide the subscription Id
$subscriptionId="c1721045-35c4-4799-81b3-06cb7a0ffac0"

#Provide the name of your resource group.
#Ensure that resource group is already created 
$resourceGroupName=rg-ee-azuredevops

#Provide the name of the Managed Disk
$diskName=buildagentlinuxosdisk

#Provide the size of the disks in GB. It should be greater than the VHD file size.
$diskSize=128


#Provide the URI of the VHD file that will be used to create Managed Disk. 
# VHD file can be deleted as soon as Managed Disk is created.
# e.g. https://contosostorageaccount1.blob.core.windows.net/vhds/contosovhd123.vhd 
$vhdUri='https://rgeeado001.blob.core.windows.net/vmcontainerec417fa8-1f52-46e1-ab32-0e07c37e8b13/osDisk.ec417fa8-1f52-46e1-ab32-0e07c37e8b13.vhd'

#Provide the storage type for the Managed Disk. Premium_LRS or Standard_LRS.
$storageType='Premium_LRS'


#Provide the Azure location (e.g. westus) where Managed Disk will be located. 
#The location should be same as the location of the storage account where VHD file is stored.
#Get all the Azure location supported for your subscription using command below:
#az account list-locations
$location='centralindia'

#Set the context to the subscription Id where Managed Disk will be created
az account set --subscription $subscriptionId

#Create the Managed disk from the VHD file 
az disk create --resource-group $resourceGroupName --name $diskName --sku $storageType --location $location --size-gb $diskSize --source $vhdUri

