# SCILL.Model.LeaderboardMemberMetadata
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**key** | **string** | The event type key used for ranking | [optional] 
**ranked** | **bool?** | Determines if the user is ranked for that event type key. If false, no score and rank will be provided | [optional] 
**score** | **int?** | The score achieved as an integer value. If you want to store floats, for example laptimes you need to convert them into an int before (i.e. multiply by 100 to get hundreds of seconds and format back to float in UI) | [optional] 
**rank** | **int?** | The position within the leaderboard | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

