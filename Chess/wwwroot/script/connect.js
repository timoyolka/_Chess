/*const connectionSignalR = new signalR.HubConnectionBuilder()
    .withUrl("/ChessHub")
    .build();*/

let gameId_global;

connectionSignalR.on("GameReady", function () {
    // $.ajax({
    //     url: '/Pages/UpdateGame',
    //     type: 'GET',
    //     data: { gameId: '@Model.Id' },
    //     success: function (data) {
    //         console.log("game is ready");
    //         location.reload();
    //         $('.container.chess-board').html(data);
    //     },
    //     error: function (xhr, status, error) {
    //         console.error('Error fetching game data:', error);
    //     }
    // });
    console.log("GameReady message recieved from server.");
    location.reload();
});

connectionSignalR.on("MoveMade", function () {
    console.log("The move was legal");
    location.reload();
});

connectionSignalR.on("GameFinished", function () {
    console.log("game is finished");
    hubConnection.invoke("LeaveGroup", Model.gameId)
        .then(() => {
            console.log(`Removed you from group # ${Model.gameId}`);
        })
        .catch(error => {
            console.error(`Error removing connections from group ${groupIdToRemove}: ${error}`);
    });
});



function joinGameOnPageLoad(gameId) {
    connectionSignalR.start().then(function () {
        console.log("Connection to SignalR hub established.");
        connectionSignalR.invoke("JoinGame", gameId)
            .then(function () {
                gameId_global = gameId;
                console.log("Joined the game successfully.");
            })
            .catch(function (err) {
                console.error("Error joining the game: ", err);
            });
        }).catch(function (err) {
            console.error("Error establishing connection to SignalR hub: ", err);
    });
}