"use strict";

var ApiEvents = {
    OperationStarted: "server.operation.started",
    OperationFailed: "server.operation.failed",
    OperationSucceeded: "server.operation.succeeded",
    ServerDetailsRefreshed: "server.details.refreshed",
    OperationTypes: {
        ServerStart: "server.start",
        ServerRestart: "server.restart",
        ServerStop: "server.stop",
        ServerDelete: "server.delete",
        ServerDetailsRefresh: "server.details.refresh"
    },
    EmitStartedEvent: function (serverId, operationType) {
        this.Emit(this.OperationStarted, this.CreateEventData(serverId, operationType));
    },
    EmitFailedEvent: function (serverId, operationType, error) {
        console.log(error);
        this.Emit(this.OperationFailed, {serverId: serverId, operation: operationType, error: error });
    },
    EmitSucceededEvent: function (serverId, operationType) {
        this.Emit(this.OperationSucceeded, this.CreateEventData(serverId, operationType));
    },
    EmitServerDetailsReceivedEvent: function (serverDetails) {
        this.Emit(this.ServerDetailsRefreshed, { server: serverDetails });
    },
    Emit: function (eventName, data) {
        window.dispatchEvent(new CustomEvent(eventName, { detail: data }));
    },
    CreateEventData: function (serverId, operationType) {
        return {
            serverId: serverId,
            operation: operationType
        };
    }
};

//window.addEventListener(ApiEvents.OperationStarted, function (e) {    
//    console.log("Server operation started: " + e.detail.operation + " for server " + e.detail.serverId);
//});

//window.addEventListener(ApiEvents.OperationFailed, function (e) {
//    console.log("Server operation failed: " + e.detail.operation + " for server " + e.detail.serverId);
//});

//window.addEventListener(ApiEvents.OperationSucceeded, function (e) {
//    console.log("Server operation succeeded: " + e.detail.operation + " for server " + e.detail.serverId);
//});

var ApiClient = {
    connection: null,
    init: function () {
        if (this.connection == null) {
            this.connection = new signalR.HubConnectionBuilder().withUrl("/ws-api").build();

            this.registerListeners();
            this.openConnection();
        }
    },
    registerListeners: function () {
        this.connection.on("OperationComplete", function (json) {
            var obj = JSON.parse(json);

            console.log('Data received from server', obj);
        });

        this.connection.on("ServerDetailsReceived", function (json) {
            var serverDetails = JSON.parse(json);

            if (serverDetails.Success) {
                ApiEvents.EmitServerDetailsReceivedEvent(serverDetails);
            }
        });
    },
    openConnection: function () {
        this.connection.start()
            .then(function () {
                if (currentServerId != undefined) {
                    ApiClient.refreshServerStatus(currentServerId);
                }
            })
            .catch(function (err) {
                return console.error(err.toString());
            });
    },
    startServer: function (serverId) {
        var operationType = ApiEvents.OperationTypes.ServerStart;

        ApiEvents.EmitStartedEvent(serverId, operationType);

        this.connection.invoke("StartServerAsync", serverId)
            .then(function () {
                ApiEvents.EmitSucceededEvent(serverId, operationType);
            })
            .catch(function (error) {
                ApiEvents.EmitFailedEvent(serverId, operationType);
            });
    },
    restartServer: function (serverId) {
        var operationType = ApiEvents.OperationTypes.ServerRestart;

        ApiEvents.EmitStartedEvent(serverId, operationType);

        this.connection.invoke("RestartServerAsync", serverId)
            .then(function () {
                ApiEvents.EmitSucceededEvent(serverId, operationType);
            })
            .catch(function (error) {
                ApiEvents.EmitFailedEvent(serverId, operationType, error);
            });
    },
    stopServer: function (serverId) {
        var operationType = ApiEvents.OperationTypes.ServerStop;

        ApiEvents.EmitStartedEvent(serverId, operationType);

        this.connection.invoke("StopServerAsync", serverId)
            .then(function () {
                ApiEvents.EmitSucceededEvent(serverId, operationType);
            })
            .catch(function (error) {
                ApiEvents.EmitFailedEvent(serverId, operationType);
            });
    },
    deleteServer: function (serverId) {
        var operationType = ApiEvents.OperationTypes.ServerDelete;

        ApiEvents.EmitStartedEvent(serverId, operationType);

        this.connection.invoke("DeleteServerAsync", serverId)
            .then(function () {
                ApiEvents.EmitSucceededEvent(serverId, operationType);

                window.location.href = window.location.origin;
            })
            .catch(function (error) {
                ApiEvents.EmitFailedEvent(serverId, operationType);
            });
    },
    refreshServerStatus: function (serverId) {
        console.log(serverId);

        var operationType = ApiEvents.OperationTypes.ServerDetailsRequested;

        ApiEvents.EmitStartedEvent(serverId, operationType);

        this.connection.invoke("GetServerDetailsAsync", serverId)
            .catch(function (error) {
                ApiEvents.EmitFailedEvent(serverId, operationType, error);
            });
    }
};

ApiClient.init();