# SCILL.Model.EventPayload
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**user_id** | **string** | This is your user id. You can set this to whatever you like, either your real user id or an obfuscated user id. However you need to be consistent here. Events linked to this user id only track if challenges or battle passes are unlocked with the same user id. | [optional] 
**session_id** | **string** | This is required if event_type is single and identifies a session. This can be anything used to group events together. For example this can be a level or a match id. | [optional] 
**event_name** | **string** | This is the event type as a string. These have predefined event names for many games and applications. It’s wise to use those as this allows us to analyse data and help you balancing your application or game. | [optional] 
**event_type** | **string** | This is either single or group. You can send multiple events in one request (group) or send events in sequence. Please note, that depending on your tier you might run into rate limits. | [optional] [default to "single"]
**meta_data** | [**EventMetaData**](EventMetaData.md) |  | [optional] 
**team_id** | **string** | Provide an optional team id that will be used in leaderboards to group results of teams. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

