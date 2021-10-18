# SCILL.Model.ChallengeWebhookPayload
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**webhook_type** | **string** | The type of the webhook. Depending on the module, there are different webhook types indicating different events. Check the reference documentation to see all types. | [optional] 
**category_position** | [**decimal?**](BigDecimal.md) | The index of the category this challenge is linked to. When you request personal challenges, you get an array of categories which contain an array of challenges in their challenges property. This value indicates in which category this challenge can be found. Speeds up updating UI as you don&#x27;t need to iterate through all catagories and challenges to find the challenge. | [optional] 
**user_token** | **string** | The access token for the user of that challenge. You can use that user_token to directly send another event and therefore to chain different SCILL pieces together. For example you can send another event driving another challenge or battle pass whenever a user has completed a challenge. | [optional] 
**new_challenge** | [**Challenge**](Challenge.md) |  | [optional] 
**old_challenge** | [**Challenge**](Challenge.md) |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

