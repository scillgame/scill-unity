# SCILL.Model.BattlePassState
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**battle_pass_id** | **string** | The unique id of this battle pass. | [optional] 
**app_id** | **string** | The unique id of the app | [optional] 
**battle_pass_priority** | **int?** | The priority of the battle pass. I.e. if multiple are available you can use this field to sort them. | [optional] 
**start_date** | **string** | The date (in iso format) when the Battle Pass starts. Tracking begins once this date is passed. | [optional] 
**end_date** | **string** | The date (in iso format) when the Battle Pass ends. Tracking stops once the end is reached and users will not be able to progress further than what they have achieved up to that point. | [optional] 
**is_active** | **bool?** | Indicated if this battle pass is active. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

