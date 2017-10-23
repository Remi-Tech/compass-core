
# Compass Core Server #

## Description ##

This is an awesome API Gateway.

## API Reference ##

### Endpoints ###

register/(applicationName}
revoke/{applicationToken}
/applications
/subscribe
/heartbeat

### Register Application ###

### Request Example ###

~~~~
{
    "applicationName": "Test"
}
~~~~

### Response Example ###

~~~~
{
    "docType": "RegisteredApplication",
    "applicationName": "Test",
    "applicationToken": "718cbbf0-2bce-439a-ac76-f7c470143ecf",
    "lastSeen": null,
    "lastEventsSubscribed": [],
    "isRevoked": false,
    "dateCreated": "2017-09-06T13:44:08.14509Z",
    "identifier": "718cbbf0-2bce-439a-ac76-f7c470143ecf"
}
~~~~

### Revoke Application ###

### Request Example ###

~~~~
{
    "applicationToken": "718cbbf0-2bce-439a-ac76-f7c470143ecf"
}
~~~~

### Response Example ###

~~~~
{
    "docType": "RegisteredApplication",
    "applicationName": "Test",
    "applicationToken": "718cbbf0-2bce-439a-ac76-f7c470143ecf",
    "lastSeen": null,
    "lastEventsSubscribed": [],
    "isRevoked": true,
    "dateCreated": "2017-09-06T13:44:08.14509Z",
    "identifier": "718cbbf0-2bce-439a-ac76-f7c470143ecf"
}
~~~~

### View All Applications ###

### Response Example ###

~~~~
{
    "docType": "RegisteredApplication",
    "applicationName": "Test",
    "applicationToken": "718cbbf0-2bce-439a-ac76-f7c470143ecf",
    "lastSeen": null,
    "lastEventsSubscribed": [],
    "isRevoked": true,
    "dateCreated": "2017-09-06T13:44:08.14509Z",
    "identifier": "718cbbf0-2bce-439a-ac76-f7c470143ecf"
}
~~~~

### Subscribe ###

### Request Example ###

~~~~
{
    "applicationToken": "ba03e186-760f-4bff-b06c-bcc9211ded91",
    "applicationUri": "https://www.example.com/",
    "subscribedEvents": [
        "test",
        "test1",
        "test2"
        ]
}
~~~~ 

### Response Example ###

~~~~
{
    "identifier": "ba03e186-760f-4bff-b06c-bcc9211ded91",
    "applicationToken": "ba03e186-760f-4bff-b06c-bcc9211ded91",
    "applicationUri": "https://www.example.com/",
    "subscribedEvents": [
        "test",
        "test1",
        "test2"
    ],
    "docType": "ServiceSubscription"
}
~~~~

### Heartbeat ###

### Request Example ###

~~~~
{
    "applicationToken": "ba03e186-760f-4bff-b06c-bcc9211ded91",
    "applicationUri": "https://www.example.com/",
    "subscribedEvents": [
        "test",
        "test1",
        "test2"
        ]
}
~~~~ 

### Route Request ###

### Request Example ###

~~~~
{
    "userId": "123456"",
    "applicationToken": "ba03e186-760f-4bff-b06c-bcc9211ded91",
    "eventName": "test",
    "payload": {
        "property1": "value1"
    }
}
~~~~ 

### Response Example ###

~~~~
{
    "message": "Request success",
    "status": "200 OK",
    "response": {
        {
            "userId": 123456,
            "eventName": "test",
            "applicationToken": "ba03e186-760f-4bff-b06c-bcc9211ded91",
            "payload": {
            "property1": "value1"
            },
            "docType": "CompassEvent",
            "identifier": "57bf4a90-ce79-4423-930f-03471fd3268e"
        }
    }
}
~~~~

## Environment Variables ##

~~~~
CompassCouchbaseUri
~~~~
Use : The uri for the backing Couchbase instance where configuration and events are stored.

~~~~
CompassCouchbaseBucketName
~~~~
Use : The name of the backing Couchbase bucket where configuration and events are stored.

~~~~
KafkaBrokerList
~~~~
Use : The uri for the Kafka broker node.

~~~~