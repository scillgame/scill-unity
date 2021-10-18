# SCILL.Model.LeaderboardRanking
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**member_id** | **string** | The id of the user - its the same user id you used to create the access token and the same user id you used to send the events | [optional] 
**member_type** | **string** | Indicates what type this entry is, it&#x27;s either user or team | [optional] 
**score** | **int?** | The score achieved as an integer value. If you want to store floats, for example laptimes you need to convert them into an int before (i.e. multiply by 100 to get hundreds of seconds and format back to float in UI) | [optional] 
**rank** | **int?** | The position within the leaderboard | [optional] 
**additional_info** | [**UserInfo**](UserInfo.md) |  | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

