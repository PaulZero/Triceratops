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
        console.log('Trying to emit an error of "' + error + '"');

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

var ApiClient = {
    connection: null,
    currentLogStream: null,
    init: function () {
        if (this.connection == null) {
            this.connection = new signalR.HubConnectionBuilder().withUrl("/ws/dashboard").build();

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
            console.log('Server details received with some JSON', json);

            var serverDetails = JSON.parse(json);

            if (serverDetails.success) {
                ApiEvents.EmitServerDetailsReceivedEvent(serverDetails);
            }
        });
    },
    joinStream: function (serverId) {
        if (this.currentLogStream != null) {
            this.currentLogStream.dispose();
        }

        var $logTextarea = $("#serverLogViewer");

        var streamResult = this.connection
            .stream("ServerLogsAsync", serverId);

        // You can call dispose() on this bollocks to effectively kill it from this end, LOL
        this.currentLogStream = streamResult.subscribe({
            next: (item) => {
                console.log(item);
                $logTextarea.prepend('<div class="log-item">' + item + '</div>');
            },
            complete: () => {
                console.log('It finished, I do not remember asking for this?');
            },
            error: (err) => {
                console.log('It fucked up *angery reacts only*' + err)
            },
        });

        return this.currentLogStream;
    },
    openConnection: function () {
        this.connection.start()
            .then(function () {
                if (typeof currentServerId != "undefined") {
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
        console.log('refreshServerStatus called with a server ID of "' + serverId + '"');

        var operationType = ApiEvents.OperationTypes.ServerDetailsRequested;

        ApiEvents.EmitStartedEvent(serverId, operationType);

        this.connection.invoke("GetServerDetailsAsync", serverId)
            .catch(function (error) {
                ApiEvents.EmitFailedEvent(serverId, operationType, error);
            });
    }
};

ApiClient.init();