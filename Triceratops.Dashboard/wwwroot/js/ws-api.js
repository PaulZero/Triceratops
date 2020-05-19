"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/ws-api").build();
var redirectAfterCompletion = false;

connection.on("OperationComplete", function (json) {    
    var obj = JSON.parse(json);

    if (obj.Success && redirectAfterCompletion) {
        window.location.href = window.location.origin;
    }

    hideLoadingMessage(obj.ServerId);
    updateServerButtons(obj.ServerId, obj.IsRunning);
    updateServerNavBar(obj.IsRunning);
    enableButtonsForServer(obj.ServerId);
});

connection.start()
.catch(function (err) {
    return console.error(err.toString());
});

function startServer(guid) {
    displayLoadingMessage(guid, 'Starting server');
    disableButtonsForServer(guid);

    connection.invoke("StartServerAsync", guid)
        .catch(function (error) {
            return console.error(error.toString());
        });
}

function stopServer(guid) {
    displayLoadingMessage(guid, 'Stopping server');
    disableButtonsForServer(guid);

    connection.invoke("StopServerAsync", guid)
        .catch(function (error) {
            return console.error(error.toString());
        });
}

function restartServer(guid) {
    displayLoadingMessage(guid, 'Restarting server');
    disableButtonsForServer(guid);

    connection.invoke("RestartServerAsync", guid)
        .catch(function (error) {
            return console.error(error.toString());
        });
}

function deleteServer(guid) {
    redirectAfterCompletion = true;
    displayLoadingMessage(guid, 'Deleting server');
    disableButtonsForServer(guid);

    connection.invoke("DeleteServerAsync", guid)
        .catch(function (error) {
            return console.error(error.toString());
        });
}

function displayLoadingMessage(guid, message) {    
    $('.server-update-status-' + guid + ' .update-status-text').text(message);
    $('.server-controls-' + guid).hide();
    $('.server-update-status-' + guid).show();
}

function hideLoadingMessage(guid) {
    $('.server-update-status-' + guid).hide();
    $('.server-controls-' + guid).show();
}

function disableButtonsForServer(guid) {
    $('.server-controls-' + guid).addClass('disabled');
}

function enableButtonsForServer(guid) {
    $('.server-controls-' + guid).removeClass('disabled');
}

function updateServerButtons(guid, isRunning) {
    $('.server-controls-' + guid + ' .show-when-running').toggle(isRunning)
    $('.server-controls-' + guid + ' .show-when-stopped').toggle(!isRunning);
}

function updateServerNavBar(isRunning) {    
    var $navbarElement = $('.server-details-navbar');

    $navbarElement.toggleClass('serverNavBar_Running', isRunning);
    $navbarElement.toggleClass('serverNavBar_Stopped', !isRunning);
}