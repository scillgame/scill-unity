# SCILL.Model.ChallengeCategory
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**is_daily_category** | **bool?** | Indicates if this is the daily category, bringing up new challenges every day for the user to tackle. | [optional] 
**category_position** | **int?** | In the admin panel you set the order of the categories. This is the position index and indicates the position within the categories array. | [optional] 
**category_slug** | **string** | A short name without special chars to make it easier to refer to a specific category (in code) that is language and id agnostic. | [optional] 
**category_name** | **string** | The name of the category in the local language set as the query parameter. | [optional] 
**category_id** | **string** | Indicates how many “tasks” must be completed or done to complete this challenge. | [optional] 
**challenges** | [**List&lt;Challenge&gt;**](Challenge.md) | An array of Challenge objects. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

